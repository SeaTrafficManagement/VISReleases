using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace STM.Common.DataAccess.Entities
{

    public class VisInstanceSettings
    {
        public long ID { get; set; }

        public string ServiceName { get; set; }

        public string ServiceId { get; set; }

        public string StmModuleUrl { get; set; }

        public string Password { get; set; }

        public byte[] ClientCertificate { get; set; }

        public string ApiKey { get; set; }

        public string ApplicationId { get; set; }

        public bool UseHMACAuthentication { get; set; }

    }
}