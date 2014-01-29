using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using BSE.Tunes.Data.Exceptions;

namespace BSE.Tunes.WebApi.Filters
{
    /// <summary>
    /// Represents an exception filter.
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("{0}\t{1}", DateTime.Now.ToFileTime(), actionExecutedContext.Exception.Message), "Exception");
            actionExecutedContext.Response = actionExecutedContext.Request.CreateExceptionResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception);
        }
    }
}