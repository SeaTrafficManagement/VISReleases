using Microsoft.VisualStudio.TestTools.UnitTesting;
using STM.Common;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Test
{
    [TestClass]
    public sealed class CreateInstanceSettings : IDisposable
    {
        private StmDbContext _dbContext;

        [TestInitialize]
        public void Init()
        {
            _dbContext = new StmDbContext();
        }

        [Ignore]
        [TestMethod]
        public void SetupVisInstanceSettings()
        {
            var instanceDbName = "stm";
            var instanceName = "stm";
            var instanceId = "urn:mrn:stm:service:instance:sma:stm";
            var stmModuleUrl = "";
            var password = "StmVis123";
            var encruptionPassphrase = "A_vErry secret paSsw0rd#that is not easy to Re2MeMbeR!";
            var encryptedPassword = Encryption.EncryptString(password, encruptionPassphrase);

            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Certifikat | *.p12";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _dbContext.init(instanceDbName);
                byte[] certBytes = File.ReadAllBytes(dlg.FileName);

                var settings = _dbContext.VisInstanceSettings.FirstOrDefault();
                if (settings == null)
                {
                    settings = new VisInstanceSettings();
                    settings.ServiceName = instanceName;
                    settings.ServiceId = instanceId;
                    settings.StmModuleUrl = stmModuleUrl;
                    settings.ClientCertificate = certBytes;
                    settings.Password = encryptedPassword;

                    _dbContext.VisInstanceSettings.Add(settings);
                }
                else
                {
                    settings.ServiceName = instanceName;
                    settings.ServiceId = instanceId;
                    settings.StmModuleUrl = stmModuleUrl;
                    settings.ClientCertificate = certBytes;
                    settings.Password = encryptedPassword;
                }

                _dbContext.SaveChanges();
            }
        }

        [Ignore]
        [TestMethod]
        public void SetupSpisInstanceSettings()
        {
            var instanceDbName = "SMA002";
            var instanceName = "SMA002";
            var instanceId = "urn:mrn:stm:serviceinstance:sma:SMA002";
            var stmModuleUrl = "";
            var password = "StmVis123";
            var encruptionPassphrase = "A_vErry secret paSsw0rd#that is not easy to Re2MeMbeR!";
            var encryptedPassword = Encryption.EncryptString(password, encruptionPassphrase);

            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Certifikat | *.p12";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _dbContext.init(instanceDbName);
                byte[] certBytes = File.ReadAllBytes(dlg.FileName);

                var settings = _dbContext.SpisInstanceSettings.FirstOrDefault();
                if (settings == null)
                {
                    settings = new SpisInstanceSettings();
                    settings.ServiceName = instanceName;
                    settings.ServiceId = instanceId;
                    settings.StmModuleUrl = stmModuleUrl;
                    settings.ClientCertificate = certBytes;
                    settings.Password = encryptedPassword;

                    _dbContext.SpisInstanceSettings.Add(settings);
                }
                else
                {
                    settings.ServiceName = instanceName;
                    settings.ServiceId = instanceId;
                    settings.StmModuleUrl = stmModuleUrl;
                    settings.ClientCertificate = certBytes;
                    settings.Password = encryptedPassword;
                }

                _dbContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}