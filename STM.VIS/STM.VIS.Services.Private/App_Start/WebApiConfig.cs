using STM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Web.Http.Dispatcher;
using STM.Common.Services;

namespace STM.VIS.Services.Private
{
    static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            log4net.Config.XmlConfigurator.Configure();

            // Web API routes
            config.MapHttpAttributeRoutes(new CustomDirectRouteProvider());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Insert(0, new TextMediaTypeFormatter());

            // Setup exception handling filter
            WebApiExceptionHandler.SetupExceptionHandlers();
        }
    }
}