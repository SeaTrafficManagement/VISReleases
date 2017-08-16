using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace STM.Common
{
    public class InstanceContext
    {
        private static Dictionary<string, object> settings = new Dictionary<string, object>();

        public static string ServiceName
        {
            get { return (string)GetValue("ServiceName"); }
            set { SetValue("ServiceName", value); }
        }

        public static string NotImplementetOperations
        {
            get { return (string)GetValue("NotImplementetOperations"); }
            set { SetValue("NotImplementetOperations", value); }
        }

        public static string ServiceId
        {
            get { return (string)GetValue("ServiceId"); }
            set { SetValue("ServiceId", value); }
        }

        public static string Instance
        {
            get { return (string)GetValue("Instance"); }
            set { SetValue("Instance", value); }
        }

        public static string StmModuleUrl
        {
            get { return (string)GetValue("StmModuleUrl"); }
            set { SetValue("StmModuleUrl", value); }
        }

        public static string Password
        {
            get { return (string)GetValue("Password"); }
            set { SetValue("Password", value); }
        }

        public static string CallerServiceId
        {
            get { return (string)GetValue("CallerServiceId"); }
            set { SetValue("CallerServiceId", value); }
        }

        public static string CallerOrgId
        {
            get { return (string)GetValue("CallerOrgId"); }
            set { SetValue("CallerOrgId", value); }
        }

        public static string IMO
        {
            get { return (string)GetValue("IMO"); }
            set { SetValue("IMO", value); }
        }

        public static string MMSI
        {
            get { return (string)GetValue("MMSI"); }
            set { SetValue("MMSI", value); }
        }

        public static X509Certificate2 ClientCertificate
        {
            get { return (X509Certificate2)GetValue("ClientCertificate"); }
            set { SetValue("ClientCertificate", value); }
        }

        public static string ApiKey
        {
            get { return (string)GetValue("ApiKey"); }
            set { SetValue("ApiKey", value); }
        }

        public static string ApplicationId
        {
            get { return (string)GetValue("ApplicationId"); }
            set { SetValue("ApplicationId", value); }
        }

        public static bool UseHMACAuthentication
        {
            get { return (bool)GetValue("UseHMACAuthentication"); }
            set { SetValue("UseHMACAuthentication", value); }
        }

        private static object GetValue(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                return HttpContext.Current.Session[key];
            }
            else
            {
                return settings[key];
            }
        }

        private static void SetValue(string key, object value)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
            else
            {
                if (settings.ContainsKey(key))
                    settings[key] = value;
                else
                    settings.Add(key, value);
            }
        }
    }
}