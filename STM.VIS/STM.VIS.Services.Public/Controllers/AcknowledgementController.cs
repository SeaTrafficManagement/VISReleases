using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using STM.Common;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using STM.Common.Services;
using STM.Common.Services.Internal;
using STM.Common.Services.Internal.Interfaces;
using STM.VIS.Services.Public.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace STM.VIS.Services.Public.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AcknowledgementController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private INotificationService _notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="notificationService"></param>
        [InjectionConstructor]
        public AcknowledgementController(StmDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Endpoint for receipt of acknowledgement of uploaded message</remarks>
        /// <param name="deliveryAck">Acknowledgement </param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be authenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("acknowledgement")]
        [SwaggerOperation("Acknowledgement")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        public virtual ResponseObj Acknowledgement([FromBody]DeliveryAck deliveryAck)
        {
            _logEventService.LogInfo(EventNumber.VIS_acknowledgement_request, EventDataType.Other, null, 
                JsonConvert.SerializeObject(deliveryAck, Formatting.Indented));
            try
            {
                _notificationService.Notify(new Common.Services.Internal.Interfaces.Notification
                {
                    FromOrgId = InstanceContext.CallerServiceId,
                    FromOrgName = deliveryAck.FromName,
                    FromServiceId = InstanceContext.CallerServiceId,
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationType = EnumNotificationType.ACKNOWLEDGEMENT_RECEIVED,
                    Subject = "Acknowledgement",
                    NotificationSource = EnumNotificationSource.VIS,
                Body = string.Format("Acknowledgement of message delivery for message with id {0} recieved from {1}, {2}", deliveryAck.ReferenceId, deliveryAck.FromName, deliveryAck.FromId)
                });

                _context.SaveChanges();
                _logEventService.LogSuccess(EventNumber.VIS_acknowledgement_response, EventDataType.Other, null,
                    JsonConvert.SerializeObject(deliveryAck, Formatting.Indented));
                return new ResponseObj("Aclnowledgement was delivered.");
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_acknowledgement_request, EventType.Error_internal, null,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));

                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_acknowledgement_request, EventType.Error_internal, null, ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                var errorMsg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(msg),
                    ReasonPhrase = "Internal error."
                };

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}