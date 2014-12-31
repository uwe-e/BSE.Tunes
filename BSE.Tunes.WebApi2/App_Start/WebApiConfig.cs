using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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
			config.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new BSE.Tunes.WebApi.Web.Http.TraceExceptionLogger());
			
			config.Routes.MapHttpRoute(
                name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
