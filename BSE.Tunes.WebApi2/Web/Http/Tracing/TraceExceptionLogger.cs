using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace BSE.Tunes.WebApi.Web.Http.Tracing
{
	public class TraceExceptionLogger : ExceptionLogger
	{
		private static readonly NLog.Logger classLogger = NLog.LogManager.GetCurrentClassLogger();

		public override void Log(ExceptionLoggerContext context)
		{
			classLogger.Log(NLog.LogLevel.Error, context.Exception, context.Exception.Message);
		}
	}
}