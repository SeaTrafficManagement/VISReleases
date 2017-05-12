using Microsoft.Win32;
using STM.StmModule.Simulator.Contract;
using STM.StmModule.Simulator.Infrastructure;
using STM.StmModule.Simulator.Services;
using STM.StmModule.Simulator.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace STM.StmModule.Simulator.ViewModels
{
    public class NewSTMMessageViewModel : ViewModelBase
    {
        public NewSTMMessageViewModel()
        {
        }
        private bool _showUvid = false;
        public bool ShowUvid
        {
            get
            {
                return _showUvid;
            }
            set
            {
                if (_showUvid == value)
                    return;
                _showUvid = value;

                OnPropertyChanged(() => ShowUvid);
            }
        }
        private bool _showAcknowledgement;
        public bool ShowAcknowledgement
        {
            get
            {
                return _showAcknowledgement;
            }
            set
            {
                if (_showAcknowledgement == value)
                    return;
                _showAcknowledgement = value;

                OnPropertyChanged(() => ShowAcknowledgement);
            }
        }

        private bool _showCallbackEndpoint = false;
        public bool ShowCallbackEndpoint
        {
            get
            {
                return _showCallbackEndpoint;
            }
            set
            {
                if (_showCallbackEndpoint == value)
                    return;
                _showCallbackEndpoint = value;

                OnPropertyChanged(() => ShowCallbackEndpoint);
            }
        }

        private string _callbackEndpoint;
        public string CallbackEndpoint
        {
            get
            {
                return _callbackEndpoint;
            }
            set
            {
                if (_callbackEndpoint == value)
                    return;
                _callbackEndpoint = value;

                OnPropertyChanged(() => CallbackEndpoint);
            }
        }
        private bool _acknowledgement;
        public bool Acknowledgement
        {
            get
            {
                return _acknowledgement;
            }
            set
            {
                if (_acknowledgement == value)
                    return;
                _acknowledgement = value;

                OnPropertyChanged(() => Acknowledgement);
            }
        }

        private string _stmMsg;
        public string StmMsg
        {
            get
            {
                return _stmMsg;
            }
            set
            {
                if (_stmMsg == value)
                    return;

                _stmMsg = value;

                OnPropertyChanged(() => StmMsg);
            }
        }

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value)
                    return;

                _id = value;

                OnPropertyChanged(() => Id);
            }
        }

        private ICommand _loadRtzCommand;
        public ICommand LoadRtzCommand
        {
            get
            {
                return _loadRtzCommand ??
                    (_loadRtzCommand = new DelegateCommand(ExecuteLoadRtzCommand, CanExecuteLoadRtzCommand));
            }
        }

        public bool CanExecuteLoadRtzCommand(object parameter)
        {
            return true;
        }

        public void ExecuteLoadRtzCommand(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                StmMsg = XmlUtil.FormatXml(File.ReadAllText(openFileDialog.FileName));
            }
        }
    }
}
