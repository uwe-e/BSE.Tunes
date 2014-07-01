using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
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
        private Stream m_responseStream;
        private bool m_isCanceled;
        private long m_bytesReceived;
        private long m_totalBytesToReceive;
        private IStorageFile m_storageFile;
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
        public AudioStreamDownloader()
        {
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
            this.m_totalBytesToReceive = 0;
            this.m_bytesReceived = 0;
            this.m_storageFile = null;
            this.m_isCanceled = false;
            bool hasDownloadStarted = false;

            try
            {
                this.OnDownloadStarting();
                this.m_totalBytesToReceive = await this.GetFileSizeAsync(source);
                if (this.m_totalBytesToReceive > 0)
                {
                    this.m_responseStream = await this.CreateStream(trackId);
                    this.m_bytesReceived = this.m_responseStream.Length;
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
                        do
                        {
                            if (this.m_isCanceled)
                            {
                                return;
                            }
                            else
                            {
                                if (this.m_bytesReceived >= ResponseContentBufferSize)
                                {
                                    if (!hasDownloadStarted)
                                    {
                                        hasDownloadStarted = true;
                                        this.OnDownloadProgessStarted();
                                    }
                                }
                                long newTo = this.m_bytesReceived + (ResponseContentBufferSize - 1);
                                int receivedBytes = await this.GetReceivedBytes(this.m_responseStream, source, this.m_bytesReceived, newTo);
                                this.m_bytesReceived += (long)receivedBytes;
                            }
                        }
                        while (this.m_bytesReceived != this.m_totalBytesToReceive);
                    }
                }
            }
            catch (WebException webException)
            {
            }
            catch (HttpRequestException httpRequestException)
            {
                throw httpRequestException;
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                //System.Threading.Tasks.Task.Run(async () => await LocalStorage.ClearTempFolderAsync());
                //var test = Task.Run(async ()=> await DownloadAsync(source, trackId));

            }
            catch (Exception exception)
            {
                //return;
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
                    if (this.m_storageFile != null)
                    {
                        this.m_storageFile = null;
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
        private async Task<Stream> CreateStream(Guid trackId)
        {
            IStorageFolder storageFolder = await LocalStorage.GetTempFolderAsync();
            this.m_storageFile = await storageFolder.CreateFileAsync(trackId.ToString(), CreationCollisionOption.OpenIfExists) as StorageFile;
            IRandomAccessStream windowsRuntimeStream = await this.m_storageFile.OpenAsync(FileAccessMode.ReadWrite);
            return windowsRuntimeStream.AsStream();
        }
        private async Task<long> GetFileSizeAsync(Uri uri)
        {
            long contentLength = -1;
            using (var httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, uri))
                {
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            contentLength = response.Content.Headers.ContentLength.GetValueOrDefault(0);
                        }
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
                using (var httpClient = new HttpClient())
                {
                    httpClient.AddRange(rangeFrom, rangeTo);
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
                    {
                        using (var responseMessage = await httpClient.SendAsync(request))
                        {
                            responseMessage.EnsureSuccessStatusCode();
                            receivedBytes = (int)responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
                            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                            {
                                try
                                {
                                    responseStream.Position = rangeFrom;
                                    await stream.CopyToAsync(this.m_responseStream, ResponseContentBufferSize);
                                    responseStream.Flush();
                                }
                                catch (Exception e)
                                {
                                    throw e;
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