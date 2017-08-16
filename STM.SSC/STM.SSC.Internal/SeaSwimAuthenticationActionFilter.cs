using STM.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace STM.SSC.Internal
{
    public class SeaSwimAuthenticationActionFilter : ActionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var serviceId = string.Empty;
            var orgId = string.Empty;

            try
            {
                if (actionContext.ActionDescriptor.ActionName == "ping")
                {
                    serviceId = "Ping";
                    orgId = "Ping";
                }
                else if (bool.Parse(ConfigurationManager.AppSettings.Get("BypassClientCertificateValidation")) == true)
                {
                    log.Warn("BypassClientCertificateValidation is set to true");
                    serviceId = ConfigurationManager.AppSettings.Get("IncomingServiceId");
                    orgId = ConfigurationManager.AppSettings.Get("IncomingOrganizationId");
                }
                else
                {
                    // Validate client certificate
                    var cert = actionContext.Request.GetClientCertificate();
                    var valid = new IdentityRegistryService().IsCertificateValid(cert);
                    if (!valid)
                    {
                        throw new AuthenticationException("Provided client certificate is not valid");
                    }

                    // Extract data from certifcate
                    var certData = cert.Subject.Split(',');

                    var certDataDictionary = new Dictionary<string, string>();
                    foreach (var item in certData)
                    {
                        var parts = item.Split('=');
                        if (parts != null && parts.Count() == 2)
                        {
                            if (parts[0].Trim().StartsWith("OID"))
                            {
                                serviceId = parts[1]; ;
                            }
                            if (parts[0].Trim() == "O")
                            {
                                orgId = parts[1]; ;
                            }
                            certDataDictionary.Add(parts[0].Trim(), parts[1].Trim());
                        }
                    }
                }

                InstanceContext.CallerOrgId = orgId;
                InstanceContext.CallerServiceId = serviceId;
            }
            catch (AuthenticationException aex)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    ReasonPhrase = aex.Message,
                    StatusCode = HttpStatusCode.Unauthorized
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }

            log.Info(string.Format("Authenticated call from service id: {0}, org: {1} to url: {2}", serviceId, orgId, actionContext.Request.RequestUri));
        }
    }
}