using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using STM.Common;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using STM.Common.Services;
using STM.Common.Services.Internal.Interfaces;
using STM.Common.XmlParsers;
using STM.VIS.Services.Public.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace STM.VIS.Services.Public.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AreaController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUploadedMessageService _uploadedMessageService;
        private IIdentityService _identityService;
        private IMessageTypeService _messageTypeService;
        private MessageType _messageType;
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uploadedMessageService"></param>
        /// <param name="identityService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="notificationService"></param>
        [InjectionConstructor]
        public AreaController(StmDbContext context, IUploadedMessageService uploadedMessageService,
            IIdentityService identityService, IMessageTypeService messageTypeService, 
            INotificationService notificationService)
        {
            _context = context;
            _uploadedMessageService = uploadedMessageService;
            _identityService = identityService;
            _messageTypeService = messageTypeService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Upload area message to VIS from other services i.e. Route Check service as an informational message</remarks>
        /// <param name="area">Uploaded area message in S124 format to consumer</param>
        /// <param name="deliveryAckEndPoint">Acknowledgement expected. Optionally an URL (base URL for VIS as in Service Registry)</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("area")]
        [SwaggerOperation("UploadArea")]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "text/xml", Exclusive = true)]
        public virtual ResponseObj UploadArea([FromBody]string area, [FromUri]string deliveryAckEndPoint = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            _messageType = _messageTypeService.Get(x => x.Name.ToLower() == "s124").First();

            var parser = new S124Parser(area);
            var dataId = parser.AreaMessageId;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("deliveryAckEndPoint", deliveryAckEndPoint);
            paramList.Add(param);

            var errorMsgResponse = string.Empty;
            var request = Request;
            var headers = request.Headers;

            if (string.IsNullOrEmpty(area))
            {
                log.Debug("AreaMessage is empty");
                errorMsgResponse += "Required parameter AreaMessage is missing." + Environment.NewLine;

                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter AreaMessage is missing.");
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
                _logEventService.LogInfo(EventNumber.VIS_uploadAreaMessage_request, EventDataType.S124, paramList, area);
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
                result.Message = Serialization.StrToByteArray(area);
                result.MessageType = _messageType;
                result.Notified = false;
                result.ReceiveTime = DateTime.UtcNow;
                result.MessageID = dataId;

                //Notify STM module
                var notification = new Common.Services.Internal.Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.MESSAGE_WAITING;
                notification.Subject = "New Area message uploaded.";
                notification.NotificationSource = EnumNotificationSource.VIS;

                result.Notified = _notificationService.Notify(notification);

                _uploadedMessageService.InsertArea(result);
                var responsObj = new ResponseObj("Success store message");

                //Save to DB
                _context.SaveChanges();

                _logEventService.LogSuccess(EventNumber.VIS_uploadAreaMessage_response, EventDataType.Other, paramList,
                    JsonConvert.SerializeObject(responsObj, Formatting.Indented));

                return responsObj;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadAreaMessage_request, EventType.Error_internal, paramList,
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

                _logEventService.LogError(EventNumber.VIS_uploadAreaMessage_request, EventType.Error_internal,
                    paramList, msg);
                log.ErrorFormat("Validation errors: {0}", sb.ToString());

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, sb.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadAreaMessage_request, EventType.Error_internal,
                    paramList, ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}