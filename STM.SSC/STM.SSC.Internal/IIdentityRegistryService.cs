using STM.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace STM.SSC.Internal
{
    public interface IIdentityRegistryService
    {
        bool IsCertificateValid(X509Certificate2 cert);
        WebRequestHelper.WebResponse MakeGenericCall(string url, string method, string body = null, WebHeaderCollection headers = null);
    }
}