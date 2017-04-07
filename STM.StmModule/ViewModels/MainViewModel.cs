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
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace STM.StmModule.Simulator.ViewModels
{
    public sealed class MainViewModel : ViewModelBase, IDisposable
    {
        private Timer _notificationTimer;
        public MainViewModel()
        {
            NotificationHandler.Instance.NewNotification += Instance_NewNotification;

            _notificationTimer = new Timer(10000);
            _notificationTimer.AutoReset = true;
            _notificationTimer.Elapsed += _notificationTimer_Elapsed;
            _notificationTimer.Start();
        }

        private void _notificationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var visService = new VisService();
            var spisService = new SpisService();
            List<Notification> notifications = new List<Notification>();

            var visNotifications = visService.GetNotifications();
            if (visNotifications != null)
                notifications.AddRange(visNotifications);

            var spisNotifications = spisService.GetNotifications();
            if (spisNotifications != null)
                notifications.AddRange(spisNotifications);

            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    Instance_NewNotification(sender, notification);
                }
            }
        }

        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        public ObservableCollection<Notification> Notifications
        {
            get
            {
                return _notifications;
            }
            set
            {
                if (_notifications == value)
                    return;

                _notifications = value;

                OnPropertyChanged(() => Notifications);
            }
        }

        private Notification _selectedNotification;
        public Notification SelectedNotification
        {
            get
            {
                return _selectedNotification;
            }
            set
            {
                if (_selectedNotification == value)
                    return;

                _selectedNotification = value;

                OnPropertyChanged(() => SelectedNotification);
            }
        }

        private void Instance_NewNotification(object sender, Notification e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { Notifications.Add(e); }));
        }

        public void Dispose()
        {
            _notificationTimer.Dispose();
            Dispose();

        }
    }
}
