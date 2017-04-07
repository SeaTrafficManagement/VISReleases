using Newtonsoft.Json;
using STM.Common;
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
        private readonly string idRegistryOpenIdBaseUrl;
        private readonly string rootCertificateTumbprint;

        private const string IDREG_PATH_OID_TOKEN = "/auth/realms/MaritimeCloud/protocol/openid-connect/token?";
        private const string IDREG_PATH_OID_AUTH = "/auth/realms/MaritimeCloud/protocol/openid-connect/auth?";
        private const string IDREG_PATH_OID = "/oidc/api";
        private const string IDREG_PATH_CRL = "/certificates/crl";
        private const string IDREG_PATH_ORG_IDENTITIES = "/org/%s/services";

        private string redirectURI = "http://localhost:9991/api/openid/auth";
        private string refreshToken;

        public IdentityRegistryService()
        {
            idRegistryBaseUrl = ConfigurationManager.AppSettings.Get("IdREgistryBaseUrl");
            idRegistryOpenIdBaseUrl = ConfigurationManager.AppSettings.Get("IdRegistryOpenIdBaseUrl");
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

            var token = GetAccessToken();
            if (headers == null)
                headers = new WebHeaderCollection();

            headers.Add("Authorization", "Bearer " + token);

            url = idRegistryBaseUrl + IDREG_PATH_OID + url;
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

        public string GetAccessToken()
        {
            Token token = null;
            if ((this.refreshToken != null))
            {
                token = GetAccessTokenUsingRefreshToken();
            }

            if ((token == null))
            {
                var authCode = GetOpenIdAuthCode();
                token = GetNewAccessToken(authCode);
            }

            refreshToken = token.RefreshToken;
            return token.AccessToken;
        }

        private string GetOpenIdAuthCode()
        {
            try
            {
                var url = new StringBuilder()
                    .Append(idRegistryOpenIdBaseUrl + IDREG_PATH_OID_AUTH)
                    .Append("client_id=cert2oidc")
                    .Append("&")
                    .Append("redirect_uri=")
                    .Append(HttpUtility.UrlEncode(redirectURI))
                    .Append("&")
                    .Append("response_type=code")
                    .Append("&")
                    .Append("kc_idp_hint=certificates")
                    .Append("&")
                    .Append("scope=openid").ToString();

                var response = WebRequestHelper.Get(url, UseCertificate: true);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new WebException(response.ErrorMessage);
                }

                string authCode = response.Body.Trim('"');
                if (string.IsNullOrEmpty(authCode))
                {
                    throw new Exception("Unable to get authCode from IdentityRegistry");
                }

                return authCode;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        private Token GetNewAccessToken(String authCode)
        {
            try
            {
                var url = idRegistryOpenIdBaseUrl + IDREG_PATH_OID_TOKEN;
                var data = new StringBuilder()
                    .Append("grant_type=authorization_code")
                    .Append("&")
                    .Append("client_id=cert2oidc")
                    .Append("&")
                    .Append("code=")
                    .Append(authCode)
                    .Append("&")
                    .Append("redirect_uri=")
                    .Append(redirectURI).ToString();

                WebHeaderCollection headers = new WebHeaderCollection();
                headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                var response = WebRequestHelper.Post(url, data, headers);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new WebException(response.ErrorMessage);
                }

                if (string.IsNullOrEmpty(response.Body))
                {
                    throw new Exception("Unable to get token from IdentityRegistry");
                }

                Token token = JsonConvert.DeserializeObject<Token>(response.Body);

                return token;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        private Token GetAccessTokenUsingRefreshToken()
        {
            try
            {
                var url = idRegistryOpenIdBaseUrl + IDREG_PATH_OID_TOKEN;
                var data = new StringBuilder()
                    .Append("grant_type=refresh_token")
                    .Append("&")
                    .Append("client_id=cert2oidc")
                    .Append("&")
                    .Append("refresh_token=")
                    .Append(refreshToken).ToString();

                WebHeaderCollection headers = new WebHeaderCollection();
                headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                var response = WebRequestHelper.Post(url, data, headers);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new WebException(response.ErrorMessage);
                }

                if (string.IsNullOrEmpty(response.Body))
                {
                    throw new Exception("Unable to get token from IdentityRegistry");
                }

                Token token = JsonConvert.DeserializeObject<Token>(response.Body);
                return token;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                refreshToken = null;
                return null;
            }
        }
    }
}
