using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using STM.Common.DataAccess;
using STM.Common.Services.Internal;
using STM.SPIS.Services.Private.Models;
using System.Data.Entity;
using STM.Common.DataAccess.Entities;
using System.Net;
using STM.Common;
using System.Globalization;
using Microsoft.Practices.Unity;
using System.Net.Http;
using System.Configuration;
using STM.Common.Services;
using STM.Common.Services.Internal.Interfaces;
using Newtonsoft.Json;

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class PublishMessageController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPublishedPcmMessageService _publishedMessageService;
        private IMessageTypeService _messageTypeService;
        private IACLObjectService _aclObjectService;
        private ISpisSubscriptionService _spisSubscriptionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedMessageService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="aclObjectService"></param>
        /// <param name="spisSubscriptionService"></param>
        [InjectionConstructor]
        public PublishMessageController(IPublishedPcmMessageService publishedMessageService, 
            IMessageTypeService messageTypeService,
            IACLObjectService aclObjectService,
            ISpisSubscriptionService spisSubscriptionService)
        {
            _publishedMessageService = publishedMessageService;
            _messageTypeService = messageTypeService;
            _aclObjectService = aclObjectService;
            _spisSubscriptionService = spisSubscriptionService;
        }

        /// <summary>
        /// Get all currently published messages
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        /// <returns></returns>
        /// 
        [HttpGet]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [Route("getPublishedMessages")]
        public List<PublishedMessageContract> GetPublishedMessages()
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            try
            {
                var result = new List<PublishedMessageContract>();
                var messages = _publishedMessageService.Get();

                foreach (var message in messages)
                {
                    result.Add(new PublishedMessageContract
                    {
                        Message = System.Text.Encoding.Default.GetString(message.Message),
                        MessageID = message.MessageID,
                        MessageLastUpdateTime = message.MessageLastUpdateTime,
                        MessageType = message.MessageType.Name,
                        MessageValidFrom = message.MessageValidFrom,
                        MessageValidTo = message.MessageValidTo,
                        PublishTime = message.PublishTime
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "SPIS internal server error. " + ex.Message;

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// Remove published message
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        [Route("publishedMessage")]
        public ResponseObj RemovePublishedMessage([FromUri] string dataId)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (string.IsNullOrEmpty(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "DataID is requeird");
            }

            try
            {
                var message = _publishedMessageService.Get(x =>
                    x.MessageID == dataId).FirstOrDefault();

                if (message == null)
                {
                    var msg = string.Format("No message found with id {0}.", dataId);
                    log.Info(msg);
                    throw CreateHttpResponseException(HttpStatusCode.NotFound, msg);
                }
                else
                {
                    // Delete subscriptions
                    var subscriptions = _spisSubscriptionService.Get(x => x.MessageID == dataId);
                    if (subscriptions != null)
                    {
                        foreach (var subscription in subscriptions)
                        {
                            _spisSubscriptionService.Delete(subscription);
                        }
                    }

                    // Delete ACL
                    var acls = _aclObjectService.Get(x => x.MessageID == dataId);
                    if (acls != null)
                    {
                        foreach (var acl in acls)
                        {
                            _aclObjectService.Delete(acl);
                        }
                    }

                    // Delete message
                    _publishedMessageService.Delete(message);
                    var msg = string.Format("Published message with id {0} was removed.", dataId);
                    log.Info(msg);

                    _context.SaveChanges();
                    return new ResponseObj(dataId);
                }
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
        /// Publish message to SPIS database for subsequent sending to subscribers
        /// </summary>
        /// <param name="dataId">data Id for published message</param>
        /// <param name="messageType">Message type for published message (PCM)</param>
        /// <param name="message">Typically STM payload data (PCM)</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        [Route("publishMessage")]
        public ResponseObj PublishMessage([FromUri]string dataId,
            [FromUri]string messageType,
            [FromBody]string message)
        {
            log.Info("Incoming request to " + GetCurrentMethod());
            //_logEventService.Init(EventDataType.PCM);

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("dataId", dataId);
            paramList.Add(param);
            param = new KeyValuePair<string, string>("messageType", messageType);
            paramList.Add(param);

            _logEventService.LogInfo(EventNumber.SPIS_stateUpdate_request, EventDataType.PCM, paramList, message);
            try
            {
                // Check incoming args
                if (string.IsNullOrEmpty(dataId))
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Missing required parameter dataId."));
                }
                if(string.IsNullOrEmpty(messageType))
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Missing required parameter messageType."));
                }
                if (string.IsNullOrEmpty(message))
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Missing required parameter message."));
                }

                // Check if correct messageType e.g. only PCM accepted
                var msgType = _messageTypeService.Get(m => m.Name == messageType).FirstOrDefault();
                if (msgType == null 
                    || string.IsNullOrEmpty(msgType.Name) 
                    || msgType.Name.ToLower() != "pcm")
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Unsupported or unknown message type."));
                }

                var messageToUpdate = _publishedMessageService.Get(m => m.MessageID == dataId).FirstOrDefault();

                var responsObj = new ResponseObj(dataId);

                if (messageToUpdate == null) //New message
                {
                    var newMessage = new PublishedPcmMessage();
                    newMessage.Message = Serialization.StrToByteArray(message);
                    newMessage.MessageID = dataId;
                    newMessage.MessageType = msgType;
                    newMessage.PublishTime = DateTime.UtcNow;

                    _publishedMessageService.Insert(newMessage);
                }
                else
                {
                    messageToUpdate.Message = Serialization.StrToByteArray(message);
                    messageToUpdate.MessageLastUpdateTime = DateTime.UtcNow;
                    _publishedMessageService.Update(messageToUpdate);
                }

                _publishedMessageService.SendMessageToSubsribers(message, dataId);

                // Save to DB
                _context.SaveChanges();
                var responseString = JsonConvert.SerializeObject(responsObj, Formatting.Indented);
                _logEventService.LogSuccess(EventNumber.VIS_publishMessage, EventDataType.None, null, responseString);
                return responsObj;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                
                _logEventService.LogError(EventNumber.SPIS_stateUpdate_request, EventType.Error_internal,
                    paramList, ex.Response.ToString());

                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                var errorMessage = "SPIS internal server error. " + ex.Message;
                _logEventService.LogError(EventNumber.SPIS_stateUpdate_request, EventType.Error_internal,
                    paramList, ex.Message);
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, errorMessage);
            }
        }
    }
}