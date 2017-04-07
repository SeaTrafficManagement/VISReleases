using Microsoft.Win32;
using STM.Common;
using Entities = STM.Common.DataAccess.Entities;
using STM.Tools.InstanceConfigurator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.IO.Compression;
using System.Diagnostics;
using STM.Tools.InstanceConfigurator.Views;
using System.Security.Cryptography;

namespace STM.Tools.InstanceConfigurator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string RootCertificateTumbprint;

        public MainViewModel()
        {
            RootCertificateTumbprint = ConfigurationManager.AppSettings["RootCertificateTumbprint"];

            Task.Factory.StartNew(() =>
            {
                LoadInstances();
            });
        }

        private ObservableCollection<string> _instances;
        public ObservableCollection<string> Instances
        {
            get
            {
                return _instances;
            }
            set
            {
                if (_instances == value)
                    return;

                _instances = value;

                OnPropertyChanged(() => Instances);
            }
        }

        private string _selectedInstance;
        public string SelectedInstance
        {
            get
            {
                return _selectedInstance;
            }
            set
            {
                if (_selectedInstance == value)
                    return;

                _selectedInstance = value;

                VisCertificate = null;
                VisCertificatePassword = null;
                SpisCertificate = null;
                SpisCertificatePassword = null;

                if (SelectedInstance != null)
                {
                    VisInstanceSettings = InstanceService.GetVisInstanceSettings(SelectedInstance);
                    SpisInstanceSettings = InstanceService.GetSpisInstanceSettings(SelectedInstance);
                }

                if (VisInstanceSettings == null)
                    VisInstanceSettings = new Entities.VisInstanceSettings();

                if (SpisInstanceSettings == null)
                    SpisInstanceSettings = new Entities.SpisInstanceSettings();

                OnPropertyChanged(() => SelectedInstance);
                OnPropertyChanged(() => HasSelectedInstance);

                DeleteInstanceCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasSelectedInstance
        {
            get
            {
                return _selectedInstance != null;
            }
        }

        public bool VisCertificateIsValid
        {
            get
            {
                if (VisCertificate != null)
                {
                    var errors = string.Empty;
                    var result = CertificateValidator.IsCertificateValid(VisCertificate, RootCertificateTumbprint, out errors);
                    VisCertificateErrors = errors;
                    return result;
                }

                return true;
            }
        }

        private string _visCertificateErrors;
        public string VisCertificateErrors
        {
            get
            {
                return _visCertificateErrors;
            }
            set
            {
                if (_visCertificateErrors == value)
                    return;

                _visCertificateErrors = value;

                OnPropertyChanged(() => VisCertificateErrors);
            }
        }

        public bool SpisCertificateIsValid
        {
            get
            {
                if (SpisCertificate != null)
                {
                    var errors = string.Empty;
                    var result = CertificateValidator.IsCertificateValid(SpisCertificate, RootCertificateTumbprint, out errors);
                    SpisCertificateErrors = errors;
                    return result;
                }

                return true;
            }
        }

        private string _spisCertificateErrors;
        public string SpisCertificateErrors
        {
            get
            {
                return _spisCertificateErrors;
            }
            set
            {
                if (_spisCertificateErrors == value)
                    return;

                _spisCertificateErrors = value;

                OnPropertyChanged(() => SpisCertificateErrors);
            }
        }

        private Entities.VisInstanceSettings _visInstanceSettings;
        public Entities.VisInstanceSettings VisInstanceSettings
        {
            get
            {
                return _visInstanceSettings;
            }
            set
            {
                if (_visInstanceSettings == value)
                    return;

                _visInstanceSettings = value;

                if (_visInstanceSettings != null
                    && _visInstanceSettings.ClientCertificate != null)
                {
                    var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                    VisCertificatePassword = Encryption.DecryptString(_visInstanceSettings.Password, encryptionPassword);
                    VisCertificate = new X509Certificate2(_visInstanceSettings.ClientCertificate, _visCertificatePassword, X509KeyStorageFlags.Exportable);
                }

                OnPropertyChanged(() => VisInstanceSettings);
            }
        }

        private Entities.SpisInstanceSettings _spisInstanceSettings;
        public Entities.SpisInstanceSettings SpisInstanceSettings
        {
            get
            {
                return _spisInstanceSettings;
            }
            set
            {
                if (_spisInstanceSettings == value)
                    return;

                _spisInstanceSettings = value;

                if (_spisInstanceSettings != null
                    && _spisInstanceSettings.ClientCertificate != null)
                {
                    var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                    SpisCertificatePassword = Encryption.DecryptString(_spisInstanceSettings.Password, encryptionPassword);
                    SpisCertificate = new X509Certificate2(_spisInstanceSettings.ClientCertificate, SpisCertificatePassword);
                }

                OnPropertyChanged(() => SpisInstanceSettings);
            }
        }

        private X509Certificate2 _spisCertificate;
        public X509Certificate2 SpisCertificate
        {
            get
            {
                return _spisCertificate;
            }
            set
            {
                if (_spisCertificate == value)
                    return;

                _spisCertificate = value;

                OnPropertyChanged(() => SpisCertificate);
                OnPropertyChanged(() => SpisCertificateIsValid);
            }
        }

        private X509Certificate2 _visCertificate;
        public X509Certificate2 VisCertificate
        {
            get
            {
                return _visCertificate;
            }
            set
            {
                if (_visCertificate == value)
                    return;

                _visCertificate = value;

                OnPropertyChanged(() => VisCertificateIsValid);
                OnPropertyChanged(() => VisCertificate);
            }
        }

        private string _visCertificatePassword;
        public string VisCertificatePassword
        {
            get
            {
                return _visCertificatePassword;
            }
            set
            {
                if (_visCertificatePassword == value)
                    return;

                _visCertificatePassword = value;

                OnPropertyChanged(() => VisCertificatePassword);
                ViewCertificateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _spisCertificatePassword;
        public string SpisCertificatePassword
        {
            get
            {
                return _spisCertificatePassword;
            }
            set
            {
                if (_spisCertificatePassword == value)
                    return;

                _spisCertificatePassword = value;

                OnPropertyChanged(() => SpisCertificatePassword);
            }
        }


        #region Commands
        private DelegateCommand _viewCertificateCommand;
        public DelegateCommand ViewCertificateCommand
        {
            get
            {
                return _viewCertificateCommand ??
                    (_viewCertificateCommand = new DelegateCommand(ExecuteViewCertificateCommand, CanExecuteViewCertificateCommand));
            }
        }

        public bool CanExecuteViewCertificateCommand(object parameter)
        {
            return true;
        }

        public void ExecuteViewCertificateCommand(object parameter)
        {
            var cert = parameter as X509Certificate2;
            if (cert != null)
            {
                X509Certificate2UI.DisplayCertificate(cert);
            }
        }

        private DelegateCommand _loadVisCertificateCommand;
        public DelegateCommand LoadVisCertificateCommand
        {
            get
            {
                return _loadVisCertificateCommand ??
                    (_loadVisCertificateCommand = new DelegateCommand(ExecuteLoadVisCertificateCommand, CanExecuteLoadVisCertificateCommand));
            }
        }

        public bool CanExecuteLoadVisCertificateCommand(object parameter)
        {
            return true;
        }

        public void ExecuteLoadVisCertificateCommand(object parameter)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Certifikat | *.zip";
            var tempFolder = Path.GetTempPath();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (ZipArchive archive = ZipFile.OpenRead(dlg.FileName))
                {
                    var certificateFile = string.Empty;
                    var privateKeyFile = string.Empty;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".pem", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(tempFolder, entry.Name), true);
                            if (entry.Name.StartsWith("Certificate_"))
                            {
                                certificateFile = Path.Combine(tempFolder, entry.Name);
                            }
                            else if (entry.Name.StartsWith("PrivateKey_"))
                            {
                                privateKeyFile = Path.Combine(tempFolder, entry.Name);
                            }
                        }
                    }

                    var outputFile = string.Empty;

                    if (!string.IsNullOrEmpty(certificateFile)
                        && !string.IsNullOrEmpty(certificateFile))
                    {
                        var password = CreateRandomPassword(10);
                        var startpos = certificateFile.IndexOf("Certificate_") + 12;
                        var name = certificateFile.Substring(startpos, certificateFile.Length - startpos - 4);
                        outputFile = tempFolder + name + "-certificate.p12";
                        var arg = string.Format("{0} {1} {2} pass:{3} {4} {5}",
                            certificateFile, privateKeyFile, "mc-ca-chain.pem", password, name, outputFile);

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenSSL\\CreateCert.cmd");
                        startInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenSSL");
                        startInfo.Arguments = arg;

                        var p = Process.Start(startInfo);
                        p.WaitForExit();

                        var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                        VisCertificatePassword = password;

                        VisCertificate = new X509Certificate2(outputFile, VisCertificatePassword, X509KeyStorageFlags.Exportable);
                    }

                    if (!string.IsNullOrEmpty(certificateFile))
                        File.Delete(certificateFile);

                    if (!string.IsNullOrEmpty(privateKeyFile))
                        File.Delete(privateKeyFile);

                    if (!string.IsNullOrEmpty(outputFile))
                        File.Delete(outputFile);
                }
            }
        }

        private DelegateCommand _saveVisInstanceSettingsCommand;
        public DelegateCommand SaveVisInstanceSettingsCommand
        {
            get
            {
                return _saveVisInstanceSettingsCommand ??
                    (_saveVisInstanceSettingsCommand = new DelegateCommand(ExecuteSaveVisInstanceSettingsCommand, CanExecuteSaveVisInstanceSettingsCommand));
            }
        }

        public bool CanExecuteSaveVisInstanceSettingsCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteSaveVisInstanceSettingsCommand(object parameter)
        {
            this.Busy = true;
            this.BusyContent = "Saving VIS instanse settings";

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                    var encryptedPassword = Encryption.EncryptString(VisCertificatePassword, encryptionPassword);
                    VisInstanceSettings.Password = encryptedPassword;
                    VisInstanceSettings.ClientCertificate = null;
                    VisInstanceSettings.ClientCertificate = VisCertificate.Export(X509ContentType.Pkcs12, VisCertificatePassword);

                    InstanceService.SaveVisInstanceSettings(SelectedInstance, VisInstanceSettings);
                });

                MessageBox.Show("VIS instance settings has been successfully saved.", "VIS instance settingse", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when saving VIS instance settings " + Environment.NewLine + ex.Message, "VIS instance settingse", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Busy = false;
            }
        }

        private DelegateCommand _cancelVisInstanceSettingsCommand;
        public DelegateCommand CancelVisInstanceSettingsCommand
        {
            get
            {
                return _cancelVisInstanceSettingsCommand ??
                    (_cancelVisInstanceSettingsCommand = new DelegateCommand(ExecuteCancelVisInstanceSettingsCommand, CanExecuteCancelVisInstanceSettingsCommand));
            }
        }

        public bool CanExecuteCancelVisInstanceSettingsCommand(object parameter)
        {
            return true;
        }

        public void ExecuteCancelVisInstanceSettingsCommand(object parameter)
        {
            VisInstanceSettings = InstanceService.GetVisInstanceSettings(SelectedInstance);
        }


        private DelegateCommand _loadSpisCertificateCommand;
        public DelegateCommand LoadSpisCertificateCommand
        {
            get
            {
                return _loadSpisCertificateCommand ??
                    (_loadSpisCertificateCommand = new DelegateCommand(ExecuteLoadSpisCertificateCommand, CanExecuteLoadSpisCertificateCommand));
            }
        }

        public bool CanExecuteLoadSpisCertificateCommand(object parameter)
        {
            return true;
        }

        public void ExecuteLoadSpisCertificateCommand(object parameter)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Certifikat | *.zip";
            var tempFolder = Path.GetTempPath();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (ZipArchive archive = ZipFile.OpenRead(dlg.FileName))
                {
                    var certificateFile = string.Empty;
                    var privateKeyFile = string.Empty;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".pem", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(tempFolder, entry.Name), true);
                            if (entry.Name.StartsWith("Certificate_"))
                            {
                                certificateFile = Path.Combine(tempFolder, entry.Name);
                            }
                            else if (entry.Name.StartsWith("PrivateKey_"))
                            {
                                privateKeyFile = Path.Combine(tempFolder, entry.Name);
                            }
                        }
                    }

                    var outputFile = string.Empty;

                    if (!string.IsNullOrEmpty(certificateFile)
                        && !string.IsNullOrEmpty(certificateFile))
                    {
                        var password = CreateRandomPassword(10);
                        var startpos = certificateFile.IndexOf("Certificate_") + 12;
                        var name = certificateFile.Substring(startpos, certificateFile.Length - startpos - 4);
                        outputFile = tempFolder + name + "-certificate.p12";
                        var arg = string.Format("{0} {1} {2} pass:{3} {4} {5}",
                            certificateFile, privateKeyFile, "mc-ca-chain.pem", password, name, outputFile);

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenSSL\\CreateCert.cmd");
                        startInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenSSL");
                        startInfo.Arguments = arg;

                        var p = Process.Start(startInfo);
                        p.WaitForExit();

                        var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                        SpisCertificatePassword = password;

                        SpisCertificate = new X509Certificate2(outputFile, SpisCertificatePassword, X509KeyStorageFlags.Exportable);
                    }

                    if (!string.IsNullOrEmpty(certificateFile))
                        File.Delete(certificateFile);

                    if (!string.IsNullOrEmpty(privateKeyFile))
                        File.Delete(privateKeyFile);

                    if (!string.IsNullOrEmpty(outputFile))
                        File.Delete(outputFile);
                }
            }
        }

        private DelegateCommand _saveSpisInstanceSettingsCommand;
        public DelegateCommand SaveSpisInstanceSettingsCommand
        {
            get
            {
                return _saveSpisInstanceSettingsCommand ??
                    (_saveSpisInstanceSettingsCommand = new DelegateCommand(ExecuteSaveSpisInstanceSettingsCommand, CanExecuteSaveSpisInstanceSettingsCommand));
            }
        }

        public bool CanExecuteSaveSpisInstanceSettingsCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteSaveSpisInstanceSettingsCommand(object parameter)
        {
            this.Busy = true;
            this.BusyContent = "Saving SPIS instanse settings";

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                    var encryptedPassword = Encryption.EncryptString(SpisCertificatePassword, encryptionPassword);
                    SpisInstanceSettings.Password = encryptedPassword;
                    SpisInstanceSettings.ClientCertificate = null;
                    SpisInstanceSettings.ClientCertificate = SpisCertificate.Export(X509ContentType.Pkcs12, SpisCertificatePassword);

                    InstanceService.SaveSpisInstanceSettings(SelectedInstance, SpisInstanceSettings);
                    MessageBox.Show("SPIS instance settings has been successfully saved.", "SPIS instance settingse", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when saving SPIS instance settings " + Environment.NewLine + ex.Message, "SPIS instance settingse", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Busy = false;
            }
        }

        private DelegateCommand _cancelSpisInstanceSettingsCommand;
        public DelegateCommand CancelSpisInstanceSettingsCommand
        {
            get
            {
                return _cancelSpisInstanceSettingsCommand ??
                    (_cancelSpisInstanceSettingsCommand = new DelegateCommand(ExecuteCancelSpisInstanceSettingsCommand, CanExecuteCancelSpisInstanceSettingsCommand));
            }
        }

        public bool CanExecuteCancelSpisInstanceSettingsCommand(object parameter)
        {
            return true;
        }

        public void ExecuteCancelSpisInstanceSettingsCommand(object parameter)
        {
            SpisInstanceSettings = InstanceService.GetSpisInstanceSettings(SelectedInstance);
        }

        private DelegateCommand _refreshListCommand;
        public DelegateCommand RefreshListCommand
        {
            get
            {
                return _refreshListCommand ??
                    (_refreshListCommand = new DelegateCommand(ExecuteRefreshListCommand, CanExecuteRefreshListCommand));
            }
        }

        public bool CanExecuteRefreshListCommand(object parameter)
        {
            return true;
        }

        public void ExecuteRefreshListCommand(object parameter)
        {
            LoadInstances();
        }

        private DelegateCommand _addInstanceCommand;
        public DelegateCommand AddInstanceCommand
        {
            get
            {
                return _addInstanceCommand ??
                    (_addInstanceCommand = new DelegateCommand(ExecuteAddInstanceCommand, CanExecuteAddInstanceCommand));
            }
        }

        public bool CanExecuteAddInstanceCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteAddInstanceCommand(object parameter)
        {
            try
            {
                var dlg = new CreateDatabaseDialog();
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (dlg.ShowDialog() == true)
                {
                    var dbName = dlg.ViewModel.DbName;

                    this.Busy = true;
                    this.BusyContent = "Adding new instanse ";

                    await Task.Factory.StartNew(() =>
                    {
                        if (InstanceService.AddInstance(dbName))
                        {
                            LoadInstances();
                        }
                    });
                }
            }
            finally
            {
                this.Busy = false;
            }
        }

        private DelegateCommand _deleteInstanceCommand;
        public DelegateCommand DeleteInstanceCommand
        {
            get
            {
                return _deleteInstanceCommand ??
                    (_deleteInstanceCommand = new DelegateCommand(ExecuteDeleteInstanceCommand, CanExecuteDeleteInstanceCommand));
            }
        }

        public bool CanExecuteDeleteInstanceCommand(object parameter)
        {
            return SelectedInstance != null;
        }

        public async void ExecuteDeleteInstanceCommand(object parameter)
        {
            if (MessageBox.Show("Are you sure that you want to delete the instance " + SelectedInstance + Environment.NewLine + "This will remove all data in th instance and can not be undone.", "Delete instance", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Busy = true;
                this.BusyContent = "Deleting instanse " + SelectedInstance;

                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (InstanceService.DeleteInstance(SelectedInstance))
                        {
                            LoadInstances();
                        }
                    });
                }
                finally
                {
                    this.Busy = false;
                }
            }
        }



        private DelegateCommand _generateAppIdCommand;
        public DelegateCommand GenerateAppIdCommand
        {
            get
            {
                return _generateAppIdCommand ??
                    (_generateAppIdCommand = new DelegateCommand(ExecuteGenerateAppIdCommand, CanExecuteGenerateAppIdCommand));
            }
        }

        public bool CanExecuteGenerateAppIdCommand(object parameter)
        {
            return true;
        }

        public void ExecuteGenerateAppIdCommand(object parameter)
        {
            var param = parameter as string;
            if (param == null)
                return;

            if (param == "VIS")
            {
                VisInstanceSettings.ApplicationId = Guid.NewGuid().ToString("N");
                OnPropertyChanged(() => VisInstanceSettings);
            }
            else
            {
                SpisInstanceSettings.ApplicationId = Guid.NewGuid().ToString("N");
                OnPropertyChanged(() => SpisInstanceSettings);
            }
        }

        private DelegateCommand _generateApiKeyCommand;
        public DelegateCommand GenerateApiKeyCommand
        {
            get
            {
                return _generateApiKeyCommand ??
                    (_generateApiKeyCommand = new DelegateCommand(ExecuteGenerateApiKeyCommand, CanExecuteGenerateApiKeyCommand));
            }
        }

        public bool CanExecuteGenerateApiKeyCommand(object parameter)
        {
            return true;
        }

        public void ExecuteGenerateApiKeyCommand(object parameter)
        {
            var param = parameter as string;
            if (param == null)
                return;

            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
                var APIKey = Convert.ToBase64String(secretKeyByteArray);

                if (param == "VIS")
                {
                    VisInstanceSettings.ApiKey = APIKey;
                    OnPropertyChanged(() => VisInstanceSettings);
                }
                else
                {
                    SpisInstanceSettings.ApiKey = APIKey;
                    OnPropertyChanged(() => SpisInstanceSettings);
                }
            }
        }

        #endregion

        #region private
        private void LoadInstances()
        {
            this.Busy = true;
            this.BusyContent = "Loading instances";

            try
            {
                var response = InstanceService.GetAllDbInstances();
                if (response != null)
                {
                    Instances = new ObservableCollection<string>(response);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading instances. " + ex.Message, "Load instances", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Busy = false;
            }
        }

        private static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        #endregion
    }
}