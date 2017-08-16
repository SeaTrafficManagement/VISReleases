using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using STM.Common;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using STM.Common.Exceptions;
using STM.Common.Services.Internal.Interfaces;
using System.Web;
using STM.SSC.Internal.Models;
using STM.Common.XmlParsers;
using Newtonsoft.Json;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishedRtzMessageService : InternalServiceBase<PublishedRtzMessage, StmDbContext>, IPublishedRtzMessageService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string m_path = Path.Combine(Environment.CurrentDirectory, @"..\..\Schema");
        private IACLObjectService _aclService;
        private IVisSubscriptionService _subscriberService;
        private ILogEventService _logEventService;
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logContext"></param>
        public PublishedRtzMessageService(StmDbContext dbContext, 
            LogDbContext logContext) : base(dbContext)
        {
            _aclService = new ACLObjectService(dbContext);
            _subscriberService = new VisSubscriptionService(dbContext);
            _logEventService = new LogEventService(logContext);
            _notificationService = new NotificationService(dbContext, logContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public override void Insert(PublishedRtzMessage entity)
        {
            SaveAndUpdate(entity, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public override void Update(PublishedRtzMessage entityToUpdate)
        {
            SaveAndUpdate(entityToUpdate, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isNew"></param>
        /// <param name="ignoreStatusInMessage"></param>
        public void SaveAndUpdate(PublishedRtzMessage entity, bool isNew, bool ignoreStatusInMessage = false)
        {
            try
            {
                Validate(entity);
                Populate(entity, ignoreStatusInMessage);

                if (isNew)
                {
                    base.Insert(entity);
                }
                else
                {
                    base.Update(entity);
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (StmSchemaValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Populate(PublishedRtzMessage entity, bool ignoreStatusInMessage = false)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);

            var parser = RtzParserFactory.Create(msg);
            var routeStatusString = parser.RouteStatus;

            RouteStatus routeStatus = RouteStatus.Unknown;
            int statusId;
            if (int.TryParse(routeStatusString, out statusId))
            {
                routeStatus = (RouteStatus)statusId;
            }

            if (!ignoreStatusInMessage)
            {
                entity.MessageStatus = (RouteStatus)statusId;
            }

            DateTime? validityPeriodStart = parser.ValidityPeriodStart;
            if (validityPeriodStart == DateTime.MinValue)
            {
                entity.MessageValidFrom = null;
            }
            else
            {
                entity.MessageValidFrom = validityPeriodStart;
            }
            DateTime? validityPeriodStop = parser.ValidityPeriodStop;
            if (validityPeriodStop == DateTime.MinValue)
            {
                entity.MessageValidTo = null;
            }
            else
            {
                entity.MessageValidTo = validityPeriodStop;
            }
        }

        private void Validate(PublishedRtzMessage entity)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);
            
            var validator = new StmSchemaValidator();

            //validate the XML against its XSD
            validator.ValidateRTZMessageXML(msg);

            //validate the message content according to our business rules
            validator.ValidateRTZMessage(msg);

            //Validate UVID between header and RTZ
            validator.ValidateUVID(entity.MessageID, msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataId"></param>
        public void SendMessageToSubsribers(string message, string dataId)
        {
            //call proxy class to SSC -> callservice for each subscriber 
            var subscriptions = _subscriberService.Get(x => 
                x.MessageID == dataId && x.IsAuthorized, includeProperties: "SubscriberIdentity");

            if (subscriptions == null)
                return;

            foreach (var subscription in subscriptions)
            {
                SendMessage(message, dataId, subscription.CallbackEndpoint, subscription.SubscriberIdentity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataId"></param>
        /// <param name="endpoint"></param>
        /// <param name="identity"></param>
        public void SendMessage(string message, string dataId, string endpoint, Identity identity)
        {
            var client = new SSC.Internal.SccPrivateService();

            string msg = string.Format("Voyageplan with uvid {0} sent to subscriber {1}", dataId, identity.Name);

            //Add event logging
            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("Message", msg);
            paramList.Add(param);

            try
            {
                var result = client.CallService(new CallServiceRequestObj
                {
                    Body = message,
                    EndpointMethod = WebRequestHelper.CombineUrl(endpoint, "voyagePlans?uvid=" + dataId),
                    Headers = new List<Header>
                    {
                        new Header("content-type", "text/xml; charset=utf-8")
                    },
                    RequestType = "POST",
                });

                if (result.StatusCode != 200)
                {
                    throw new Exception(result.Body);
                }

                log.Info(msg);

                _logEventService.LogSuccess(EventNumber.VIS_publishMessage, EventDataType.RTZ, paramList, message);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                //logg event
                _logEventService.LogError(EventNumber.VIS_publishMessage, EventType.Error_internal, paramList, ex.Message);

                // Send notification
                var notification = new Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.ERROR_MESSAGE;
                notification.Subject = "Unable to send message to subscriber";
                notification.Body = string.Format("Voyageplan with UVID {0} could not be sent to subscriber {1}, {2}", dataId, identity.Name, identity.UID);
                notification.NotificationSource = EnumNotificationSource.VIS;
                _notificationService.Notify(notification);
            }
        }
    }
}