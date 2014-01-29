using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace System.Net.Http
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpRequestMessage"/> class.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Helper method that performs content negotiation and creates a <see cref="HttpResponseMessage"/> representing an error
        /// with an instance of <see cref="ObjectContent{T}"/> wrapping an <see cref="HttpError"/> for exception <paramref name="exception"/>.
        /// If no formatter is found, this method returns a response with status 406 NotAcceptable.
        /// </summary>
        /// <remarks>
        /// This method requires that <paramref name="request"/> has been associated with an instance of
        /// <see cref="HttpConfiguration"/>.
        /// </remarks>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code of the created response.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>An error response for <paramref name="exception"/> with status code <paramref name="statusCode"/>.</returns>
        public static HttpResponseMessage CreateExceptionResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Exception exception)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return request.CreateExceptionResponse(statusCode, includeErrorDetail => new BSE.Web.Http.HttpError(exception, includeErrorDetail));
        }

        private static HttpResponseMessage CreateExceptionResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, BSE.Web.Http.HttpError> errorCreator)
        {
            HttpConfiguration configuration = request.GetConfiguration();
            BSE.Web.Http.HttpError error = errorCreator(request.ShouldIncludeErrorDetail());

            if (configuration == null)
            {
                using (HttpConfiguration defaultConfig = new HttpConfiguration())
                {
                    return request.CreateResponse<BSE.Web.Http.HttpError>(statusCode, error, defaultConfig);
                }
            }
            else
            {
                return request.CreateResponse<BSE.Web.Http.HttpError>(statusCode, error, configuration);
            }
        }
    }
}