using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;
using STM.VIS.Services.Private;
using System.Web;
using System.IO;
using System;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace STM.VIS.Services.Private
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
                        c.SingleApiVersion("2_2_0", "STM Voyage Information Service STM Onboard API")
                            .Description("2.2.0");
                        c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/STM.VIS.Services.Private.xml"));
                        c.DescribeAllEnumsAsStrings();
                        c.OperationFilter<Common.Services.RequestContentTypeOperationFilter>();
                        c.OperationFilter<Common.Services.ResponseContentTypeOperationFilter>();
                    }).EnableSwaggerUi(c =>
                        {
                        });
        }
    }
}