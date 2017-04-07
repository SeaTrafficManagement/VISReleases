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
    public class SpisAclViewModel : ViewModelBase, IAclViewModel
    {
        public SpisAclViewModel()
        {

        }

        public async void LoadAcl(string id)
        {
            Busy = true;
            BusyContent = "Loading ACL";
            Id = id;

            List<Organization> orgs = new List<Organization>();
            List<IdentityDescriptionObject> acl = new List<IdentityDescriptionObject>();

            AllIdentities = new ObservableCollection<Organization>();
            AclList = new ObservableCollection<IdentityDescriptionObject>();

            await Task.Factory.StartNew(() =>
            {
                var SpisService = new SpisService();
                acl = SpisService.GetAcl(Id);
                orgs = SpisService.FindIdentties();
            });

            foreach (var i in acl)
                AclList.Add(i);

            foreach (var org in orgs)
            {
                if (acl.FirstOrDefault(x => x.IdentityId == org.Mrn) == null)
                {
                    AllIdentities.Add(org);
                }
            }

            Busy = false;
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

        private ObservableCollection<Organization> _allIdentities;
        public ObservableCollection<Organization> AllIdentities
        {
            get
            {
                return _allIdentities;
            }
            set
            {
                if (_allIdentities == value)
                    return;

                _allIdentities = value;

                OnPropertyChanged(() => AllIdentities);
            }
        }

        private Organization _selectedIdentity;
        public Organization SelectedIdentity
        {
            get
            {
                return _selectedIdentity;
            }
            set
            {
                if (_selectedIdentity == value)
                    return;

                _selectedIdentity = value;

                OnPropertyChanged(() => SelectedIdentity);
                AddAclCommand.RaiseCanExecuteChanged();
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
                DeleteAclCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _addAclCommand;
        public DelegateCommand AddAclCommand
        {
            get
            {
                return _addAclCommand ??
                    (_addAclCommand = new DelegateCommand(ExecuteAddAclCommand, CanExecuteAddAclCommand));
            }
        }

        public bool CanExecuteAddAclCommand(object parameter)
        {
            return SelectedIdentity != null;
        }

        public async void ExecuteAddAclCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Adding ACL";

            try
            {
                var identity = new IdentityDescriptionObject(SelectedIdentity.Mrn, SelectedIdentity.Name);

                await Task.Factory.StartNew(() =>
                {
                    var SpisService = new SpisService();
                    SpisService.AuthorizeIdentities(Id, new List<IdentityDescriptionObject>
                    {
                    identity
                    });
                });

                AclList.Add(identity);
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

        private DelegateCommand _deleteAclCommand;
        public DelegateCommand DeleteAclCommand
        {
            get
            {
                return _deleteAclCommand ??
                    (_deleteAclCommand = new DelegateCommand(ExecuteDeleteAclCommand, CanExecuteDeleteAclCommand));
            }
        }

        public bool CanExecuteDeleteAclCommand(object parameter)
        {
            return SelectedAcl != null;
        }

        public async void ExecuteDeleteAclCommand(object parameter)
        {
            Busy = true;
            BusyContent = "Deleting ACL";

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var SpisService = new SpisService();
                    SpisService.RemoveAuthorization(Id, new List<IdentityDescriptionObject>
                    {
                    SelectedAcl
                    });
                });

                AclList.Remove(SelectedAcl);
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