using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Throws an exception if the IsSuccessStatusCode property for the HTTP response is false.
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> object.</param>
        public static void EnsureExtendedSuccessStatusCode(this HttpResponseMessage responseMessage)
        {
            if (responseMessage != null)
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.Content != null)
                    {
                        var exception = responseMessage.Content.ReadExceptionAsAsync().Result;
                        responseMessage.Content.Dispose();
                        throw exception ?? new HttpRequestException(
                            string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
                            {
                                (int)responseMessage.StatusCode,
                                responseMessage.ReasonPhrase
                            }));
                    }
                }
            }
        }
    }
}
