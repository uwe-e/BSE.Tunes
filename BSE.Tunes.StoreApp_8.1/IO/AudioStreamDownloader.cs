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
    internal class AudioStreamDownloader
    {
        #region Events
        public event EventHandler<DownloaderEventArgs> DownloadStarting;
        public event EventHandler<DownloaderEventArgs> DownloadEnded;
        public event EventHandler<DownloaderEventArgs> CanStartConsumingData;
        public event EventHandler<DownloaderEventArgs> ProgressChanged;
        #endregion

        #region Constants
        internal const int ResponseContentBufferSize = 52000;//512000;
        #endregion

        #region FieldsPrivate
        private Stream m_responseStream;
        private HttpStatusCode m_httpStatusCode;
        private bool m_isCanceled;
        private long m_bytesReceived;
        private long m_totalBytesToReceive;
        #endregion

        #region Properties
        public IRandomAccessStream Stream
        {
            get
            {
                return this.m_responseStream.AsRandomAccessStream();
            }
        }
        #endregion

        #region MethodsPublic
        public AudioStreamDownloader()
        {
        }
        public async Task DownloadAsync(Uri source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.m_totalBytesToReceive = -1;
            bool canStartConsumingDataExecuted = false;
            this.m_responseStream = await this.CreateStream();
            try
            {
                this.OnDownloadStarting(source);
                while (this.m_bytesReceived != this.m_totalBytesToReceive)
                {
                    if (!this.m_isCanceled)
                    {
                        long newTo = this.m_bytesReceived + (ResponseContentBufferSize - 1);
                        int receivedBytes = await this.GetReceivedBytes(source, this.m_bytesReceived, newTo);
                        this.m_bytesReceived += (long)receivedBytes;
                        this.OnProgressChanged(source, this.m_bytesReceived, this.m_totalBytesToReceive);
                        if (!canStartConsumingDataExecuted && (this.m_bytesReceived >= ResponseContentBufferSize || this.m_bytesReceived >= this.m_totalBytesToReceive))
                        {
                            this.OnCanStartConsumingData(source, this.m_bytesReceived, this.m_totalBytesToReceive);
                            canStartConsumingDataExecuted = true;
                        }
                    }
                }
                this.OnDownloadEnded(source, true, null);
            }
            catch (TaskCanceledException taskCanceledException)
            {
                this.OnDownloadEnded(source, true, taskCanceledException);
                return;
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
        protected virtual void OnDownloadStarting(Uri requestUri)
        {
            if (this.DownloadStarting != null)
            {
                this.DownloadStarting(this, new DownloaderEventArgs(requestUri, this.m_responseStream, false, null));
            }
        }
        protected void OnProgressChanged(Uri requestUri, long bytesReceived, long totalBytesToReceive)
        {
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this, new DownloaderEventArgs(requestUri, this.m_responseStream, bytesReceived, totalBytesToReceive));
            }
        }
        protected void OnDownloadEnded(Uri requestUri, bool isCancelled, Exception error)
        {
            if (this.DownloadEnded != null)
            {
                this.DownloadEnded(this, new DownloaderEventArgs(requestUri, this.m_responseStream, isCancelled, error));
            }
        }
        protected void OnCanStartConsumingData(Uri requestUri, long bytesReceived, long totalBytesToReceive)
        {
            if (this.CanStartConsumingData != null)
            {
                this.CanStartConsumingData(this, new DownloaderEventArgs(requestUri, this.m_responseStream, bytesReceived, totalBytesToReceive));
            }
        }
        #endregion

        #region MethodsPrivate
        private async Task<Stream> CreateStream()
        {
            IStorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync("dummy") as Windows.Storage.StorageFile;
            if (file != null)
            {
                await file.DeleteAsync();
            }
            file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("dummy");
            IRandomAccessStream windowsRuntimeStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            return windowsRuntimeStream.AsStream();
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
                        this.m_totalBytesToReceive = responseMessage.Content.Headers.ContentRange.Length.Value;
                        receivedBytes = (int)responseMessage.Content.Headers.ContentLength.GetValueOrDefault(0);
                        using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                        {
                            this.m_responseStream.Seek(rangeFrom, SeekOrigin.Begin);
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
