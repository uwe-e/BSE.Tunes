using BSE.Tunes.WebApi.Web.Http.Tracing;
using Microsoft.Web.Http.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;

namespace BSE.Tunes.WebApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
            var constraintResolver = new DefaultInlineConstraintResolver()
            {
                ConstraintMap = { ["apiVersion"] = typeof(ApiVersionRouteConstraint) }
            };
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            //config.AddApiVersioning(o => o.ReportApiVersions = true);
            //Enable Backward Compatibility
            config.AddApiVersioning(o => o.AssumeDefaultVersionWhenUnspecified = true);
            // Web API routes
            //config.MapHttpAttributeRoutes();
            config.MapHttpAttributeRoutes(constraintResolver);
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
            //map the versioned route
            config.Routes.MapHttpRoute(
                "VersionedUrl",
                "api/v{apiVersion}/{controller}/{action}/{id}",
                defaults: null
                //constraints: new
                //{
                //    apiVersion = new ApiVersionRouteConstraint()
                //});
                );

        }
    }
}
