using STM.StmModule.Simulator.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.Infrastructure
{
    public class NotificationHandler
    {
        private static NotificationHandler _instance;

        public event EventHandler<Notification> NewNotification ;

        public static NotificationHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotificationHandler();

                return _instance;
            }
        }

        public void RaiseNewNotification(Notification notification)
        {
            NewNotification?.Invoke(this, notification);
        }
    }
}