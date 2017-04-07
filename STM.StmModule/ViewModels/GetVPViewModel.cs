using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.ViewModels
{
    public class GetVPViewModel : ViewModelBase
    {
        public GetVPViewModel()
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

        private string _routeStatus;
        public string RouteStatus
        {
            get
            {
                return _routeStatus;
            }
            set
            {
                if (_routeStatus == value)
                    return;
                _routeStatus = value;

                OnPropertyChanged(() => RouteStatus);
            }
        }
    }
}
