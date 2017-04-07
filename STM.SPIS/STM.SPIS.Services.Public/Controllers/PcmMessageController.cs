using Microsoft.Practices.Unity;
using STM.Common.Services.Internal;
using STM.SPIS.Services.Public.Models;
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

namespace STM.SPIS.Services.Public.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PcmMessageController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPublishedPcmMessageService _publishedMessageService;
        private IACLObjectService _aclObjectService;
        private IIdentityService _identityService;
        private ISpisSubscriptionService _subscriptionService;
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
        public PcmMessageController(StmDbContext context,
            IPublishedPcmMessageService publishedMessageService,
            IACLObjectService aclObjectService,
            IIdentityService identityService,
            ISpisSubscriptionService subscriptionService,
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
        /// <remarks>Upload text message to SPIS from other services i.e. Route Optimization service.</remarks>
        /// <param name="pcmMessageObject">PCM message to add to SPIS message db for delivery to STM Onboard system</param>
        /// <param name="deliveryAckEndPoint">Acknowledgement required, optionally an URL could be provided to send acknowledgment.</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("pcm")]
        [SwaggerOperation("UploadPcmMessage")]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        public virtual ResponseObj UploadPCMMessage([FromBody]string pcmMessageObject, [FromUri]string deliveryAckEndPoint = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            var messageType = _messageTypeService.Get(x => x.Name.ToLower() == "pcm").First();
            var parser = new PcmParser(pcmMessageObject);
            string dataId = parser.PcmMessageId;

            var errorMsgResponse = string.Empty;
            var request = Request;
            var headers = request.Headers;

            if (string.IsNullOrEmpty(pcmMessageObject))
            {
                log.Debug("PCM Message is empty");
                errorMsgResponse += "Required parameter pcmMessageObject is missing." + Environment.NewLine;

                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter pcmMessageObject is missing.");
            }

            var param = new KeyValuePair<string, string>("deliveryAckEndPoint", deliveryAckEndPoint);
            var paramList = new List<KeyValuePair<string, string>>();
            paramList.Add(param);

            try
            {
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");
                    errorMsgResponse += "Calling organization identity missing in header." + Environment.NewLine;

                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                //Write to log table
                _logEventService.LogInfo(EventNumber.SPIS_uploadPCM_request, EventDataType.PCM, paramList, pcmMessageObject);

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
                result.Message = Serialization.StrToByteArray(pcmMessageObject);
                result.MessageType = messageType;
                result.Notified = false;
                result.ReceiveTime = DateTime.UtcNow;
                result.MessageID = dataId;

                //Notify STM module
                var notification = new Common.Services.Internal.Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.MESSAGE_WAITING;
                notification.Subject = "PCM message uploaded.";
                notification.NotificationSource = EnumNotificationSource.SPIS;

                result.Notified = _notificationService.Notify(notification);

                _uploadedMessageService.InsertPCM(result);
                var responsObj = new ResponseObj("Success store message");

                //Save to DB
                _context.SaveChanges();

                _logEventService.LogSuccess(EventNumber.SPIS_uploadPCM_response, EventDataType.None,
                    paramList, JsonConvert.SerializeObject(responsObj, Formatting.Indented));

                return responsObj;
            }

            catch (HttpResponseException ex)
            {
                var responseString = JsonConvert.SerializeObject(ex.Response, Formatting.Indented);
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.SPIS_uploadPCM_request, EventType.Error_internal,
                    paramList, responseString);

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
                string msg = "SPIS internal server error. " + dbEx.Message;

                _logEventService.LogError(EventNumber.SPIS_uploadPCM_request, EventType.Error_internal,
                    paramList, dbEx.Message);
                log.ErrorFormat("Validation errors: {0}", sb.ToString());

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, sb.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.SPIS_uploadPCM_request, EventType.Error_internal,
                    paramList, ex.Message);

                string msg = "SPIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}