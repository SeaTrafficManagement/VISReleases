using Newtonsoft.Json;
using STM.Common;
using STM.Common.Certificates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace STM.SSC.Internal
{
    public class IdentityRegistryService : IIdentityRegistryService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string idRegistryBaseUrl;
        private readonly string rootCertificateTumbprint;

        private const string IDREG_PATH_ORG_IDENTITIES = "/org/%s/services";


        public IdentityRegistryService()
        {
            idRegistryBaseUrl = ConfigurationManager.AppSettings.Get("IdREgistryBaseUrl");
            rootCertificateTumbprint = ConfigurationManager.AppSettings.Get("RootCertificateTumbprint");
        }

        public bool IsCertificateValid(X509Certificate2 cert)
        {
            var errors = string.Empty;
            return CertificateValidator.IsCertificateValid(cert, rootCertificateTumbprint, out errors);
        }

        public WebRequestHelper.WebResponse MakeGenericCall(string url, string method, string body = null, WebHeaderCollection headers = null)
        {
            log.Info("Make generic call to IdentityRegistry");

            WebRequestHelper.WebResponse response = null;

            url = idRegistryBaseUrl + url;
            if (method == "GET")
                response = WebRequestHelper.Get(url, headers, true);
            else if (method == "POST") 
                response = WebRequestHelper.Post(url, body, headers: headers, UseCertificate: true);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new WebException(response.ErrorMessage);
            }

            return response; 
        }
    }
}
