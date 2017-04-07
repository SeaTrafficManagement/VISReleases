using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.ViewModels
{
    public class DeleteSubscriptionDlgViewModel : ViewModelBase
    {
        public DeleteSubscriptionDlgViewModel()
        {

        }

        private string _uvid;
        public string Uvid
        {
            get
            {
                return _uvid;
            }
            set
            {
                if (_uvid == value)
                    return;
                _uvid = value;

                OnPropertyChanged(() => Uvid);
            }
        }

        private string _callbackEndpoint;
        public string CallbackEndpoint
        {
            get
            {
                return _callbackEndpoint;
            }
            set
            {
                if (_callbackEndpoint == value)
                    return;
                _callbackEndpoint = value;

                OnPropertyChanged(() => CallbackEndpoint);
            }
        }
    }
}
