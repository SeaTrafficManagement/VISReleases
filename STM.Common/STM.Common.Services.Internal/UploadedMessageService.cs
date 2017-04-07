using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STM.Common.DataAccess;
using STM.Common;
using STM.Common.Services.Internal.Interfaces;
using Newtonsoft.Json;
using STM.SSC.Internal.Models;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadedMessageService : InternalServiceBase<UploadedMessage, StmDbContext>, IUploadedMessageService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="notificationService"></param>
        public UploadedMessageService(StmDbContext dbContext,
            INotificationService notificationService) : base(dbContext)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void InsertRTZ(UploadedMessage entity)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);
            var validator = new StmSchemaValidator();

            //validate the XML against its XSD
            validator.ValidateRTZMessageXML(msg);

            //validate the message content according to our business rules
            validator.ValidateRTZMessage(msg);

            //Validate UVID between header and RTZ
            validator.ValidateUVID(entity.MessageID, msg);

            base.Insert(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void InsertTXT(UploadedMessage entity)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);
            var validator = new StmSchemaValidator();
            validator.ValidateTextMessageXML(msg);

            base.Insert(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void InsertArea(UploadedMessage entity)
        {
            var msg = Serialization.ByteArrayToString(entity.Message);
            var validator = new StmSchemaValidator();
            validator.ValidateAreaMessageXML(msg);

            base.Insert(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void InsertPCM(UploadedMessage message)
        {
            var msg = Serialization.ByteArrayToString(message.Message);
            var validator = new StmSchemaValidator();
            validator.ValidatePCMMessageXML(msg);

            base.Insert(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfMessages"></param>
        /// <param name="mTypes"></param>
        /// <returns></returns>
        public List<UploadedMessage> GetMessagesByLimitNumber(int numberOfMessages, List<MessageType> mTypes)
        {
            List<UploadedMessage> result = new List<UploadedMessage>();
            var noMessagesLeft = numberOfMessages;
            foreach(var mType in mTypes)
            {
                var messages = base._context.UploadedMessage.Include("FromOrg").Include("MessageType").OrderByDescending(
                    x => x.ReceiveTime).Where(x => x.Notified && !x.FetchedByShip && x.MessageType.ID == mType.ID).
                    Take(noMessagesLeft).ToList();
                noMessagesLeft -= messages.Count();
                foreach(var msg in messages)
                {
                    result.Add(msg);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mTypes"></param>
        /// <returns></returns>
        public List<UploadedMessage> GetAllUnFetchedMessages(List<MessageType> mTypes)
        {
            List<UploadedMessage> result = new List<UploadedMessage>();

            foreach(var mType in mTypes)
            {
                var messages = base._context.UploadedMessage.Include("FromOrg").Include("MessageType").
                    OrderByDescending(x => x.ReceiveTime).Where(x => x.Notified && 
                    !x.FetchedByShip && 
                    x.MessageType.ID == mType.ID).ToList();
                foreach (var msg in messages)
                {
                    result.Add(msg);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mTypes"></param>
        /// <returns></returns>
        public List<UploadedMessage> GetUnFetchedMessagesById(string Id, List<MessageType> mTypes)
        {
            List<UploadedMessage> result = new List<UploadedMessage>();

            foreach (var mType in mTypes)
            {
                var messages = base._context.UploadedMessage.Include("FromOrg").Include("MessageType").
                    OrderByDescending(x => x.ReceiveTime).Where(x => x.Notified && 
                    !x.FetchedByShip && 
                    x.MessageID == Id && 
                    x.MessageType.ID == mType.ID).ToList();
                foreach(var msg in messages)
                {
                    result.Add(msg);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mTypes"></param>
        /// <returns></returns>
        public int GetNumberOfRemainingMessages(List<MessageType> mTypes)
        {
            int result = 0;

            foreach (var mType in mTypes)
            {
                var messages = base.Get(x => x.Notified && 
                !x.FetchedByShip && 
                x.MessageType.ID == mType.ID).ToList();
                result += messages.Count();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void SendAck(IList<UploadedMessage> messages)
        {
            var client = new STM.SSC.Internal.SccPrivateService();

            foreach (var message in messages)
            {
                try
                {
                    if (message.DeliveryAckReqested && !message.AckDelivered)
                    {
                        var ackObject = new DeliveryAck
                        {
                            AckResult = "OK",
                            FromId = message.FromOrg.UID,
                            FromName = message.FromOrg.Name,
                            Id =  Guid.NewGuid().ToString(),
                            ReferenceId = message.MessageID,
                            TimeOfDelivery = DateTime.UtcNow,
                            ToId = InstanceContext.ServiceId,
                            ToName = InstanceContext.ServiceName
                        };
                        client.CallService(new CallServiceRequestObj
                        {
                            Body = JsonConvert.SerializeObject(ackObject),
                            EndpointMethod = WebRequestHelper.CombineUrl(message.DeliveryAckEndpoint, "acknowledgement"),
                            Headers = new List<Header>
                            {
                                new Header("content-type", "application/json; charset=UTF8;charset=UTF8")
                            },
                            RequestType = "POST",
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);

                    // Send notification
                    var notification = new Internal.Interfaces.Notification();
                    notification.FromOrgName = message.FromOrg.Name;
                    notification.FromOrgId = message.FromOrg.UID;
                    notification.FromServiceId = message.FromServiceId;
                    notification.NotificationType = EnumNotificationType.ERROR_MESSAGE;
                    notification.Subject = "Unable to send acknowledgement";
                    notification.Body = string.Format("Acknowledgement for voyageplan with UVID {0} could not be sent to {1}, {2}", message.MessageID, message.FromOrg.Name, message.FromOrg.UID);
                    notification.NotificationSource = EnumNotificationSource.VIS;
                    _notificationService.Notify(notification);
                }
            }
        }
    }
}
