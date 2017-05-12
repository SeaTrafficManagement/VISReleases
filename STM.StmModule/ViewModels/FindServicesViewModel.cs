using Microsoft.Win32;
using STM.StmModule.Simulator.Contract;
using STM.StmModule.Simulator.Infrastructure;
using STM.StmModule.Simulator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace STM.StmModule.Simulator.ViewModels
{
    public class FindServicesViewModel : ViewModelBase
    {
        public FindServicesViewModel()
        {
            Services = new ObservableCollection<ServiceInstance>();

            var services = ConfigurationManager.AppSettings["Services"];
            if (services != null)
            {
                try
                {
                    foreach (var service in services.Split(';'))
                    {
                        var tmp = service.Split('#');
                        if (tmp != null && tmp.Length == 2)
                        Services.Add(new ServiceInstance
                        {
                            Name = tmp[0],
                            EndpointUri = tmp[1],
                            InstanceAsXml = new Xml()
                        });
                    }
                }
                catch (Exception)
                { }
                
            }
        }

        #region properties
        private string _area;
        public string Area
        {
            get
            {
                return _area;
            }
            set
            {
                if (_area == value)
                    return;
                _area = value;

                OnPropertyChanged(() => Area);
            }
        }

        private string _unloCode;
        public string UnloCode
        {
            get
            {
                return _unloCode;
            }
            set
            {
                if (_unloCode == value)
                    return;
                _unloCode = value;

                OnPropertyChanged(() => UnloCode);
            }
        }

        private string _serviceProviderIds;
        public string ServiceProviderIds
        {
            get
            {
                return _serviceProviderIds;
            }
            set
            {
                if (_serviceProviderIds == value)
                    return;
                _serviceProviderIds = value;

                OnPropertyChanged(() => ServiceProviderIds);
            }
        }

        private string _serviceDesignId;
        public string ServiceDesignId
        {
            get
            {
                return _serviceDesignId;
            }
            set
            {
                if (_serviceDesignId == value)
                    return;

                _serviceDesignId = value;

                OnPropertyChanged(() => ServiceDesignId);
            }
        }

        private string _serviceInstanceId;
        public string ServiceInstanceId
        {
            get
            {
                return _serviceInstanceId;
            }
            set
            {
                if (_serviceInstanceId == value)
                    return;

                _serviceInstanceId = value;

                OnPropertyChanged(() => ServiceInstanceId);
            }
        }

        private string _mmsi;
        public string Mmsi
        {
            get
            {
                return _mmsi;
            }
            set
            {
                if (_mmsi == value)
                    return;
                _mmsi = value;

                OnPropertyChanged(() => Mmsi);
            }
        }

        private string _imo;
        public string Imo
        {
            get
            {
                return _imo;
            }
            set
            {
                if (_imo == value)
                    return;
                _imo = value;

                OnPropertyChanged(() => Imo);
            }
        }

        private string _serviceType;
        public string ServiceType
        {
            get
            {
                return _serviceType;
            }
            set
            {
                if (_serviceType == value)
                    return;
                _serviceType = value;

                OnPropertyChanged(() => ServiceType);
            }
        }

        private string _serviceStatus;
        public string ServiceStatus
        {
            get
            {
                return _serviceStatus;
            }
            set
            {
                if (_serviceStatus == value)
                    return;
                _serviceStatus = value;

                OnPropertyChanged(() => ServiceStatus);
            }
        }

        private string _keywords;
        public string Keywords
        {
            get
            {
                return _keywords;
            }
            set
            {
                if (_keywords == value)
                    return;

                _keywords = value;

                OnPropertyChanged(() => Keywords);
            }
        }

        private string _freeText;
        public string FreeText
        {
            get
            {
                return _freeText;
            }
            set
            {
                if (_freeText == value)
                    return;
                _freeText = value;

                OnPropertyChanged(() => FreeText);
            }
        }

        private ObservableCollection<ServiceInstance> _services;
        public ObservableCollection<ServiceInstance> Services
        {
            get
            {
                return _services;
            }
            set
            {
                if (_services == value)
                    return;

                _services = value;

                OnPropertyChanged(() => Services);
            }
        }

        private ServiceInstance _selectedService;
        public ServiceInstance SelectedService
        {
            get
            {
                return _selectedService;
            }
            set
            {
                if (_selectedService == value)
                    return;

                _selectedService = value;

                if (_selectedService != null)
                {
                    Text = _selectedService.InstanceAsXml.Content;
                }

                OnPropertyChanged(() => SelectedService);
                UploadVPCommand.RaiseCanExecuteChanged();
                GetVPCommand.RaiseCanExecuteChanged();
                UploadAreaCommand.RaiseCanExecuteChanged();
                UploadTextCommand.RaiseCanExecuteChanged();
                SubscribeCommand.RaiseCanExecuteChanged();
                DeleteSubscriberCommand.RaiseCanExecuteChanged();
            }
        }

        private Message _resultMessage;
        public Message ResultMessage
        {
            get
            {
                return _resultMessage;
            }
            set
            {
                if (_resultMessage == value)
                    return;

                _resultMessage = value;

                if (ResultMessage != null)
                    Text = _resultMessage.StmMessage.Message;

                OnPropertyChanged(() => ResultMessage);
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;

                _text = value;

                OnPropertyChanged(() => Text);
            }
        }
        
        #endregion

        #region commands
        private ICommand _findServicesCommand;
        public ICommand FindServicesCommand
        {
            get
            {
                return _findServicesCommand ??
                    (_findServicesCommand = new DelegateCommand(ExecuteFindServicesCommand, CanExecuteFindServicesCommand));
            }
        }

        public bool CanExecuteFindServicesCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteFindServicesCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Searching Service Registry";
            await Task.Factory.StartNew(() =>
            {
                var visService = new VisService();

                try
                {
                    List<string> ServiceProviderIdsList = null;
                    if (!string.IsNullOrEmpty(ServiceProviderIds))
                    {
                        string[] s = ServiceProviderIds.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        ServiceProviderIdsList = s.ToList();
                    }

                    List<string> KeywordsList = null;
                    if (!string.IsNullOrEmpty(Keywords))
                    {
                        string[] s = Keywords.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        KeywordsList = s.ToList();
                    }

                    Services = visService.FindServices("WKT", Area, UnloCode, ServiceProviderIdsList, ServiceDesignId, ServiceInstanceId,
                        Mmsi, Imo, ServiceType, ServiceStatus, KeywordsList, FreeText, null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            Busy = false;

        }

        private DelegateCommand _uploadVPCommand;
        public DelegateCommand UploadVPCommand
        {
            get
            {
                return _uploadVPCommand ??
                    (_uploadVPCommand = new DelegateCommand(ExecuteUploadVPCommand, CanExecuteUploadVPCommand));
            }
        }

        private bool CanExecuteUploadVPCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteUploadVPCommand(object obj)
        {
            string vpbody = null;
            bool sendAck = false;
            string callbackEndpoint = string.Empty;

            var dlg = new NewSTMMessageDialog();
            dlg.ViewModel.ShowAcknowledgement = true;
            dlg.ViewModel.ShowCallbackEndpoint = true;
            if (dlg.ShowDialog() == true)
            {
                var newMsg = new PublishedMessageContract
                {
                    Message = dlg.ViewModel.StmMsg,
                    //MessageID = dlg.ViewModel.Id
                };
                sendAck = dlg.ViewModel.Acknowledgement;
                vpbody = newMsg.Message;
                callbackEndpoint = dlg.ViewModel.CallbackEndpoint;
            }
            if (!string.IsNullOrEmpty(vpbody))
            {
                Busy = true;
                BusyContent = "Uploading voyageplan";
                await Task.Factory.StartNew(() =>
                {
                    var visService = new VisService();
                    if (sendAck)
                    {
                        string myAck = ConfigurationManager.AppSettings.Get("VisPublicUrl").Replace("{database}", VisService.DbName);
                        string endpointUri = SelectedService.EndpointUri;
                        if (!string.IsNullOrEmpty(callbackEndpoint))
                        {
                            endpointUri += string.Format("/voyagePlans?callbackEndpoint={0}", callbackEndpoint) + string.Format("&deliveryAckEndPoint={0}", myAck);
                        }
                        else
                        {
                            endpointUri += string.Format("/voyagePlans?deliveryAckEndPoint={0}", myAck);
                        }
                        var result = visService.CallService(vpbody, endpointUri, "POST", "text/xml; charset=UTF8");
                        MessageBox.Show(result);
                    }
                    else
                    {
                        string endpointUri = SelectedService.EndpointUri + string.Format("/voyagePlans?callbackEndpoint={0}", callbackEndpoint);
                        var result = visService.CallService(vpbody, endpointUri, "POST", "text/xml; charset=UTF8");
                        MessageBox.Show(result);
                    }
                });

                Busy = false;
                
            }         
        }

        private DelegateCommand _uploadTextCommand;
        public DelegateCommand UploadTextCommand
        {
            get
            {
                return _uploadTextCommand ??
                    (_uploadTextCommand = new DelegateCommand(ExecuteUploadTextCommand, CanExecuteUploadTextCommand));
            }
        }

        private bool CanExecuteUploadTextCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteUploadTextCommand(object obj)
        {
            string txtbody = null;
            bool sendAck = false;

            var dlg = new NewSTMMessageDialog();
            dlg.ViewModel.ShowAcknowledgement = true;
            if (dlg.ShowDialog() == true)
            {
                var newMsg = new PublishedMessageContract
                {
                    Message = dlg.ViewModel.StmMsg,
                    MessageID = dlg.ViewModel.Id
                };
                sendAck = dlg.ViewModel.Acknowledgement;
                txtbody = newMsg.Message;
            }
            if (!string.IsNullOrEmpty(txtbody))
            {
                Busy = true;
                BusyContent = "Uploading text message";
                await Task.Factory.StartNew(() =>
                {
                    var visService = new VisService();
                    if (sendAck)
                    {
                        string myAck = ConfigurationManager.AppSettings.Get("VisPublicUrl").Replace("{database}", VisService.DbName); ;
                        string endpointUri = SelectedService.EndpointUri + string.Format("/textMessage?deliveryAckEndPoint={0}", myAck);
                        var result = visService.CallService(txtbody, endpointUri, "POST", "text/xml; charset=UTF8");
                        MessageBox.Show(result);
                    }
                    else
                    {
                        var result = visService.CallService(txtbody, SelectedService.EndpointUri + "/textMessage", "POST", "text/xml; charset=UTF8;charset=UTF8");
                        MessageBox.Show(result);
                    }
                });

                Busy = false;
            }
        }

        private DelegateCommand _uploadAreaCommand;
        public DelegateCommand UploadAreaCommand
        {
            get
            {
                return _uploadAreaCommand ??
                    (_uploadAreaCommand = new DelegateCommand(ExecuteUploadAreaCommand, CanExecuteUploadAreaCommand));
            }
        }

        private bool CanExecuteUploadAreaCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteUploadAreaCommand(object obj)
        {
            string areaBody = null;
            bool sendAck = false;

            var dlg = new NewSTMMessageDialog();
            dlg.ViewModel.ShowAcknowledgement = true;
            if (dlg.ShowDialog() == true)
            {
                var newMsg = new PublishedMessageContract
                {
                    Message = dlg.ViewModel.StmMsg,
                    MessageID = dlg.ViewModel.Id
                };
                sendAck = dlg.ViewModel.Acknowledgement;
                areaBody = newMsg.Message;
            }
            if (!string.IsNullOrEmpty(areaBody))
            {
                Busy = true;
                BusyContent = "Uploading text message";
                await Task.Factory.StartNew(() =>
                {
                    var visService = new VisService();
                    if (sendAck)
                    {
                        string myAck = ConfigurationManager.AppSettings.Get("VisPublicUrl").Replace("{database}", VisService.DbName); ;
                        string endpointUri = SelectedService.EndpointUri + string.Format("/area?deliveryAckEndPoint={0}", myAck);
                        var result = visService.CallService(areaBody, endpointUri, "POST", "text/xml; charset=UTF8");
                        MessageBox.Show(result);
                    }
                    else
                    {
                        var result = visService.CallService(areaBody, SelectedService.EndpointUri + "/area", "POST", "text/xml; charset=UTF8;charset=UTF8");
                        MessageBox.Show(result);
                    }
                });

                Busy = false;

            }
        }

        private DelegateCommand _getVPCommand;
        public DelegateCommand GetVPCommand
        {
            get
            {
                return _getVPCommand ?? (_getVPCommand = new DelegateCommand(ExecuteGetVPCommand, CanExecuteGetVPCommand));
            }
        }

        private bool CanExecuteGetVPCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteGetVPCommand(object obj)
        {
            string endpoint = SelectedService.EndpointUri + "/voyagePlans";
            string uvid;
            string routeStatus;

            var dlg = new GetVPDialog();
            dlg.ShowDialog();

            uvid = dlg.ViewModel.Uvid;
            routeStatus = dlg.ViewModel.RouteStatus;

            Busy = true;
            BusyContent = "Get voyageplan";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(uvid) && string.IsNullOrEmpty(routeStatus))
                    {
                        endpoint += "?uvid=" + uvid;
                    }
                    if (string.IsNullOrEmpty(uvid) && !string.IsNullOrEmpty(routeStatus))
                    {
                        endpoint += "?routeStatus=" + routeStatus;
                    }
                    if (!string.IsNullOrEmpty(uvid) && !string.IsNullOrEmpty(routeStatus))
                    {
                        endpoint += "?uvid=" + uvid;
                        endpoint += "&routeStatus=" + routeStatus;
                    }

                    var visService = new VisService();
                    var result = visService.CallService(null, endpoint, "GET", "application/json; charset=UTF8");

                    if (!string.IsNullOrEmpty(result))
                    {
                        var msg = new Message();
                        msg.FromServiceId = SelectedService.InstanceId;
                        msg.MessageType = "RTZ";
                        msg.ReceivedAt = DateTime.UtcNow;
                        msg.Id = "123";
                        msg.StmMessage = new StmMessage(result);

                        ResultMessage = msg;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            Busy = false;
        }

        private DelegateCommand _subscribeCommand;
        public DelegateCommand SubscribeCommand
        {
            get
            {
                return _subscribeCommand ?? (_subscribeCommand = new DelegateCommand(ExecuteSubscribeCommand, CanExecuteSubscribeCommand));
            }
        }

        private bool CanExecuteSubscribeCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteSubscribeCommand(object obj)
        {
            //http://localhost/STM.VIS.Services.Public/VIS/V2/voyagePlans/subscription?callbackEndpoint=hello.com

            string endpoint = SelectedService.EndpointUri + "/voyagePlans/subscription?callbackEndpoint=";
            endpoint += ConfigurationManager.AppSettings["VisPublicUrl"].Replace("{database}", VisService.DbName);
            string uvid;
            //Dialog to get uvid parameter
            var dlg = new SubscriptionDialog();
            dlg.ShowDialog();
            uvid = dlg.ViewModel.Uvid;

            Busy = true;
            BusyContent = "Post subscription";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var visService = new VisService();
                    if (!string.IsNullOrEmpty(uvid))
                    {
                        endpoint += "&uvid=" + uvid;
                    }

                    var result = visService.CallService(null, endpoint, "POST", "application/json; charset=UTF8");
                    MessageBox.Show(result);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            });
            Busy = false;
            
        }

        private DelegateCommand _deleteSubscriberCommand;
        public DelegateCommand DeleteSubscriberCommand
        {
            get
            {
                return _deleteSubscriberCommand ?? (_deleteSubscriberCommand = new DelegateCommand(ExecuteDeleteSubscriberCommand, CanExecuteDeleteSubscriberCommand));
            }
        }

        private bool CanExecuteDeleteSubscriberCommand(object obj)
        {
            return SelectedService != null;
        }

        private async void ExecuteDeleteSubscriberCommand(object obj)
        {

            string endpoint = SelectedService.EndpointUri + "/voyagePlans/subscription";

            string uvid;
            string callbackEndpoint;
            //Dialog to get uvid parameter
            var dlg = new DeleteSubscriptionDialog();
            dlg.ShowDialog();
            uvid = dlg.ViewModel.Uvid;
            callbackEndpoint = dlg.ViewModel.CallbackEndpoint;

            Busy = true;
            BusyContent = "Delete subscription";

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var visService = new VisService();
                    if(!string.IsNullOrEmpty(uvid) && !string.IsNullOrEmpty(callbackEndpoint))
                    {
                        var endpAddress = string.Format(endpoint + "?uvid={0}&callbackEndpoint={1}", uvid, callbackEndpoint);
                        var result = visService.CallService(null, endpAddress, "DELETE", "application/json; charset=UTF8");
                        MessageBox.Show(result);
                    }
                    else
                    {
                        MessageBox.Show("Mandatory parameters uvid and callbackEndpoint are missing or empty!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            Busy = false;
        }
        #endregion
    }
}
