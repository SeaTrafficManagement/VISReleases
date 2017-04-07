using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.ViewModels
{
    public class SubscriptionDlgViewModel : ViewModelBase
    {
        public void SubscriptionDlViewModel()
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
    }
}
