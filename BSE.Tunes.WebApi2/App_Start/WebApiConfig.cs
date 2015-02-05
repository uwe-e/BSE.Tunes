using BSE.Tunes.WebApi.Web.Http.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace BSE.Tunes.WebApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{

			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
			// Web API routes
			config.MapHttpAttributeRoutes();
			// Web API configuration and services
			// Configure the custom exceptions.
			config.Filters.Add(new BSE.Tunes.WebApi.Filters.ExceptionFilter());
			config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());

			config.Routes.MapHttpRoute(
				name: "FilesApi",
				routeTemplate: "api/files/{action}/{id}",
				defaults: new
				{
					id = RouteParameter.Optional
				}
			);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
