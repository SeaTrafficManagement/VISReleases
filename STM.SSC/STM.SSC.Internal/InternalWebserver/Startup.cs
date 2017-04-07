using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace STM.SSC.Internal.InternalWebserver
{
    public class Startup
    {
        private static HttpSelfHostServer _server;

        public static void StartWebservice()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:9991");

            config.Routes.MapHttpRoute(
                    "API Default", "api/{controller}/{id}",
                    new
                    {
                        id = RouteParameter.Optional
                    });

            _server = new HttpSelfHostServer(config);

            try
            {
                _server.OpenAsync().Wait();
            }
            catch (System.AggregateException)
            {
                // Ignore
            }
        }

        public static bool IsStarted
        {
            get
            {
                return _server != null;
            }
        }
    }
}