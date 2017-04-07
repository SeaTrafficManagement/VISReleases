using STM.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using STM.Common.Services.Internal.Interfaces;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class StmModuleService : IStmModuleService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _stmModuleUrl;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationService"></param>
        public StmModuleService(INotificationService notificationService)
        {
            _stmModuleUrl = ConfigurationManager.AppSettings["StmModuleUrl"];
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public bool Notify(Notification notification)
        {
            var dbNotification = new DataAccess.Entities.Notification
            {
                Body = notification.Body ?? "Empty",
                FetchedByShip = false,
                FetchTime = null,
                FromOrgId = notification.FromOrgId,
                FromOrgName = notification.FromOrgName,
                FromServiceId = notification.FromServiceId,
                NotificationCreatedAt = DateTime.UtcNow,
                NotificationType = (DataAccess.Entities.NotificationType)notification.NotificationType,
                NotificationSource = (DataAccess.Entities.NotificationSource)notification.NotificationSource,
                ReceivedAt = DateTime.UtcNow,
                Subject = notification.Subject
            };

            var nrOfNotifications = _notificationService.Get().Count();

            // Write notification to database
            _notificationService.Insert(dbNotification);

            try
            {
                // Try to push notification to client
                if (!string.IsNullOrEmpty(_stmModuleUrl))
                {
                    var url = _stmModuleUrl + "/api/StmModulePublic/Notify";

                    notification.MessageWaiting = nrOfNotifications;
                    var response = WebRequestHelper.Post(url, notification.ToJson());
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        dbNotification.FetchedByShip = true;
                        dbNotification.FetchTime = DateTime.UtcNow;
                        _notificationService.Update(dbNotification);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }

            return true;
        }
    }
}