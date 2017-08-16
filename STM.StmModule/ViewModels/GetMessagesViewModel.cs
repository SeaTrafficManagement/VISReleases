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
    public class GetMessagesViewModel : ViewModelBase
    {
        public GetMessagesViewModel()
        {
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

        private string _fromTime;
        public string FromTime
        {
            get
            {
                return _fromTime;
            }
            set
            {
                if (_fromTime == value)
                    return;

                _fromTime = value;

                OnPropertyChanged(() => FromTime);
            }
        }

        private string _toTime;
        public string ToTime
        {
            get
            {
                return _toTime;
            }
            set
            {
                if (_toTime == value)
                    return;

                _toTime = value;

                OnPropertyChanged(() => ToTime);
            }
        }

        private int _limitQuery = 10;
        public int LimitQuery
        {
            get
            {
                return _limitQuery;
            }
            set
            {
                if (_limitQuery == value)
                    return;

                _limitQuery = value;

                OnPropertyChanged(() => LimitQuery);
            }
        }

        private int _remainingMessages;
        public int RemainingMessages
        {
            get
            {
                return _remainingMessages;
            }
            set
            {
                if (_remainingMessages == value)
                    return;

                _remainingMessages = value;

                OnPropertyChanged(() => RemainingMessages);
            }
        }

        private Message _selectedMessage;
        public Message SelectedMessage
        {
            get
            {
                return _selectedMessage;
            }
            set
            {
                if (_selectedMessage == value)
                    return;

                _selectedMessage = value;

                OnPropertyChanged(() => SelectedMessage);
                ShowOnMapCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                if (_messages == value)
                    return;

                _messages = value;

                OnPropertyChanged(() => Messages);
            }
        }


        private ICommand _getMessagesCommand;
        public ICommand GetMessagesCommand
        {
            get
            {
                return _getMessagesCommand ??
                    (_getMessagesCommand = new DelegateCommand(ExecuteGetMessagesCommand, CanExecuteGetMessagesCommand));
            }
        }

        public bool CanExecuteGetMessagesCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteGetMessagesCommand(object parameter)
        {
            DateTime? from = null;
            DateTime? to = null;
            DateTime temp;

            if (!string.IsNullOrEmpty(FromTime))
            {
                if (DateTime.TryParse(FromTime, out temp))
                {
                    from = temp;
                }
                else
                {
                    MessageBox.Show("Invalid from time");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(ToTime))
            {
                if (DateTime.TryParse(ToTime, out temp))
                {
                    to = temp;
                }
                else
                {
                    MessageBox.Show("Invalid to time");
                    return;
                }
            }

            if (from != null)
                Messages = new ObservableCollection<Message>();

            var visService = new VisService();
            MessageEnvelope result = null;

            Busy = true;
            BusyContent = "Loading messages from VIS";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    result = visService.GetMessages(Id, LimitQuery, from, to);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Busy = false;
                }
            });

            if (result == null)
                return;

            RemainingMessages = result.RemainingNumberOfMessages ?? 0;

            if (result.Messages != null)
            {
                foreach (var message in result.Messages)
                {
                    if (message != null && message.StmMessage != null && message.StmMessage.Message != null)
                        message.StmMessage.Message = XmlUtil.FormatXml(message.StmMessage.Message);

                    Messages.Add(message);
                }

                Messages = new ObservableCollection<Message>(Messages.OrderBy(x => x.ReceivedAt));
            }
        }

        private ICommand _getPCMMessagesCommand;

        public ICommand GetPCMMessagesCommand
        {
            get
            {
                return _getPCMMessagesCommand ??
                    (_getPCMMessagesCommand = new DelegateCommand(ExecuteGetPCMMessagesCommand, CanExecuteGetPCMMessagesCommand));
            }
        }

        private bool CanExecuteGetPCMMessagesCommand(object obj)
        {
            return true;
        }

        private async void ExecuteGetPCMMessagesCommand(object obj)
        {
            DateTime? from = null;
            DateTime? to = null;
            DateTime temp;

            if (!string.IsNullOrEmpty(FromTime))
            {

                if (DateTime.TryParse(FromTime, out temp))
                {
                    from = temp;
                }
                else
                {
                    MessageBox.Show("Invalid from time");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(ToTime))
            {
                if (DateTime.TryParse(ToTime, out temp))
                {
                    to = temp;
                }
                else
                {
                    MessageBox.Show("Invalid to time");
                    return;
                }
            }

            if (from != null)
                Messages = new ObservableCollection<Message>();

            var service = new SpisService();
            MessageEnvelope result = null;

            Busy = true;
            BusyContent = "Loading messages from SPIS";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    result = service.GetMessages(Id, LimitQuery, from, to);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Busy = false;
                }
            });

            if (result == null)
                return;

            RemainingMessages = result.RemainingNumberOfMessages ?? 0;

            if (result.Messages != null)
            {
                foreach (var message in result.Messages)
                {
                    if (message != null && message.StmMessage != null && message.StmMessage.Message != null)
                        message.StmMessage.Message = XmlUtil.FormatXml(message.StmMessage.Message);

                    Messages.Add(message);
                }
            }
        }


        private DelegateCommand _showOnMapCommand;
        public DelegateCommand ShowOnMapCommand
        {
            get
            {
                return _showOnMapCommand ??
                    (_showOnMapCommand = new DelegateCommand(ExecuteShowOnMapCommand, CanExecuteShowOnMapCommand));
            }
        }

        public bool CanExecuteShowOnMapCommand(object parameter)
        {
            return SelectedMessage != null 
                && (SelectedMessage.MessageType == "RTZ" || SelectedMessage.MessageType == "TXT");
        }

        public void ExecuteShowOnMapCommand(object parameter)
        {
            if (SelectedMessage.MessageType == "RTZ")
                MapRoutes.AddRoute(SelectedMessage.StmMessage.Message, System.Windows.Media.Colors.Red);

            if (SelectedMessage.MessageType == "TXT")
                MapRoutes.AddTextMessage(SelectedMessage.StmMessage.Message, System.Windows.Media.Colors.Green);
        }
    }
}