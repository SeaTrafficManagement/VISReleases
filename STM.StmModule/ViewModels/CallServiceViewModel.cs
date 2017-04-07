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
    public class CallServiceViewModel : ViewModelBase
    {
        public CallServiceViewModel()
        {
        }

        private string _url;
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (_url == value)
                    return;

                _url = value;

                OnPropertyChanged(() => Url);
            }
        }

        private string _contentType = "application/json; charset=UTF8;charset=UTF8";
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                if (_contentType == value)
                    return;

                _contentType = value;

                OnPropertyChanged(() => ContentType);
            }
        }

        private string _requestType = "POST";
        public string RequestType
        {
            get
            {
                return _requestType;
            }
            set
            {
                if (_requestType == value)
                    return;

                _requestType = value;

                OnPropertyChanged(() => RequestType);
            }
        }

        private string _postData;
        public string PostData
        {
            get
            {
                return _postData;
            }
            set
            {
                if (_postData == value)
                    return;

                _postData = value;

                OnPropertyChanged(() => PostData);
            }
        }

        private string _response;
        public string Response
        {
            get
            {
                return _response;
            }
            set
            {
                if (_response == value)
                    return;

                _response = value;

                OnPropertyChanged(() => Response);
            }
        }

        private ICommand _callServiceCommand;
        public ICommand CallServiceCommand
        {
            get
            {
                return _callServiceCommand ??
                    (_callServiceCommand = new DelegateCommand(ExecuteCallServiceCommand, CanExecuteCallServiceCommand));
            }
        }

        public bool CanExecuteCallServiceCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteCallServiceCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Executing call service";
            await Task.Factory.StartNew(() =>
            {
                var visService = new VisService();
                Response = visService.CallService(PostData, Url, RequestType, ContentType);
            });

            Busy = false;
        }
    }
}