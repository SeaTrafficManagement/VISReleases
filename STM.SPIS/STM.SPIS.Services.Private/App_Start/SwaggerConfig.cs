using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;
using STM.SPIS.Services.Private;
using System.IO;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace STM.SPIS.Services.Private
{
    class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.Schemes(new[] { "http", "https" });
                    c.SingleApiVersion("SPIS_v1_0_0", "STM Ship Port Information Service STM Onboard API");
                    c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/STM.SPIS.Services.Private.xml"));
                    c.DescribeAllEnumsAsStrings();
                    c.OperationFilter<Common.Services.RequestContentTypeOperationFilter>();
                    c.OperationFilter<Common.Services.ResponseContentTypeOperationFilter>();
                }).EnableSwaggerUi(c =>
                {
                });
        }

        private static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}\XmlComments.xml", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}