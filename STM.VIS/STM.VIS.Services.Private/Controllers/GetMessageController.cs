using System.Threading.Tasks;
using STM.Common.DataAccess.Entities;
using STM.Common.Services.Internal;
using STM.Common.DataAccess;
using System.ComponentModel.DataAnnotations;
using STM.VIS.Services.Private.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using Microsoft.Practices.Unity;
using System.Data.Entity;
using System;
using STM.Common;
using System.Configuration;
using STM.Common.Services;
using STM.Common.Services.Internal.Interfaces;
using System.Threading;
using System.Web.Http.Controllers;
using System.Linq;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class GetMessageController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUploadedMessageService _uploadedMessageService;
        private IMessageTypeService _messageTypeService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadedMessageService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public GetMessageController(IUploadedMessageService uploadedMessageService, IMessageTypeService messageTypeService,
            IConnectionInformationService connectionInformationService)
        {
            _uploadedMessageService = uploadedMessageService;
            _messageTypeService = messageTypeService;
            _connectionInformationService = connectionInformationService;
        }

        /// <summary>
        /// Retrieve received messages from VIS database
        /// </summary>
        /// <param name="limitQuery">Limit the number of messages to be received</param>
        /// <response code="200">Success</response>
        /// <response code="default">Unexpected error</response>
        [HttpGet]
        [Route("getMessage")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public MessageEnvelope GetMessage(string limitQuery = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            int limitNumberOfMessages = 0;
            if (int.TryParse(limitQuery, out limitNumberOfMessages))
            {
                log.Info(string.Format("LimitQuery parsed successfully to {0}.", limitNumberOfMessages));
            }

            try
            {
                var result = new MessageEnvelope();
                result.Messages = new List<Message>();
                var uploadedMessages = new List<UploadedMessage>();
                var mTypes = _messageTypeService.Get(m => m.Name != "PCM").ToList();

                if (limitNumberOfMessages > 0)
                {
                    uploadedMessages = _uploadedMessageService.GetMessagesByLimitNumber(limitNumberOfMessages, mTypes);
                }
                else
                {
                    uploadedMessages = _uploadedMessageService.GetAllUnFetchedMessages(mTypes);
                }

                foreach (var uploadedMessage in uploadedMessages)
                {
                    uploadedMessage.FetchedByShip = true;
                    uploadedMessage.FetchTime = DateTime.UtcNow;
                    _uploadedMessageService.Update(uploadedMessage);

                    var messageToAdd = new Message();
                    messageToAdd.Id = uploadedMessage.MessageID;
                    messageToAdd.FromOrgId = uploadedMessage.FromOrg.UID;
                    messageToAdd.FromOrgName = uploadedMessage.FromOrg.Name;
                    messageToAdd.FromServiceId = uploadedMessage.FromServiceId;
                    messageToAdd.MessageType = uploadedMessage.MessageType.Name;
                    messageToAdd.ReceivedAt = uploadedMessage.ReceiveTime;

                    if (messageToAdd.ReceivedAt.HasValue)
                        messageToAdd.ReceivedAt = DateTime.SpecifyKind(messageToAdd.ReceivedAt.Value, DateTimeKind.Utc);

                    var messageBody = Serialization.ByteArrayToString(uploadedMessage.Message);
                    messageToAdd.StmMessage = new StmMessage(messageBody);
                    messageToAdd.CallbackEndpoint = uploadedMessage.CallbackEndpoint;

                    result.Messages.Add(messageToAdd);
                }
                _context.SaveChanges();

                result.NumberOfMessages = uploadedMessages.Count;
                result.RemainingNumberOfMessages = _uploadedMessageService.GetNumberOfRemainingMessages(mTypes);

                _uploadedMessageService.SendAck(uploadedMessages);

                //set last interaction time
                var conInfo = _connectionInformationService.Get().FirstOrDefault();
                if (conInfo == null)
                    _connectionInformationService.Insert(new ConnectionInformation { LastInteraction = DateTime.UtcNow });
                else
                {
                    conInfo.LastInteraction = DateTime.UtcNow;
                    _connectionInformationService.Update(conInfo);
                }

                _context.SaveChanges();
                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);

                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// Retrieve messages received in date intervall
        /// </summary>
        /// <param name="fromDate">Retreive message received after this date</param>
        /// <param name="toDate">Retreive message received before this date. If null all messages up to current date will be fetched</param>
        /// <response code="200">Success</response>
        /// <response code="default">Unexpected error</response>
        [HttpGet]
        [Route("getMessageInTimeIntervall")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public MessageEnvelope GetMessageInTimeIntervall(DateTime fromDate, DateTime? toDate = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            try
            {
                if (toDate == null)
                    toDate = DateTime.MaxValue;

                var result = new MessageEnvelope();
                result.Messages = new List<Message>();
                var uploadedMessages = new List<UploadedMessage>();
                var mTypes = _messageTypeService.Get(m => m.Name != "PCM").ToList();

                uploadedMessages = _uploadedMessageService.GetMessagesInTimeIntervall(fromDate, toDate.Value, mTypes);

                foreach (var uploadedMessage in uploadedMessages)
                {
                    var messageToAdd = new Message();
                    messageToAdd.Id = uploadedMessage.MessageID;
                    messageToAdd.FromOrgId = uploadedMessage.FromOrg.UID;
                    messageToAdd.FromOrgName = uploadedMessage.FromOrg.Name;
                    messageToAdd.FromServiceId = uploadedMessage.FromServiceId;
                    messageToAdd.MessageType = uploadedMessage.MessageType.Name;
                    messageToAdd.ReceivedAt = uploadedMessage.ReceiveTime;

                    if (messageToAdd.ReceivedAt.HasValue)
                        messageToAdd.ReceivedAt = DateTime.SpecifyKind(messageToAdd.ReceivedAt.Value, DateTimeKind.Utc);

                    var messageBody = Serialization.ByteArrayToString(uploadedMessage.Message);
                    messageToAdd.StmMessage = new StmMessage(messageBody);
                    messageToAdd.CallbackEndpoint = uploadedMessage.CallbackEndpoint;

                    result.Messages.Add(messageToAdd);
                }

                result.NumberOfMessages = uploadedMessages.Count;
                result.RemainingNumberOfMessages = _uploadedMessageService.GetNumberOfRemainingMessages(mTypes);

                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}