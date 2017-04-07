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
    public class RtzMessagesViewModel : ViewModelBase
    {
        public RtzMessagesViewModel()
        {
        }

        private PublishedMessageContract _selectedMessage;
        public PublishedMessageContract SelectedMessage
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
                PublishMessageCommand.RaiseCanExecuteChanged();
                DeleteMessageCommand.RaiseCanExecuteChanged();
                ShowAclCommand.RaiseCanExecuteChanged();
                ShowSubscribersCommand.RaiseCanExecuteChanged();
                ShowOnMapCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<PublishedMessageContract> _messages = new ObservableCollection<PublishedMessageContract>();
        public ObservableCollection<PublishedMessageContract> Messages
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


        private DelegateCommand _loadPublishedMessagesCommand;
        public DelegateCommand LoadPublishedMessagesCommand
        {
            get
            {
                return _loadPublishedMessagesCommand ??
                    (_loadPublishedMessagesCommand = new DelegateCommand(ExecuteLoadPublishedMessagesCommand, CanExecuteLoadPublishedMessagesCommand));
            }
        }

        public bool CanExecuteLoadPublishedMessagesCommand(object parameter)
        {
            return true;
        }

        public async void ExecuteLoadPublishedMessagesCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Loading published voyageplans from VIS";
            await Task.Factory.StartNew(() =>
            {
                var service = new VisService();
                Messages = new ObservableCollection<PublishedMessageContract>(service.GetPublishedMessages());
            });

            Busy = false;
        }


        private DelegateCommand _createNewMessageCommand;
        public DelegateCommand CreateNewMessageCommand
        {
            get
            {
                return _createNewMessageCommand ??
                    (_createNewMessageCommand = new DelegateCommand(ExecuteCreateNewMessageCommand, CanExecuteCreateNewMessageCommand));
            }
        }

        public bool CanExecuteCreateNewMessageCommand(object parameter)
        {
            return true;
        }

        public void ExecuteCreateNewMessageCommand(object parameter)
        {
            var dlg = new NewSTMMessageDialog();
            dlg.ViewModel.ShowUvid = true;
            if (dlg.ShowDialog() == true)
            {
                var newVp = new PublishedMessageContract
                {
                    Message = dlg.ViewModel.StmMsg,
                    MessageID = dlg.ViewModel.Id
                };

                Messages.Add(newVp);
                SelectedMessage = newVp;
            }
        }

        private DelegateCommand _publishMessageCommand;
        public DelegateCommand PublishMessageCommand
        {
            get
            {
                return _publishMessageCommand ??
                    (_publishMessageCommand = new DelegateCommand(ExecutePublishMessageCommand, CanExecutePublishMessageCommand));
            }
        }

        public bool CanExecutePublishMessageCommand(object parameter)
        {
            return SelectedMessage != null;
        }

        public async void ExecutePublishMessageCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Publishing voyageplan";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var visService = new VisService();
                    var result = visService.PublishMessage(SelectedMessage.MessageID, "RTZ", SelectedMessage.Message);
                    MessageBox.Show(result);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Busy = false;
                }
            });

            
            Busy = false;
        }

        private DelegateCommand _deleteMessageCommand;
        public DelegateCommand DeleteMessageCommand
        {
            get
            {
                return _deleteMessageCommand ??
                    (_deleteMessageCommand = new DelegateCommand(ExecuteDeleteMessageCommand, CanExecuteDeleteMessageCommand));
            }
        }

        public bool CanExecuteDeleteMessageCommand(object parameter)
        {
            return SelectedMessage != null;
        }

        public async void ExecuteDeleteMessageCommand(object parameter)
        {
            if (MessageBox.Show("Are you sure that you want to delete message?", "Delete message", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            Busy = true;
            BusyContent = "Deleting voyageplan";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var visService = new VisService();
                    var result = visService.DeleteMessage(SelectedMessage.MessageID);
                    MessageBox.Show(result);                   
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

            ExecuteLoadPublishedMessagesCommand(null);
        }

        private DelegateCommand _showAclCommand;
        public DelegateCommand ShowAclCommand
        {
            get
            {
                return _showAclCommand ??
                    (_showAclCommand = new DelegateCommand(ExecuteShowAclCommand, CanExecuteShowAclCommand));
            }
        }

        public bool CanExecuteShowAclCommand(object parameter)
        {
            return SelectedMessage != null;
        }

        public void ExecuteShowAclCommand(object parameter)
        {
            var dlg = new AclDialog();
            dlg.ViewModel = new VisAclViewModel();
            dlg.ViewModel.LoadAcl(SelectedMessage.MessageID);
            dlg.Show();
        }

        
        private DelegateCommand _showSubscribersCommand;
        public DelegateCommand ShowSubscribersCommand
        {
            get
            {
                return _showSubscribersCommand ??
                    (_showSubscribersCommand = new DelegateCommand(ExecuteShowSubscribersCommand, CanExecuteShowSubscribersCommand));
            }
        }

        public bool CanExecuteShowSubscribersCommand(object parameter)
        {
            return SelectedMessage != null;
        }

        public void ExecuteShowSubscribersCommand(object parameter)
        {
            var dlg = new VisSubscribersDialog();
            dlg.ViewModel.LoadSubscriptions(SelectedMessage.MessageID);
            dlg.Show();
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
            return SelectedMessage != null;
        }

        public void ExecuteShowOnMapCommand(object parameter)
        {
            MapRoutes.AddRoute(SelectedMessage.Message, System.Windows.Media.Colors.Blue);
        }
    }
}
