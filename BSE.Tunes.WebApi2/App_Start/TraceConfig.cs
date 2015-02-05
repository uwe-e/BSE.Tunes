using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace BSE.Tunes.WebApi.App_Start
{
	public static class TraceConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.Services.Replace(typeof(ITraceWriter), new BSE.Tunes.WebApi.Web.Http.Tracing.NLogTraceWriter());
		}
	}
}