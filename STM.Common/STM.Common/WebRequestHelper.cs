using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STM.Common
{
    public class WebRequestHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool UseHMACAuthentication { get; set; }
        public static string APIKey { get; set; }
        public static string APPId { get; set; }

        public static WebResponse Get(string url, WebHeaderCollection headers = null, bool UseCertificate = false)
        {
            var result = ExecuteWebRequest(url, null, "GET", headers, UseCertificate);
            return result;
        }

        public static WebResponse Post(string url, string body, WebHeaderCollection headers = null, bool UseCertificate = false)
        {
            var result = ExecuteWebRequest(url, body, "POST", headers, UseCertificate);
            return result;
        }

        public static WebResponse Put(string url, string body, WebHeaderCollection headers = null, bool UseCertificate = false)
        {
            var result = ExecuteWebRequest(url, body, "PUT", headers, UseCertificate);
            return result;
        }

        public static WebResponse Delete(string url, string body, WebHeaderCollection headers = null, bool UseCertificate = false)
        {
            var result = ExecuteWebRequest(url, body, "DELETE", headers, UseCertificate);
            return result;
        }

        private static WebResponse ExecuteWebRequest(string url, string body, string method, WebHeaderCollection headers = null, bool UseCertificate = false)
        {
            log.Debug(string.Format("ExecuteWebRequest url: {0}, body: {1}, method: {2}, UseCertificate: {3}", url, body, method, UseCertificate.ToString()));

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.PreAuthenticate = true;

            if (headers != null)
            {
                foreach (string key in headers.AllKeys)
                {
                    if (key.ToLower() == "content-type")
                    {
                        request.ContentType = headers[key];
                        headers.Remove(key);
                    }
                }

                if (request.Headers == null)
                    request.Headers = headers;
                else
                {
                    foreach (string key in headers.AllKeys)
                        request.Headers.Add(key, headers[key]);
                }
            }

            if (UseCertificate)
            {
                if (InstanceContext.ClientCertificate == null)
                    throw new Exception("Client certificate has not been set");

                request.ClientCertificates.Add(InstanceContext.ClientCertificate);
            }

            if (UseHMACAuthentication)
            {
                // Add HMAC authentication hader to the request
                request = AddHAMCAuthentication(request, body);
            }

            if (method == "POST" || method == "DELETE")
            {
                var byteArray = Encoding.UTF8.GetBytes(body ?? string.Empty);
                request.ContentLength = byteArray.Length;
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            var result = new WebResponse();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                result.HttpStatusCode = response.StatusCode;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result.Body = sr.ReadToEnd();
                }

                log.Debug("Response: " + response.StatusCode + ", " + result.Body);
            }
            catch (WebException wex)
            {
                log.Error(wex.Message, wex);
                if (wex.Response != null)
                {
                    using (var sr = new StreamReader(wex.Response.GetResponseStream()))
                    {
                        while (sr.Peek() > -1)
                        {
                            result.Body = sr.ReadToEnd();
                        }
                    }
                    result.HttpStatusCode = ((HttpWebResponse)wex.Response).StatusCode;
                    log.Error(result.HttpStatusCode + " " + result.Body);
                }
                else
                {
                    result.HttpStatusCode = HttpStatusCode.InternalServerError;
                }

                result.ErrorMessage = wex.Message;
            }

            return result;
        }

        private static HttpWebRequest AddHAMCAuthentication(HttpWebRequest request, string body)
        {
            string requestContentBase64String = string.Empty;

            string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            string requestHttpMethod = request.Method;

            // Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            byte[] content = null;
            if (body != null)
                content = Encoding.UTF8.GetBytes(body);

            if (content != null)
            {
                MD5 md5 = MD5.Create();
                //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            // Creating the raw signature string
            string signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Encoding.UTF8.GetBytes(APIKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                //Setting the values in the Authorization header using custom scheme (amx)
                request.Headers.Add("Authorization", string.Format("amx {0}:{1}:{2}:{3}", APPId, requestSignatureBase64String, nonce, requestTimeStamp));
            }

            return request;
        }

        public static string CombineUrl (string baseUrl, string stringToAdd)
        {
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            var uri = new Uri(new Uri(baseUrl), stringToAdd);
            return uri.ToString();
        }

        public class WebResponse
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string ErrorMessage { get; set; }
            public string Body { get; set; }
        }
    }
}
