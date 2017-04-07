using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Practices.Unity;
using STM.Common.DataAccess;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Threading;
using System.Web.Http.Routing;
using System.Data.Entity;
using STM.Common;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Configuration;
using STM.Common.Services.Internal.Interfaces;

namespace STM.Common.Services
{
    public class LoggingControllerBase : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Dependency]
        protected ILogEventService _logEventService { get; set; }

        [Dependency]
        protected LogDbContext _logContext { get; set; }

        [Dependency]
        protected StmDbContext _context { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        protected string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        protected string GetCurrentComponent()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().DeclaringType.FullName;
        }

        protected HttpResponseException CreateHttpResponseException(HttpStatusCode statusCode, string message)
        {
            var errorMsg = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(message)
            };
            return new HttpResponseException(errorMsg);
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            string instance = null;
            var route = (IHttpRouteData[])controllerContext.RouteData.Values["MS_SubRoutes"];
            if (route != null && route.Count() > 0)
            {
                instance = (string)route[0].Values["instance"];
            }

            log4net.GlobalContext.Properties["Instance"] = instance;

            try
            {
                _context.init(instance);
                _logContext.init(instance);
            }
            catch (Exception ex)
            {
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, ex.Message);
            }

            // Set client settings
            var serviceType = ConfigurationManager.AppSettings["ServiceType"];

            if (serviceType == "VIS")
            {
                log4net.GlobalContext.Properties["ServiceType"] = "VIS";

                var settings = _context.VisInstanceSettings.FirstOrDefault();
                if (settings == null)
                {
                    log.Error("No instance settings found");

                    string msg = "VIS internal server error. No instance settings found";
                    throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
                }

                InstanceContext.Password = settings.Password;
                InstanceContext.ServiceId = settings.ServiceId;
                InstanceContext.ServiceName = settings.ServiceName;
                InstanceContext.StmModuleUrl = settings.StmModuleUrl;
                InstanceContext.Instance = instance;
                InstanceContext.ApiKey = settings.ApiKey;
                InstanceContext.ApplicationId = settings.ApplicationId;
                InstanceContext.UseHMACAuthentication = settings.UseHMACAuthentication;

                var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                var cert = new X509Certificate2(settings.ClientCertificate, Encryption.DecryptString(settings.Password, encryptionPassword));
                InstanceContext.ClientCertificate = cert;
            }
            else if (serviceType == "SPIS")
            {
                log4net.GlobalContext.Properties["ServiceType"] = "SPIS";

                var settings = _context.SpisInstanceSettings.FirstOrDefault();
                if (settings == null)
                {
                    log.Error("No instance settings found");

                    string msg = "SPIS internal server error. No instance settings found";
                    throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
                }

                InstanceContext.Password = settings.Password;
                InstanceContext.ServiceId = settings.ServiceId;
                InstanceContext.ServiceName = settings.ServiceName;
                InstanceContext.StmModuleUrl = settings.StmModuleUrl;
                InstanceContext.Instance = instance;
                InstanceContext.IMO = settings.IMO;
                InstanceContext.MMSI = settings.MMSI;
                InstanceContext.ApiKey = settings.ApiKey;
                InstanceContext.ApplicationId = settings.ApplicationId;
                InstanceContext.UseHMACAuthentication = settings.UseHMACAuthentication;

                var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                var cert = new X509Certificate2(settings.ClientCertificate, Encryption.DecryptString(settings.Password, encryptionPassword));
                InstanceContext.ClientCertificate = cert;
            }
            else
            {
                log.Error("Service type not configured");

                string msg = "Internal server error. Service type not configured";
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }

            return base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}