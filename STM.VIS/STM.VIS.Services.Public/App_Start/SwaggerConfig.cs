using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;
using STM.VIS.Services.Public;
using System.IO;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace STM.VIS.Services.Public
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
                    c.SingleApiVersion("2_2_0", "STM Voyage Information Service SeaSWIM API")
                        .Description("2.2.0");
                    c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/STM.VIS.Services.Public.xml"));
                    c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/STM.Common.Services.Internal.xml"));
                    c.DescribeAllEnumsAsStrings();
                    c.OperationFilter<Common.Services.RequestContentTypeOperationFilter>();
                    c.OperationFilter<Common.Services.ResponseContentTypeOperationFilter>();
                })
                        .EnableSwaggerUi(c =>
                        {
                        });
        }

        private static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}\XmlComments.xml", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}