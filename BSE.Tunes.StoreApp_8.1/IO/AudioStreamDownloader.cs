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
        public event EventHandler<DownloaderEventArgs> DownloadStarting;
        public event EventHandler<DownloaderEventArgs> DownloadEnded;
        public event EventHandler<DownloaderEventArgs> DownloadProgessStarted;
        #endregion

        #region Constants
        internal const int ResponseContentBufferSize = 512000;
        #endregion

        #region FieldsPrivate
        private Stream m_responseStream;
        private bool m_isCanceled;
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
            this.m_isCanceled = false;
            bool hasDownloadStarted = false;
            this.m_responseStream = await this.CreateStream(trackId);
            try
            {
                this.OnDownloadStarting(source);
                this.m_totalBytesToReceive = await this.GetFileSizeAsync(source);

                do
                {
                    if (this.m_isCanceled)
                    {
                        return;
                    }
                    else
                    {
                        long newTo = this.m_bytesReceived + (ResponseContentBufferSize - 1);
                        int receivedBytes = await this.GetReceivedBytes(source, this.m_bytesReceived, newTo);
                        this.m_bytesReceived += (long)receivedBytes;
                        if (!hasDownloadStarted)
                        {
                            hasDownloadStarted = true;
                            this.OnDownloadProgessStarted(source, this.m_bytesReceived, this.m_totalBytesToReceive);
                        }
                    }
                }
                while (this.m_bytesReceived != this.m_totalBytesToReceive);
                this.OnDownloadEnded(source, true, null);
            }
            catch (WebException webException)
            {
            }
            catch (HttpRequestException httpRequestException)
            {

            }
            catch (Exception exception)
            {
                this.OnDownloadEnded(source, false, exception);
                return;
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
        protected virtual void OnDownloadStarting(Uri requestUri)
        {
            if (this.DownloadStarting != null)
            {
                this.DownloadStarting(this, new DownloaderEventArgs(requestUri, this.m_responseStream, false, null));
            }
        }
        protected void OnDownloadEnded(Uri requestUri, bool isCancelled, Exception error)
        {
            if (this.DownloadEnded != null)
            {
                this.DownloadEnded(this, new DownloaderEventArgs(requestUri, this.m_responseStream, isCancelled, error));
            }
        }
        protected void OnDownloadProgessStarted(Uri requestUri, long bytesReceived, long totalBytesToReceive)
        {
            if (this.DownloadProgessStarted != null)
            {
                this.DownloadProgessStarted(this, new DownloaderEventArgs(requestUri, this.m_responseStream, bytesReceived, totalBytesToReceive));
            }
        }
        #endregion

        #region MethodsPrivate
        private async Task<Stream> CreateStream(Guid trackId)
        {
            IStorageFolder storageFolder = await LocalStorage.GetTempFolderAsync();
            IStorageFile file = await storageFolder.CreateFileAsync(trackId.ToString(), CreationCollisionOption.GenerateUniqueName) as StorageFile;
            IRandomAccessStream windowsRuntimeStream = await file.OpenAsync(FileAccessMode.ReadWrite);
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
        private async Task<int> GetReceivedBytes(Uri uri, long rangeFrom, long rangeTo)
        {
            int receivedBytes = -1;
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
                            this.m_responseStream.Position = rangeFrom;
                            await stream.CopyToAsync(this.m_responseStream, ResponseContentBufferSize);
                            this.m_responseStream.Flush();
                        }
                    }
                }
            }
            return receivedBytes;
        }
        #endregion
    }
}
