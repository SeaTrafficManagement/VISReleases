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
using STM.Common.DataAccess;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class StmModuleService : IStmModuleService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly StmDbContext _context;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="notificationService"></param>
        public StmModuleService(StmDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public bool Notify(Notification notification)
        {

            notification.ReceivedAt = DateTime.UtcNow;
            notification.NotificationCreatedAt = DateTime.UtcNow;

            var dbNotification = new DataAccess.Entities.Notification
            {
                Body = notification.Body ?? "Empty",
                FetchedByShip = false,
                FetchTime = null,
                FromOrgId = notification.FromOrgId,
                FromOrgName = notification.FromOrgName,
                FromServiceId = notification.FromServiceId,
                NotificationCreatedAt = notification.NotificationCreatedAt,
                NotificationType = (DataAccess.Entities.NotificationType)notification.NotificationType,
                NotificationSource = (DataAccess.Entities.NotificationSource)notification.NotificationSource,
                ReceivedAt = notification.ReceivedAt,
                Subject = notification.Subject
            };

            var nrOfNotifications = _notificationService.Get(XmlParsers => XmlParsers.FetchedByShip == false).Count();

            // Write notification to database
            _notificationService.Insert(dbNotification);
            _context.SaveChanges();

            try
            {
                // Try to push notification to client
                if (!string.IsNullOrEmpty(InstanceContext.StmModuleUrl))
                {
                    log.Debug("Trying to send notification to STM module on url: " + InstanceContext.StmModuleUrl);

                    notification.MessageWaiting = nrOfNotifications;
                    var response = WebRequestHelper.Post(InstanceContext.StmModuleUrl, notification.ToJson());
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        dbNotification.FetchedByShip = true;
                        dbNotification.FetchTime = DateTime.UtcNow;
                        _notificationService.Update(dbNotification);
                    }
                    else
                        return false;
                }
                else
                {
                    log.Debug("No push to STM Module becouse no url is configured for the instance");
                    return false;
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