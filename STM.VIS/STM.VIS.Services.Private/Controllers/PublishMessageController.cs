using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using STM.Common.DataAccess;
using STM.Common.Services.Internal;
using STM.VIS.Services.Private.Models;
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
using STM.Common.XmlParsers;
using Newtonsoft.Json;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class PublishMessageController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPublishedRtzMessageService _publishedMessageService;
        private IMessageTypeService _messageTypeService;
        private IACLObjectService _aclObjectService;
        private IVisSubscriptionService _visSubscriptionService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedMessageService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="aclObjectService"></param>
        /// <param name="visSubscriptionService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public PublishMessageController(IPublishedRtzMessageService publishedMessageService, 
            IMessageTypeService messageTypeService,
            IACLObjectService aclObjectService,
            IVisSubscriptionService visSubscriptionService,
            IConnectionInformationService connectionInformationService)
        {
            _publishedMessageService = publishedMessageService;
            _messageTypeService = messageTypeService;
            _aclObjectService = aclObjectService;
            _visSubscriptionService = visSubscriptionService;
            _connectionInformationService = connectionInformationService;
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

            try
            {
                var result = new List<PublishedMessageContract>();
                var messages = _publishedMessageService.Get(filter: x => (int)x.MessageStatus != 8);

                foreach (var message in messages)
                {
                    result.Add(new PublishedMessageContract
                    {
                        Message = System.Text.Encoding.Default.GetString(message.Message),
                        MessageID = message.MessageID,
                        MessageLastUpdateTime = message.MessageLastUpdateTime,
                        MessageStatus = (int)message.MessageStatus,
                        MessageType = message.MessageType.Name,
                        MessageValidFrom = message.MessageValidFrom,
                        MessageValidTo = message.MessageValidTo,
                        PublishTime = message.PublishTime
                    });
                }
                SetLastInteractionTime();
                _context.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "VIS internal server error. " + ex.Message;

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// Remove published message
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("publishedMessage")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public ResponseObj RemovePublishedMessage([FromUri] string dataId)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (!FormatValidation.IsValidUvid(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
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
                    var subscriptions = _visSubscriptionService.Get(x => x.MessageID == dataId);
                    if (subscriptions != null)
                    {
                        foreach (var subscription in subscriptions)
                        {
                            _visSubscriptionService.Delete(subscription);
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

                    SetLastInteractionTime();

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
        /// Publish message to VIS database for subsequent sending to subscribers
        /// </summary>
        /// <param name="dataId">data Id for published message normally this is the UVID</param>
        /// <param name="messageType">Message type for published message (RTZ)</param>
        /// <param name="message">Typically STM payload data (RTZ)</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("publishMessage")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "text/xml", Exclusive = true)]
        public ResponseObj PublishMessage([FromUri]string dataId,
            [FromUri]string messageType,
            [FromBody]string message)
        {

            try
            {
                // Check incoming args
                if (!FormatValidation.IsValidUvid(dataId))
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Invalid UVID format."));
                }
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

                // Check if correct messageType e.g. only RTZ accepted
                var msgType = _messageTypeService.Get(m => m.Name == messageType).FirstOrDefault();
                if (msgType == null 
                    || string.IsNullOrEmpty(msgType.Name) 
                    || msgType.Name.ToLower() != "rtz")
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Unsupported or unknown message type."));
                }

                var messageToUpdate = _publishedMessageService.Get(m => m.MessageID == dataId).FirstOrDefault();

                var responsObj = new ResponseObj(dataId);

                CheckRouteStatus(message, dataId);

                if (messageToUpdate == null) //New message
                {
                    var newMessage = new PublishedRtzMessage();
                    newMessage.Message = Serialization.StrToByteArray(message);
                    newMessage.MessageID = dataId;
                    newMessage.MessageType = msgType;
                    newMessage.PublishTime = DateTime.UtcNow;

                    _publishedMessageService.Insert(newMessage);
                }
                else
                {
                    messageToUpdate.Message = Serialization.StrToByteArray(message);
                    messageToUpdate.PublishTime = DateTime.UtcNow;

                    _publishedMessageService.Update(messageToUpdate);
                }

                //Notify subscribers if messageStatus not inactive
                var parser = RtzParserFactory.Create(message);

                string routeInfo = parser.RouteInfo;
                string routeStatus = parser.RouteStatus;
                if (!string.IsNullOrEmpty(routeInfo) && !string.IsNullOrEmpty(routeStatus))
                {
                    _publishedMessageService.SendMessageToSubsribers(message, dataId);
                }

                SetLastInteractionTime();
                // Save to DB
                _context.SaveChanges();

                return responsObj;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                var errorMessage = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        private void CheckRouteStatus(string msg, string uvid)
        {
            try
            {
                var parser = RtzParserFactory.Create(msg);

                if (!string.IsNullOrEmpty(msg) 
                    && !string.IsNullOrEmpty(parser.RouteInfo) 
                    && !string.IsNullOrEmpty(parser.RouteStatus))
                {
                    int status = Convert.ToInt32(parser.RouteStatus);

                    // If there is already one item in status used for monitoring set the old one to status 8
                    if (status == (int)RouteStatus.Used_for_monitoring)
                    {
                        var storedMessage = _publishedMessageService.Get(m =>
                            m.MessageStatus == RouteStatus.Used_for_monitoring && m.MessageID != uvid).FirstOrDefault();

                        if (storedMessage != null)
                        {
                            storedMessage.MessageStatus = RouteStatus.Inactive;
                            _publishedMessageService.SaveAndUpdate(storedMessage, false, true);
                        }
                    }
                }
                else
                {
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "RouteStatusEnum in STM extension cannot be null or empty");
                }
            }
            catch(HttpResponseException )
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                var error = "VIS internal server error. " + ex.Message;

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, error);
            }
        }

        private void SetLastInteractionTime()
        {
            var conInfo = _connectionInformationService.Get().FirstOrDefault();
            if (conInfo == null)
                _connectionInformationService.Insert(new ConnectionInformation { LastInteraction = DateTime.UtcNow });
            else
            {
                conInfo.LastInteraction = DateTime.UtcNow;
                _connectionInformationService.Update(conInfo);
            }
        }
    }
}