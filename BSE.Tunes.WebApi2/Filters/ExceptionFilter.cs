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
	/// <remarks>
	/// An exception filter is executed when a controller method throws any unhandled exception that is not an HttpResponseException exception.
	/// </remarks>
	public class ExceptionFilter : ExceptionFilterAttribute
	{
		/// <summary>
		/// Raises the exception event.
		/// </summary>
		/// <param name="actionExecutedContext">The context for the action.</param>
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			actionExecutedContext.Response = actionExecutedContext.Request.CreateExceptionResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception);
		}
	}
}