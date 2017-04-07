using System.Collections.ObjectModel;
using STM.StmModule.Simulator.Contract;

namespace STM.StmModule.Simulator.ViewModels
{
    public interface IAclViewModel
    {
        ObservableCollection<IdentityDescriptionObject> AclList { get; set; }
        DelegateCommand AddAclCommand { get; }
        ObservableCollection<Organization> AllIdentities { get; set; }
        DelegateCommand DeleteAclCommand { get; }
        string Id { get; set; }
        IdentityDescriptionObject SelectedAcl { get; set; }
        Organization SelectedIdentity { get; set; }

        bool CanExecuteAddAclCommand(object parameter);
        bool CanExecuteDeleteAclCommand(object parameter);
        void ExecuteAddAclCommand(object parameter);
        void ExecuteDeleteAclCommand(object parameter);
        void LoadAcl(string id);
    }
}