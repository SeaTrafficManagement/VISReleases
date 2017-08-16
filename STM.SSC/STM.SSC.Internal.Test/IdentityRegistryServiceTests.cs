using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using STM.Common;
using System.IO;
using System.Configuration;

namespace STM.SSC.Internal.Test
{
    [TestClass]
    public class IdentityRegistryServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            using (var stream = Assembly.GetAssembly(typeof(IdentityRegistryServiceTests)).GetManifestResourceStream("STM.SSC.Internal.Test.certificate.p12"))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                var cert = new X509Certificate2(bytes, "StmVis123");
                InstanceContext.ClientCertificate = cert;
            }
        }

        [TestMethod]
        public void FindIdentitiesTest()
        {
            var sscService = new SccPrivateService();
            var result = sscService.FindIdentities();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ValidateCertificate()
        {
            var idRegService = new IdentityRegistryService();
            string certPath = ConfigurationManager.AppSettings["TestCertPath"];
            byte[] certBytes = File.ReadAllBytes(certPath);
            var cert = new X509Certificate2(certBytes, "StmVis123");

            Assert.IsTrue(idRegService.IsCertificateValid(cert));
        }
    }
}