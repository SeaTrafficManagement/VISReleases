using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using STM.Common;

namespace STM.SSC.Internal.Test
{
    [TestClass]
    public class ServiceRegistryServiceTests
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
        public void FindServicesTest()
        {
            var sscService = new SSC.Internal.SccPrivateService();
            var req = new Models.FindServicesRequestObj();
            req.Filter = new Models.FindServicesRequestObjFilter
            {
                KeyWords = new System.Collections.Generic.List<string>
                {
                    "VIS"
                }
            };

            var result = sscService.FindServices(req);
            Assert.IsNotNull(result);
        }
    }
}
