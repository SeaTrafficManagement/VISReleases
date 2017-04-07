using STM.StmModule.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Web.Http;
using System.Web.Http.SelfHost;
using STM.StmModule.Simulator.Services;
using STM.Common;

namespace STM.StmModule.Simulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private HttpSelfHostServer _server;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1 || e.Args.Length == 3)
            {
                VisService.DbName = e.Args[0];
                SpisService.DbName = e.Args[0];
            }

            if (e.Args.Length == 3)
            {
                WebRequestHelper.UseHMACAuthentication = true;
                WebRequestHelper.APPId = e.Args[1];
                WebRequestHelper.APIKey = e.Args[2];
            }

            if (e.Args.Length != 1 && e.Args.Length != 3)
            {
                MessageBox.Show("Invalid startup arguments. Supply instancename, application id and API key");
                Application.Current.Shutdown();
            }

            var wnd = new MainWindow();
            wnd.Show();
        }
        public App()
        {
            //var port = int.Parse(ConfigurationManager.AppSettings.Get("LocalPort"));
            //var config = new HttpSelfHostConfiguration("http://localhost:" + port);

            //config.Routes.MapHttpRoute(
            //    "API Default", "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });

            //_server = new HttpSelfHostServer(config);
            //_server.OpenAsync().Wait();
        }
    }
}
