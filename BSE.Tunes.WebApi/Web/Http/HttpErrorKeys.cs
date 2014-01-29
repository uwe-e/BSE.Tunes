using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE.Web.Http
{
    /// <summary>
    /// Provides keys to look up error information stored in the <see cref="HttpError"/> dictionary.
    /// </summary>
    public static class HttpErrorKeys
    {
        /// <summary>
        /// Provides a key for the ExceptionSource.
        /// </summary>
        public static readonly string ExceptionSourceKey = "ExceptionSource";
    }
}