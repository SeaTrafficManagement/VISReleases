﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace STM.SPIS.Services.Private
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            log.Info(string.Format("SPIS Private is starting up at: {0} UTC", DateTime.UtcNow));

            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_End()
        {
            log.Info(string.Format("SPIS Private is stopping at: {0} UTC", DateTime.UtcNow));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
