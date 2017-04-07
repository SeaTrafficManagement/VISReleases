using Microsoft.Win32;
using STM.StmModule.Simulator.Contract;
using STM.StmModule.Simulator.Infrastructure;
using STM.StmModule.Simulator.Services;

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
    public class NewVoyageplanViewModel : ViewModelBase
    {
        public NewVoyageplanViewModel()
        {
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

        private string _rtz;
        public string Rtz
        {
            get
            {
                return _rtz;
            }
            set
            {
                if (_rtz == value)
                    return;

                _rtz = value;

                OnPropertyChanged(() => Rtz);
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
                Rtz = FormatXml(File.ReadAllText(openFileDialog.FileName));
            }
        }

        private string FormatXml(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            StringBuilder sb = new StringBuilder();
            System.IO.TextWriter tr = new System.IO.StringWriter(sb);
            XmlTextWriter wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.Indented;
            doc.Save(wr);
            wr.Close();
            return sb.ToString();
        }

    }
}
