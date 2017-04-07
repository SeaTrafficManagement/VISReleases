using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common
{
    public class InstanceContext
    {
        public static string ServiceName { get; set; }
        public static string ServiceId { get; set; }
        public static string Instance { get; set; }
        public static string StmModuleUrl { get; set; }
        public static string Password { get; set; }
        public static string CallerServiceId { get; set; }
        public static string CallerOrgId { get; set; }
        public static string IMO { get; set; }
        public static string MMSI { get; set; }
        public static X509Certificate2 ClientCertificate { get; set; }
        public static string ApiKey { get; set; }
        public static string ApplicationId { get; set; }
        public static bool UseHMACAuthentication { get; set; }
    }
}
