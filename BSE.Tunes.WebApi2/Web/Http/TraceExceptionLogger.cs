using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace BSE.Tunes.WebApi.Web.Http
{
	public class TraceExceptionLogger : ExceptionLogger
	{
		public override void Log(ExceptionLoggerContext context)
		{
			System.Diagnostics.Trace.TraceError(string.Format("{0}\t{1}\t{2}\t{3}", DateTime.Now.ToLocalTime(), context.Request.Method, context.Request.RequestUri, context.Exception.Message));
		}
	}
}