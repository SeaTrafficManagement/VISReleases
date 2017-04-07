﻿using Microsoft.Practices.Unity;
using STM.SSC.Internal;
using STM.VIS.Services.Private.Models;
using STM.Common.DataAccess;
using STM.Common.Services;
using STM.Common.Services.Internal;
using STM.Common.Services.Internal.Interfaces;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;


namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class NotificationController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private INotificationService _notificationService;
        private IConnectionInformationService _connectionInformationService;
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public NotificationController(INotificationService notificationService, IConnectionInformationService connectionInformationService)
        {
            _notificationService = notificationService;
            _connectionInformationService = connectionInformationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Get all awaiting notifications</remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpGet]
        [Route("getNotification")]
        [SwaggerOperation("GetNotification")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerResponse(200, type: typeof(List<Notification>))]
        public List<Notification> GetNotification()
        {
            // log.Info("Incoming request to " + GetCurrentMethod());

            List<Notification> result = new List<Notification>();
            try
            {
                var notifications = _notificationService.Get(x => x.FetchedByShip == false && x.NotificationSource == Common.DataAccess.Entities.NotificationSource.VIS);

                if (notifications != null)
                {
                    foreach (var notification in notifications)
                    {
                        var receivedAt = DateTime.SpecifyKind(notification.ReceivedAt, DateTimeKind.Utc);
                        var NotificationCreatedAt = DateTime.SpecifyKind(notification.NotificationCreatedAt, DateTimeKind.Utc);

                        result.Add(new Notification
                        {
                            Body = notification.Body,
                            FromOrgId = notification.FromOrgId,
                            FromOrgName = notification.FromOrgName,
                            FromServiceId = notification.FromServiceId,
                            MessageWaiting = 0,
                            NotificationCreatedAt = NotificationCreatedAt,
                            NotificationType = (EnumNotificationType)notification.NotificationType,
                            ReceivedAt = receivedAt,

                            Subject = notification.Subject,
                            NotificationSource = EnumNotificationSource.VIS
                        });

                        notification.FetchedByShip = true;
                        notification.FetchTime = DateTime.UtcNow;
                        _notificationService.Update(notification);
                    }
                }

                //set last interaction time
                var conInfo = _connectionInformationService.Get().FirstOrDefault();
                if (conInfo == null)
                    _connectionInformationService.Insert(new STM.Common.DataAccess.Entities.ConnectionInformation
                    { LastInteraction = DateTime.UtcNow });
                else
                {
                    conInfo.LastInteraction = DateTime.UtcNow;
                    _connectionInformationService.Update(conInfo);
                }

                _context.SaveChanges();
                return result;
            }
            catch(Exception ex)
            {
                logger.Debug(ex);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}