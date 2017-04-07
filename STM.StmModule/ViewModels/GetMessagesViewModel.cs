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
            var visService = new VisService();
            MessageEnvelope result = null;

            Busy = true;
            BusyContent = "Loading messages from VIS";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    result = visService.GetMessages(Id, LimitQuery);
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
                    Messages.Add(message);
                    if (message.MessageType == "RTZ")
                    {
                        MapRoutes.AddRoute(message.StmMessage.Message, System.Windows.Media.Colors.Red);
                    }
                }
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
            var service = new SpisService();
            MessageEnvelope result = null;

            Busy = true;
            BusyContent = "Loading messages from SPIS";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    service.GetMessages(Id, LimitQuery);
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
            return SelectedMessage != null && SelectedMessage.MessageType == "RTZ";
        }

        public void ExecuteShowOnMapCommand(object parameter)
        {
            MapRoutes.AddRoute(SelectedMessage.StmMessage.Message, System.Windows.Media.Colors.Red);
        }
    }
}