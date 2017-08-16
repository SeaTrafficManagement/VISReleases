using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.Certificates
{
    public class CertificateValidator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsCertificateValid(X509Certificate2 cert, string rootCertificateTumbprint, out string errors)
        {
            errors = string.Empty;

            log.Debug("Checking certificate");

            X509Chain chain = new X509Chain();
            chain.ChainPolicy = new X509ChainPolicy()
            {
                RevocationFlag = X509RevocationFlag.EndCertificateOnly,
                RevocationMode = X509RevocationMode.Online,
                UrlRetrievalTimeout = new TimeSpan(0, 0, 10),
                VerificationTime = DateTime.UtcNow
            };

            var chainBuilt = chain.Build(cert);
            log.Info(string.Format("Chain building status: {0}", chainBuilt));

            if (chainBuilt)
            {
                if (chain.ChainElements
                    .Cast<X509ChainElement>()
                    .Any(x => x.Certificate.Thumbprint.ToUpper().Trim().CompareTo(rootCertificateTumbprint.ToUpper()) == 0))
                {
                    log.Info("Certificate is valid");
                    return true;
                }
                else
                {
                    log.Info("Invalid certificate, invalid issuer thumbprint");
                    return false;
                }
            }
            else
            {
                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                {
                    errors += string.Format("Chain error: {0} {1}", chainStatus.Status, chainStatus.StatusInformation);
                }

                log.Info(errors);
                return false;
            }
        }
    }
}