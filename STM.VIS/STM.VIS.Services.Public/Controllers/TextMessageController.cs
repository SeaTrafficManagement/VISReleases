using Microsoft.Practices.Unity;
using STM.Common.Services.Internal;
using STM.VIS.Services.Public.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using STM.Common.DataAccess.Entities;
using STM.Common;
using System.Net.Http;
using System.Configuration;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using System.Data.Entity.Validation;
using System.Text;
using STM.Common.Services;
using Newtonsoft.Json;
using STM.Common.XmlParsers;

namespace STM.VIS.Services.Public.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class TextMessageController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPublishedRtzMessageService _publishedMessageService;
        private IACLObjectService _aclObjectService;
        private IIdentityService _identityService;
        private IVisSubscriptionService _subscriptionService;
        private IMessageTypeService _messageTypeService;
        private IUploadedMessageService _uploadedMessageService;
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="publishedMessageService"></param>
        /// <param name="aclObjectService"></param>
        /// <param name="identityService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="uploadedMessageService"></param>
        /// <param name="notificationService"></param>
        [InjectionConstructor]
        public TextMessageController(StmDbContext context,
            IPublishedRtzMessageService publishedMessageService,
            IACLObjectService aclObjectService,
            IIdentityService identityService,
            IVisSubscriptionService subscriptionService,
            IMessageTypeService messageTypeService,
            IUploadedMessageService uploadedMessageService,
            INotificationService notificationService)
        {
            _context = context;
            _publishedMessageService = publishedMessageService;
            _aclObjectService = aclObjectService;
            _identityService = identityService;
            _subscriptionService = subscriptionService;
            _messageTypeService = messageTypeService;
            _uploadedMessageService = uploadedMessageService;
            _notificationService = notificationService;
            InstanceContext.ServiceId = ConfigurationManager.AppSettings["MyIdentity"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Upload text message to VIS from other services i.e. Route Optimization service.</remarks>
        /// <param name="textMessageObject">Uploaded Text message to consumer</param>
        /// <param name="deliveryAckEndPoint">Acknowledgement expected. Optionally an URL (base URL for VIS as in Service Registry) can be provided if acknowledgement is expected</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("textMessage")]
        [SwaggerOperation("UploadTextMessage")]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "text/xml", Exclusive = true)]
        public virtual ResponseObj UploadTextMessage([FromBody]string textMessageObject, [FromUri]string deliveryAckEndPoint = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            var messageType = _messageTypeService.Get(x => x.Name.ToLower() == "txt").First();

            var parser = new TxtParser(textMessageObject);
            var dataId = parser.TextMessageId;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("deliveryAckEndPoint", deliveryAckEndPoint);
            paramList.Add(param);
            
            var errorMsgResponse = string.Empty;
            var request = Request;
            var headers = request.Headers;

            if (string.IsNullOrEmpty(textMessageObject))
            {
                log.Debug("TextMessage is empty");
                errorMsgResponse += "Required parameter TextMessage is missing." + Environment.NewLine;

                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter TextMessage is missing.");
            }

            try
            {
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");
                    errorMsgResponse += "Calling organization identity missing in header." + Environment.NewLine;

                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                //Write to log table
                _logEventService.LogInfo(EventNumber.VIS_uploadTextMessage_request, EventDataType.TXT,
                    paramList, textMessageObject);
                // Get identity ether from internal id talbe or from id registry
                var identity = _identityService.GetCallerIdentity();

                var result = new UploadedMessage();
                result.AckDelivered = false;
                result.DeliveryAckEndpoint = deliveryAckEndPoint;
                result.DeliveryAckReqested = string.IsNullOrEmpty(deliveryAckEndPoint) ? false : true;
                result.FetchedByShip = false;
                result.FetchTime = null;
                result.FromOrg = identity;
                result.FromServiceId = InstanceContext.CallerServiceId;
                result.Message = Serialization.StrToByteArray(textMessageObject);
                result.MessageType = _messageTypeService.Get(x => x.Name.ToLower() == "txt").First();
                result.Notified = false;
                result.ReceiveTime = DateTime.UtcNow;
                result.MessageID = dataId;

                //Notify STM module
                var notification = new Common.Services.Internal.Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.MESSAGE_WAITING;
                notification.Subject = "New Text message uploaded.";
                notification.NotificationSource = EnumNotificationSource.VIS;

                result.Notified = _notificationService.Notify(notification);

                _uploadedMessageService.InsertTXT(result);
                var responsObj = new ResponseObj("Success store message");

                //Save to DB
                _context.SaveChanges();

                _logEventService.LogSuccess(EventNumber.VIS_uploadTextMessage_response, EventDataType.Other, paramList, 
                    JsonConvert.SerializeObject(responsObj, Formatting.Indented));
                
                return responsObj;
            }

            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadTextMessage_request, EventType.Error_internal,
                    paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));

                throw;
            }
            catch (DbEntityValidationException dbEx)
            {
                var response = request.CreateResponse(HttpStatusCode.InternalServerError);
                StringBuilder sb = new StringBuilder();
                foreach (var item in dbEx.EntityValidationErrors)
                {
                    sb.Append(item + " errors: ");
                    foreach (var i in item.ValidationErrors)
                    {
                        sb.Append(i.PropertyName + " : " + i.ErrorMessage);
                    }
                    sb.Append(Environment.NewLine);
                }
                string msg = "VIS internal server error. " + dbEx.Message;

                _logEventService.LogError(EventNumber.VIS_uploadTextMessage_request, EventType.Error_internal,
                    paramList, msg);
                log.ErrorFormat("Validation errors: {0}", sb.ToString());

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, sb.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadTextMessage_request, EventType.Error_internal,
                    paramList, ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}