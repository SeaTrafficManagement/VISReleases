using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STM.StmModule.Simulator.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private int _zoomLevel = 5;
        public int ZoomLevel
        {
            get
            {
                return _zoomLevel;
            }
            set
            {
                if (_zoomLevel == value)
                    return;
                _zoomLevel = value;
                OnPropertyChanged(() => ZoomLevel);
            }
        }

        private Location _center = new Location(60, 19);
        public Location Center
        {
            get
            {
                return _center;
            }
            set
            {
                if (_center == value)
                    return;
                _center = value;
                OnPropertyChanged(() => Center);
            }
        }

        private BackgroundMapEnum _backgroundMap = BackgroundMapEnum.BingAreal;
        public BackgroundMapEnum BackgroundMap
        {
            get
            {
                return _backgroundMap;
            }
            set
            {
                if (_backgroundMap == value)
                    return;

                _backgroundMap = value;
                OnPropertyChanged(() => BackgroundMap);
            }
        }

        #region Commands
        private ICommand _zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                return _zoomInCommand ??
                    (_zoomInCommand = new DelegateCommand(ExecuteZoomInCommand, CanExecuteZoomInCommand));
            }
        }

        public bool CanExecuteZoomInCommand(object parameter)
        {
            return true;
        }

        public void ExecuteZoomInCommand(object parameter)
        {
            ZoomLevel += 1;
        }

        private ICommand _zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                return _zoomOutCommand ??
                    (_zoomOutCommand = new DelegateCommand(ExecuteZoomOutCommand, CanExecuteZoomOutCommand));
            }
        }

        public bool CanExecuteZoomOutCommand(object parameter)
        {
            return true;
        }

        public void ExecuteZoomOutCommand(object parameter)
        {
            ZoomLevel -= 1;
        }




        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ??
                    (_clearCommand = new DelegateCommand(ExecuteClearCommand, CanExecuteClearCommand));
            }
        }

        public bool CanExecuteClearCommand(object parameter)
        {
            return true;
        }

        public void ExecuteClearCommand(object parameter)
        {
            MapRoutes.CliearRoutes();
        }

        #endregion
    }
}
