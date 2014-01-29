using BSE.Net.Http;
using BSE.Tunes.Data;
using BSE.Tunes.WebApi.Configuration;
using BSE.Tunes.WebApi.Providers;
using BSE.Tunes.WebApi.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
    [RoutePrefix("api/files")]
    [Authorize(Roles = "tunesusers")]
    public class FilesController : ApiController
    {
        #region FieldsPrivate
        private ITunesService m_tunesService;
        private FileProvider m_fileProvider;
        private ImpersonationUser m_impersonationUser;
        // Track whether Dispose has been called.
        private bool m_isDisposed;
        #endregion

        #region MethodsPublic
        public FilesController()
        {
            this.m_fileProvider = new FileProvider();
            this.m_tunesService = new BSE.Tunes.Entities.TunesBusinessObject();
            this.m_impersonationUser = Settings.ImpersonationUser;
        }

        //[Route("{id}")]
        public HttpResponseMessage GetAudioFile(string id)
        {
            HttpResponseMessage responseMessage = null;
            if (string.IsNullOrEmpty(id) == false)
            {
                Guid guid = Guid.Empty;
                if (Guid.TryParse(id, out guid))
                {
                    string fileName = this.m_tunesService.GetAudioFileNameByGuid(guid);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        using (var impersonator = new Impersonator(
                            this.m_impersonationUser.Username,
                            this.m_impersonationUser.Domain,
                            this.m_impersonationUser.Password,
                            this.m_impersonationUser.LogonType))
                        {
                            if (!this.m_fileProvider.Exists(fileName))
                            {
                                throw new HttpResponseException(HttpStatusCode.NotFound);
                            }
                            var fileStream = this.m_fileProvider.Open(fileName);
                            responseMessage = new HttpResponseMessage();
                            responseMessage.Content = new StreamContent(fileStream);
                            responseMessage.Content.Headers.ContentLength = fileStream.Length;
                            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                }
            }
            return responseMessage;
        }
        [Route("{id}")]
        public HttpResponseMessage GetFile(string id)
        {
            HttpResponseMessage responseMessage = null;
            if (string.IsNullOrEmpty(id) == false)
            {
                Guid guid = Guid.Empty;
                if (Guid.TryParse(id, out guid))
                {
                    string fileName = this.m_tunesService.GetAudioFileNameByGuid(guid);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //var audio = new AudioStream(fileName);


                        responseMessage = Request.CreateResponse();
                        //responseMessage.Content = new PushStreamContent(audio.WriteToStream, new MediaTypeHeaderValue("application/octet-stream"));

                        responseMessage.Content = new PushStreamContent(async (Stream outputStream, HttpContent content, TransportContext context) =>
                        {
                            using (var impersonator = new Impersonator(
                                                        this.m_impersonationUser.Username,
                                                        this.m_impersonationUser.Domain,
                                                        this.m_impersonationUser.Password,
                                                        this.m_impersonationUser.LogonType))
                            {
                                try
                                {
                                    var buffer = new byte[65536];

                                    using (var audio = File.Open(fileName, FileMode.Open, FileAccess.Read))
                                    {
                                        var length = (int)audio.Length;
                                        var bytesRead = 1;

                                        while (length > 0 && bytesRead > 0)
                                        {
                                            bytesRead = audio.Read(buffer, 0, Math.Min(length, buffer.Length));
                                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                                            length -= bytesRead;
                                        }
                                    }
                                }
                                finally
                                {
                                    outputStream.Close();
                                }
                            }
                        }, new MediaTypeHeaderValue("application/octet-stream"));
                    }
                }
            }
            return responseMessage;
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
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.m_isDisposed == false)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (this.m_tunesService != null)
                    {
                        this.m_tunesService = null;
                    }
                    if (this.m_fileProvider != null)
                    {
                        this.m_fileProvider.Dispose();
                    }
                }
                this.m_isDisposed = true;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}