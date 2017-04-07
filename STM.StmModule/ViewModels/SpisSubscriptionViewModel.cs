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
    public class SpisSubscriptionViewModel : ViewModelBase
    {
        public SpisSubscriptionViewModel()
        {

        }

        public async void LoadSubscriptions(string id)
        {
            Busy = true;
            BusyContent = "Loading subscriptions";
            Id = id;

            List<SpisSubscriptionObject> subscriptions = new List<SpisSubscriptionObject>();
            List<IdentityDescriptionObject> acl = new List<IdentityDescriptionObject>();

            SubscriptionList = new ObservableCollection<SpisSubscriptionObject>();
            AclList = new ObservableCollection<IdentityDescriptionObject>();

            await Task.Factory.StartNew(() =>
            {
                var SpisService = new SpisService();
                subscriptions = SpisService.GetSubscriptions(Id);
                acl = SpisService.GetAcl(Id);
            });


            foreach (var i in acl)
                AclList.Add(i);

            foreach (var i in subscriptions)
                SubscriptionList.Add(i);

            Busy = false;
        }

        private ObservableCollection<SpisSubscriptionObject> _subscriptionList;
        public ObservableCollection<SpisSubscriptionObject> SubscriptionList
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

        private SpisSubscriptionObject _selectedSubscription;
        public SpisSubscriptionObject SelectedSubscription
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

        private string _amssUrl;
        public string AmssUrl
        {
            get
            {
                return _amssUrl;
            }
            set
            {
                if (_amssUrl == value)
                    return;

                _amssUrl = value;

                OnPropertyChanged(() => AmssUrl);
                AddSubscriptionCommand.RaiseCanExecuteChanged();
            }
        }

        private string _mbUrl;
        public string MbUrl
        {
            get
            {
                return _mbUrl;
            }
            set
            {
                if (_mbUrl == value)
                    return;

                _mbUrl = value;

                OnPropertyChanged(() => MbUrl);
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
            return SelectedAcl != null && AmssUrl != null;
        }

        public async void ExecuteAddSubscriptionCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Adding subscription";

            try
            {
                var subscription = new SpisSubscriptionObject
                {
                    AmssEndpointURL = new Uri(AmssUrl),
                    MbEndpointURL = new Uri(MbUrl),
                    IdentityId = SelectedAcl.IdentityId,
                    IdentityName = SelectedAcl.IdentityName
                };

                await Task.Factory.StartNew(() =>
                {
                    var SpisService = new SpisService();
                    SpisService.AddSubscription(Id, new List<SpisSubscriptionObject>
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
                    var SpisService = new SpisService();
                    SpisService.RemoveSubscription(Id, new List<SpisSubscriptionObject>
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