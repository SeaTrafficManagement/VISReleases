using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Tools.InstanceConfigurator.ViewModels
{
    public class CreateDatabaseDialogViewModel : ViewModelBase
    {
        private string _dbName;
        public string DbName
        {
            get
            {
                return _dbName;
            }
            set
            {
                if (_dbName == value)
                    return;

                _dbName = value;

                OnPropertyChanged(() => DbName);
            }
        }
    }
}