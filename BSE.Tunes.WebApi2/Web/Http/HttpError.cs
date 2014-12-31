using BSE.Tunes.Data.Exceptions;
using BSE.Tunes.WebApi.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;

namespace BSE.Web.Http
{
    /// <summary>
    /// Defines a serializable container for storing error information. This information is stored
    /// as key/value pairs. The dictionary keys to look up extended error information are available
    /// on the <see cref="System.Web.Http.HttpErrorKeys"/> and <see cref="HttpErrorKeys"/> types.
    /// </summary>
    public class HttpError : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpError"/> class.
        /// </summary>
        public HttpError()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpError"/> class for <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The exception to use for error information.</param>
        /// <param name="includeErrorDetail"><c>true</c> to include the exception information in the error; <c>false</c> otherwise</param>
        public HttpError(Exception exception, bool includeErrorDetail)
            : this()
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Message = SRResources.ErrorOccurred;

            if (includeErrorDetail)
            {
                Add(System.Web.Http.HttpErrorKeys.ExceptionMessageKey, exception.Message);
                Add(System.Web.Http.HttpErrorKeys.ExceptionTypeKey, exception.GetType().FullName);
                var exceptionSource = exception.Source;
                BSEtunesException tunesException = exception as BSEtunesException;
                if (tunesException != null)
                {
                    exceptionSource = tunesException.AssemblyName;
                }
                Add(HttpErrorKeys.ExceptionSourceKey, exceptionSource);
                if (exception.InnerException != null)
                {
                    Add(System.Web.Http.HttpErrorKeys.InnerExceptionKey, new HttpError(exception.InnerException, includeErrorDetail));
                }
            }
        }
        /// <summary>
        /// The high-level, user-visible message explaining the cause of the error. Information carried in this field
        /// should be considered public in that it will go over the wire regardless of the <see cref="IncludeErrorDetailPolicy"/>.
        /// As a result care should be taken not to disclose sensitive information about the server or the application.
        /// </summary>
        public string Message
        {
            get { return GetPropertyValue<String>(System.Web.Http.HttpErrorKeys.MessageKey); }
            set { this[System.Web.Http.HttpErrorKeys.MessageKey] = value; }
        }
        /// <summary>
        /// Gets a particular property value from this error instance.
        /// </summary>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="key">The name of the error property.</param>
        /// <returns>The value of the error property.</returns>
        public TValue GetPropertyValue<TValue>(string key)
        {
            object value;
            if (this.TryGetValue(key, out value))
            {
                return (TValue)value;
            }
            return default(TValue);
        }
    }
}