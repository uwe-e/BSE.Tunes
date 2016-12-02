using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Extensions;

namespace BSE.Tunes.StoreApp.IO
{
	internal class AudioStreamDownloader : IDisposable
	{
		#region Events
		public event EventHandler<EventArgs> DownloadStarting;
		public event EventHandler<EventArgs> DownloadProgessStarted;
		#endregion

		#region Constants
		internal const int ResponseContentBufferSize = 512000;
		#endregion

		#region FieldsPrivate
		private IDataService m_dataService;
		private Stream m_responseStream;
		private bool m_isCanceled;
		private HttpStatusCode m_httpStatusCode;
		private long m_bytesReceived;
		private long m_totalBytesToReceive;
		// Track whether Dispose has been called.
		private bool m_bDisposed;
		#endregion

		#region Properties
		public IRandomAccessStream Stream
		{
			get
			{
				return this.m_responseStream.AsRandomAccessStream();
			}
		}
		public long TotalBytesToReceive
		{
			get
			{
				return this.m_totalBytesToReceive;
			}
		}
		#endregion

		#region MethodsPublic
		public AudioStreamDownloader(IDataService dataService)
		{
			this.m_dataService = dataService;
		}
		// Implement IDisposable.
		// Do not make this method virtual.
		// A derived class should not be able to override this method.
		public void Dispose()
		{
			this.Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}
		public async Task DownloadAsync(Uri source, Guid trackId)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.m_totalBytesToReceive = -1;
			this.m_bytesReceived = 0;
			this.m_isCanceled = false;
			bool hasDownloadStarted = false;
			int exceptionAttempt = 0;
			int delayAttempt = 0;

			try
			{
				this.OnDownloadStarting();
				this.m_responseStream = await this.CreateStream(trackId);
				this.m_bytesReceived = this.m_responseStream.Length;

				do
				{
					if (this.m_isCanceled)
					{
						return;
					}
					else
					{
						if (delayAttempt == 3)
						{
							await Task.Delay(10000);
							delayAttempt = 0;
						}
						
						try
						{
							//Initialize a new download
							if (this.m_totalBytesToReceive == -1)
							{
								this.m_totalBytesToReceive = await this.GetFileSizeAsync(source);
							}
							if (this.m_bytesReceived.Equals(this.m_totalBytesToReceive))
							{
								if (!hasDownloadStarted)
								{
									hasDownloadStarted = true;
									this.OnDownloadProgessStarted();
								}
							}
							else
							{
								long newTo = this.m_bytesReceived + (ResponseContentBufferSize - 1);
								int receivedBytes = await this.GetReceivedBytes(this.m_responseStream, source, this.m_bytesReceived, newTo);
								this.m_bytesReceived += (long)receivedBytes;
								if (!hasDownloadStarted)
								{
									hasDownloadStarted = true;
									this.OnDownloadProgessStarted();
								}
								exceptionAttempt = 0;
							}
						}
						catch (HttpRequestException httpRequestException)
						{
							if (exceptionAttempt == 13)
							{
								throw httpRequestException;
							}
							//If the used server a windows 8 computer, than we get sometimes a StatusCode 404 exception.
							//After a few seconds the communication with the server works again
							if (this.m_httpStatusCode == HttpStatusCode.NotFound)
							{
								delayAttempt += 1;
							}
							exceptionAttempt += 1;
						}
						catch (Exception)
						{
							throw;
						}
						continue;
					}
				}
				while (this.m_bytesReceived != this.m_totalBytesToReceive);
			}
			catch (HttpRequestException httpRequestException)
			{
				throw httpRequestException;
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{

			}
			catch (Exception exception)
			{
			}
		}
		public virtual void Close()
		{
			this.m_isCanceled = true;
			this.m_responseStream = null;
		}
		#endregion

		#region MethodsProtected
		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the
		// runtime from inside the finalizer and you should not reference
		// other objects. Only unmanaged resources can be disposed.
		protected void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (this.m_bDisposed == false)
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					if (this.m_responseStream != null)
					{
						this.m_responseStream.Dispose();
						this.m_responseStream = null;
					}
				}
				this.m_bDisposed = true;
			}
		}
		protected virtual void OnDownloadStarting()
		{
			if (this.DownloadStarting != null)
			{
				this.DownloadStarting(this, EventArgs.Empty);
			}
		}
		protected void OnDownloadProgessStarted()
		{
			if (this.DownloadProgessStarted != null)
			{
				this.DownloadProgessStarted(this, EventArgs.Empty);
			}
		}
		#endregion

		#region MethodsPrivate
		private async Task<Stream> CreateStream(Guid trackId, CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
		{
			Stream stream = null;
			bool isUnauthorizedAccess = false;
			try
			{
				await IOUtilities.WrapSharingViolations(async () =>
				{
					IStorageFolder storageFolder = await LocalStorage.GetTempFolderAsync();
					var storageFile = await storageFolder.CreateFileAsync(trackId.ToString(), creationCollisionOption) as StorageFile;
					IRandomAccessStream windowsRuntimeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
					stream = windowsRuntimeStream.AsStream();
				});
			}
			catch (UnauthorizedAccessException)
			{
				isUnauthorizedAccess = true;
			}
			if (isUnauthorizedAccess)
			{
				//When, for example, the new track for playing is the same than the current played track.
				return await Task<Stream>.Run(() =>
				{
					return CreateStream(trackId, CreationCollisionOption.GenerateUniqueName);
				});
			}
			return stream;
		}
		private async Task<long> GetFileSizeAsync(Uri uri)
		{
			long contentLength = -1;
			using(var httpClient = await this.m_dataService.GetHttpClient())
			{
				httpClient.Timeout = new TimeSpan(0, 0, 0, 12);
				using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, uri))
				{
					using (HttpResponseMessage responseMessage = await httpClient.SendAsync(request))
					{
						this.m_httpStatusCode = responseMessage.StatusCode;
						responseMessage.EnsureSuccessStatusCode();
						contentLength = responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
					}
				}
			}
			return contentLength;
		}
		private async Task<int> GetReceivedBytes(Stream responseStream, Uri uri, long rangeFrom, long rangeTo)
		{
			int receivedBytes = -1;
			if (responseStream != null)
			{
				using (var httpClient = await this.m_dataService.GetHttpClient(false))
				{
					httpClient.AddRange(rangeFrom, rangeTo);
					using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
					{
						using (var responseMessage = await httpClient.SendAsync(request))
						{
							this.m_httpStatusCode = responseMessage.StatusCode;
							responseMessage.EnsureSuccessStatusCode();
							this.m_totalBytesToReceive = responseMessage.Content.Headers.ContentRange.Length.Value;
							receivedBytes = (int)responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
							using (var stream = await responseMessage.Content.ReadAsStreamAsync())
							{
								try
								{
									await IOUtilities.WrapSharingViolations(async () =>
									{
										responseStream.Position = rangeFrom;
										await stream.CopyToAsync(this.m_responseStream, ResponseContentBufferSize);
										responseStream.Flush();
									});
								}
								catch (Exception)
								{
									throw;
								}
							}
						}
					}
				}
			}
			return receivedBytes;
		}
		#endregion
	}
}