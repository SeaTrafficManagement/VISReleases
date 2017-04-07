using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;
using STM.SPIS.Services.Public;
using System;
using System.IO;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace STM.SPIS.Services.Public
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
                    c.SingleApiVersion("SPIS_v0_1_0", "STM Ship Port Information Service SeaSWIM API");
                    c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/STM.SPIS.Services.Public.xml"));
                    c.DescribeAllEnumsAsStrings();
                })
                        .EnableSwaggerUi(c =>
                        {
                        });
        }
    }
}