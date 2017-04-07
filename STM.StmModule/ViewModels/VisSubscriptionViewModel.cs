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
    public class VisSubscriptionViewModel : ViewModelBase
    {
        public VisSubscriptionViewModel()
        {

        }

        public async void LoadSubscriptions(string id)
        {
            Busy = true;
            BusyContent = "Loading subscriptions";
            Id = id;

            List<VisSubscriptionObject> subscriptions = new List<VisSubscriptionObject>();
            List<IdentityDescriptionObject> acl = new List<IdentityDescriptionObject>();

            SubscriptionList = new ObservableCollection<VisSubscriptionObject>();
            AclList = new ObservableCollection<IdentityDescriptionObject>();

            await Task.Factory.StartNew(() =>
            {
                var visService = new VisService();
                subscriptions = visService.GetSubscriptions(Id);
                acl = visService.GetAcl(Id);
            });


            foreach (var i in acl)
                AclList.Add(i);

            foreach (var i in subscriptions)
                SubscriptionList.Add(i);

            Busy = false;
        }

        private ObservableCollection<VisSubscriptionObject> _subscriptionList;
        public ObservableCollection<VisSubscriptionObject> SubscriptionList
        {
            get
            {
                return _subscriptionList;
            }
            set
            {
                if (_subscriptionList == value)
                    return;

                _subscriptionList = value;

                OnPropertyChanged(() => SubscriptionList);
            }
        }

        private VisSubscriptionObject _selectedSubscription;
        public VisSubscriptionObject SelectedSubscription
        {
            get
            {
                return _selectedSubscription;
            }
            set
            {
                if (_selectedSubscription == value)
                    return;

                _selectedSubscription = value;

                OnPropertyChanged(() => SelectedSubscription);
                DeleteSubscriptionCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<IdentityDescriptionObject> _aclList;
        public ObservableCollection<IdentityDescriptionObject> AclList
        {
            get
            {
                return _aclList;
            }
            set
            {
                if (_aclList == value)
                    return;

                _aclList = value;

                OnPropertyChanged(() => AclList);
            }
        }

        private IdentityDescriptionObject _selectedAcl;
        public IdentityDescriptionObject SelectedAcl
        {
            get
            {
                return _selectedAcl;
            }
            set
            {
                if (_selectedAcl == value)
                    return;

                _selectedAcl = value;

                OnPropertyChanged(() => SelectedAcl);
                AddSubscriptionCommand.RaiseCanExecuteChanged();
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
                AddSubscriptionCommand.RaiseCanExecuteChanged();
            }
        }


        private DelegateCommand _addSubscriptionCommand;
        public DelegateCommand AddSubscriptionCommand
        {
            get
            {
                return _addSubscriptionCommand ??
                    (_addSubscriptionCommand = new DelegateCommand(ExecuteAddSubscriptionCommand, CanExecuteAddSubscriptionCommand));
            }
        }

        public bool CanExecuteAddSubscriptionCommand(object parameter)
        {
            return SelectedAcl != null && Url != null;
        }

        public async void ExecuteAddSubscriptionCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Adding subscription";

            try
            {
                var subscription = new VisSubscriptionObject
                {
                    EndpointURL = new Uri(Url),
                    IdentityId = SelectedAcl.IdentityId,
                    IdentityName = SelectedAcl.IdentityName
                };

                await Task.Factory.StartNew(() =>
                {
                    var visService = new VisService();
                    visService.AddSubscription(Id, new List<VisSubscriptionObject>
                        {
                        subscription
                        });
                });

                SubscriptionList.Add(subscription);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Busy = false;
            }
        }


        private DelegateCommand _deleteSubscriptionCommand;
        public DelegateCommand DeleteSubscriptionCommand
        {
            get
            {
                return _deleteSubscriptionCommand ??
                    (_deleteSubscriptionCommand = new DelegateCommand(ExecuteDeleteSubscriptionCommand, CanExecuteDeleteSubscriptionCommand));
            }
        }

        public bool CanExecuteDeleteSubscriptionCommand(object parameter)
        {
            return SelectedSubscription != null;
        }

        public async void ExecuteDeleteSubscriptionCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Deleting subscription";

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var visService = new VisService();
                    visService.RemoveSubscription(Id, new List<VisSubscriptionObject>
                    {
                    SelectedSubscription
                    });
                });

                SubscriptionList.Remove(SelectedSubscription);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Busy = false;
            }
        }
    }
}
