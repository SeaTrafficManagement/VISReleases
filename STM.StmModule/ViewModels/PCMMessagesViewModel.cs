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
    public class PCMMessagesViewModel : ViewModelBase
    {
        public PCMMessagesViewModel()
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
                DeleteMessageCommand.RaiseCanExecuteChanged();
                PublishMessageCommand.RaiseCanExecuteChanged();
                ShowAclCommand.RaiseCanExecuteChanged();
                ShowSubscribersCommand.RaiseCanExecuteChanged();
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
            BusyContent = "Loading published PCM from SPIS";
            await Task.Factory.StartNew(() =>
            {
                var service = new SpisService();
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
            BusyContent = "Publishing PCM";

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var service = new SpisService();
                    var result = service.PublishMessage(SelectedMessage.MessageID, "PCM", SelectedMessage.Message);
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
            BusyContent = "Deleting message";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var spisService = new SpisService();
                    var result = spisService.DeleteMessage(SelectedMessage.MessageID);
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
            dlg.ViewModel = new SpisAclViewModel();
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
            var dlg = new SpisSubscribersDialog();
            dlg.ViewModel.LoadSubscriptions(SelectedMessage.MessageID);
            dlg.Show();
        }

    }
}
