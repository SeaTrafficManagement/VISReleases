using STM.Common;
using STM.Common.Services;
using STM.SSC.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace STM.SPIS.Services.Public
{
    static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            log4net.Config.XmlConfigurator.Configure();

            config.Filters.Add(new SeaSwimAuthenticationActionFilter());

            // Web API routes
            config.MapHttpAttributeRoutes(new CustomDirectRouteProvider());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Insert(0, new TextMediaTypeFormatter());

            // Setup exception handling filter
            WebApiExceptionHandler.SetupExceptionHandlers();
        }
    }
}