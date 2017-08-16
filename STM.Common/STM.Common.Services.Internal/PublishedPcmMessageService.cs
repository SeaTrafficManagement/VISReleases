using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STM.Common;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using STM.Common.Exceptions;
using System.Web;
using STM.SSC.Internal.Models;
using Newtonsoft.Json;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using STM.Common.Services.Internal;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishedPcmMessageService : InternalServiceBase<PublishedPcmMessage, StmDbContext>, IPublishedPcmMessageService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string m_path = Path.Combine(Environment.CurrentDirectory, @"..\..\Schema");
        private IACLObjectService _aclService;
        private ISpisSubscriptionService _subscriberService;
        private ILogEventService _logEventService;
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logContext"></param>
        public PublishedPcmMessageService(StmDbContext dbContext,
            LogDbContext logContext) : base(dbContext)
        {
            _aclService = new ACLObjectService(dbContext);
            _subscriberService = new SpisSubscriptionService(dbContext);
            _logEventService = new LogEventService(logContext);
            _notificationService = new NotificationService(dbContext, logContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public override void Insert(PublishedPcmMessage entity)
        {
            SaveAndUpdate(entity, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public override void Update(PublishedPcmMessage entityToUpdate)
        {
            SaveAndUpdate(entityToUpdate, false);
        }

        private void SaveAndUpdate(PublishedPcmMessage entity, bool isNew)
        {
            try
            {
                Validate(entity);

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

        private void Validate(PublishedPcmMessage entity)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);

            var validator = new StmSchemaValidator();

            //validate the XML against its XSD
            validator.ValidatePCMMessageXML(msg);
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
                // If no queueId setup new queue
                if (string.IsNullOrEmpty(subscription.QueueId))
                {
                    var queueId = SetupQueue(message, dataId, subscription.MbEndpoint, subscription.SubscriberIdentity);
                    subscription.QueueId = queueId;

                    _subscriberService.Update(subscription);
                }

                // Send PCM to AMSS service
                SendToAmssService(message, dataId, subscription.AmssEndpoint, subscription.SubscriberIdentity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataId"></param>
        /// <param name="mbEndpoint"></param>
        /// <param name="amssEndpoint"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public string SendMessage(string message,
            string dataId,
            string mbEndpoint,
            string amssEndpoint,
            Identity identity)
        {
            var queueId = SetupQueue(message, dataId, mbEndpoint, identity);

            if (message != null)
                SendToAmssService(message, dataId, amssEndpoint, identity);

            return queueId;
        }

        private void SendToAmssService(string message,
            string dataId,
            string amssEndpoint,
            Identity identity)
        {
            var paramList = new List<KeyValuePair<string, string>>();
            var p1 = new KeyValuePair<string, string>("dataId", dataId);
            paramList.Add(p1);
            p1 = new KeyValuePair<string, string>("amssEndpoint", amssEndpoint);
            paramList.Add(p1);
            var requestObj = new CallServiceRequestObj
            {
                Body = message,
                EndpointMethod = WebRequestHelper.CombineUrl(amssEndpoint, "/amss/state_update"),
                Headers = null,
                RequestType = "POST",
            };
            try
            {
                var headers = new List<Header>
                {
                    new Header("content-type", "application/xml")
                };

                requestObj.Headers = headers;

                _logEventService.LogInfo(EventNumber.SPIS_amss_request, EventDataType.PCM, paramList, 
                    JsonConvert.SerializeObject(requestObj, Newtonsoft.Json.Formatting.Indented));

                var client = new SSC.Internal.SccPrivateService();
                var queueResult = client.CallService(requestObj);

                if (queueResult.StatusCode < 200 || queueResult.StatusCode >= 300)
                {
                    throw new Exception("Unable to send state update. " + queueResult.StatusCode + " " + queueResult.Body);
                }

                _logEventService.LogSuccess(EventNumber.SPIS_amss_response, EventDataType.Other, paramList, JsonConvert.SerializeObject(queueResult, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.SPIS_amss_request, EventType.Error_internal, 
                    paramList, JsonConvert.SerializeObject(requestObj, Newtonsoft.Json.Formatting.Indented));

                // Send notification
                var notification = new Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.ERROR_MESSAGE;
                notification.Subject = "Unable to send message";
                notification.Body = string.Format("Unable to send message with id {0} to identity {1}, {2}. {3}", dataId, identity.Name, identity.UID, ex.Message);
                notification.NotificationSource = EnumNotificationSource.SPIS;
                _notificationService.Notify(notification);
            }
        }

        private string SetupQueue(string message,
            string dataId,
            string mbEndpoint,
            Identity identity)
        {
            string result = null;

            string vesselId = ConfigurationManager.AppSettings["vesselId"];
            if (!string.IsNullOrEmpty(InstanceContext.IMO))
            {
                vesselId += "IMO:" + InstanceContext.IMO;
            }
            else if (!string.IsNullOrEmpty(InstanceContext.MMSI))
            {
                vesselId += "MMSI:" + InstanceContext.MMSI;
            }
            else
            {
                throw new Exception("Failed to find vessel identifier.");
            }

            try
            {
                // Setup filter
                var queueFilter = new List<QueueFilter>
                {
                    new QueueFilter
                        {
                            type="VESSEL", element=vesselId
                        }
                };

                var headers = new List<Header>
                {
                    new Header("content-type", "application/json; charset=utf-8")
                };

                var client = new SSC.Internal.SccPrivateService();
                var queueResult = client.CallService(new CallServiceRequestObj
                {
                    Body = JsonConvert.SerializeObject(queueFilter),
                    EndpointMethod = WebRequestHelper.CombineUrl(mbEndpoint, "/mb/mqs"),
                    Headers = headers,
                    RequestType = "POST",
                });

                if (queueResult.StatusCode == 200 || queueResult.StatusCode == 201)
                {
                    result = queueResult.Body;
                }
                else
                {
                    throw new Exception("Unable to create queue. " + queueResult.StatusCode + " " + queueResult.Body);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                // Send notification
                var notification = new Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.ERROR_MESSAGE;
                notification.NotificationSource = EnumNotificationSource.SPIS;
                notification.Subject = "Unable to send set up message broker queue";
                notification.Body = string.Format("Unable to create queue for message with id {0}, identity {1}, {2}", dataId, identity.Name, identity.UID);
                _notificationService.Notify(notification);
            }

            return result;
        }

    }
}