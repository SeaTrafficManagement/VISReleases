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
using STM.Common.Services;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using STM.SSC.Internal;

using Newtonsoft.Json;
using STM.Common.XmlParsers;

namespace STM.VIS.Services.Public.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class VoyagePlanController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPublishedRtzMessageService _publishedMessageService;
        private IACLObjectService _aclObjectService;
        private IIdentityService _identityService;
        private IVisSubscriptionService _subscriptionService;
        private IMessageTypeService _messageTypeService;
        private IUploadedMessageService _uploadedMessageService;
        private INotificationService _notificationService;
        private ISccPrivateService _sscService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityService"></param>
        /// <param name="aclObjectService"></param>
        /// <param name="publishedMessageService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="uploadedMessageService"></param>
        /// <param name="notificationService"></param>
        /// <param name="context"></param>
        /// <param name="sscService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public VoyagePlanController(StmDbContext context,
            IPublishedRtzMessageService publishedMessageService,
            IACLObjectService aclObjectService,
            IIdentityService identityService,
            IVisSubscriptionService subscriptionService,
            IMessageTypeService messageTypeService,
            IUploadedMessageService uploadedMessageService,
            INotificationService notificationService,
            ISccPrivateService sscService,
            IConnectionInformationService connectionInformationService)
        {
            _context = context;
            _publishedMessageService = publishedMessageService;
            _aclObjectService = aclObjectService;
            _identityService = identityService;
            _subscriptionService = subscriptionService;
            _messageTypeService = messageTypeService;
            _uploadedMessageService = uploadedMessageService;
            _notificationService = notificationService;
            _sscService = sscService;
            _connectionInformationService = connectionInformationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Returns active VoyagePlans</remarks>
        /// <param name="uvid">Unique identity (URN) of a voyageplan</param>
        /// <param name="routeStatus">Status of a route for a voyageplan: 1-Original   2-Planned_for_voyage    3-Optimized 4-Cross_Checked 5-Safety_Checked    6-Approved  7-Used_for_monitoring   8-Inactive</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden (Not authorized request forwarded to operator)</response>
        /// <response code="404">Not Found (the requested voyagePlan is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpGet]
        [Route("voyagePlans")]
        [SwaggerOperation("GetVoyagePlans")]
        [SwaggerResponse(200, type: typeof(Models.GetVoyagePlanResponse))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public virtual Models.GetVoyagePlanResponse GetVoyagePlans([FromUri] string uvid = null, [FromUri]string routeStatus = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            GetVoyagePlanResponse result;

            var request = Request;
            var headers = request.Headers;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("uvid", uvid);
            paramList.Add(param);
            param = new KeyValuePair<string, string>("routeStatus", routeStatus);
            paramList.Add(param);

            // Get identity ether from internal id talbe or from id registry
            var identity = _identityService.GetCallerIdentity();

            // Get the calling serviceId
            if (string.IsNullOrEmpty(InstanceContext.CallerServiceId))
            {
                var msg = "Missing 'incomingServiceId'";
                log.Debug(msg);
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, msg);
            }
            log.Debug("incomingServiceId: " + InstanceContext.CallerServiceId);

            // Get the calling organisation identity
            if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Missing organizationId in request");
            }
            log.Debug("incomingOrganizationId: " + InstanceContext.CallerServiceId);

            try
            {
                _logEventService.LogInfo(EventNumber.VIS_getVoyagePlan_request, EventDataType.None, paramList, null);

                if (string.IsNullOrEmpty(uvid) && string.IsNullOrEmpty(routeStatus))
                {
                    result = GetPublishedVoyagePlans(identity, paramList);
                }

                else if (!string.IsNullOrEmpty(uvid) && string.IsNullOrEmpty(routeStatus))
                {
                    if (!FormatValidation.IsValidUvid(uvid))
                    {
                        throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
                    }
                    result = GetPublishedVoyagePlans(identity, paramList, uvid);
                }

                else if (string.IsNullOrEmpty(uvid) && !string.IsNullOrEmpty(routeStatus))
                {
                    int routeStatusInt = -1;
                    if (int.TryParse(routeStatus, out routeStatusInt))
                    {
                        if (routeStatusInt <= 0 || routeStatusInt > 7)
                        {
                            throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Argument {0} out of range.", routeStatus));
                        }
                    }
                    else
                    {
                        throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Argument {0} format unknown.", routeStatus));
                    }
                    result = GetPublishedVoyagePlans(identity, paramList, null, routeStatusInt);
                }
                else
                {
                    if (!FormatValidation.IsValidUvid(uvid))
                    {
                        throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
                    }

                    int routeStatusInt = -1;
                    if (int.TryParse(routeStatus, out routeStatusInt))
                    {
                        if (routeStatusInt <= 0 || routeStatusInt > 8)
                        {
                            throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Argument {0} out of range.", routeStatus));
                        }
                    }
                    else
                    {
                        throw CreateHttpResponseException(HttpStatusCode.BadRequest, string.Format("Argument {0} format unknown.", routeStatus));
                    }
                    result = GetPublishedVoyagePlans(identity, paramList, uvid, routeStatusInt);
                }
                _logEventService.LogSuccess(EventNumber.VIS_getVoyagePlan_response, EventDataType.Other,
                    paramList, JsonConvert.SerializeObject(result, Formatting.Indented));

                var conStatus = _connectionInformationService.Get().FirstOrDefault();
                if (conStatus != null && conStatus.LastInteraction != null)
                {
                    result.LastInteractionTime = conStatus.LastInteraction;
                    result.LastInteractionTime = DateTime.SpecifyKind(result.LastInteractionTime.Value, DateTimeKind.Utc);
                }

                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_getVoyagePlan_request, EventType.Error_internal, paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_getVoyagePlan_request, EventType.Error_internal, paramList, ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        private GetVoyagePlanResponse GetPublishedVoyagePlans(Identity identity, List<KeyValuePair<string, string>> paramList, 
            string uvid = null, int? routeStatusInt = null)
        {
            bool accessToAnyUVID = false;

            var result = new Models.GetVoyagePlanResponse(DateTime.UtcNow);
            result.VoyagePlans = new List<Models.VoyagePlan>();
            List<PublishedRtzMessage> publishedVoyagePlans = null;

            //Get all published voyageplans based on parameter values
            if (uvid == null && routeStatusInt == null)
            {
                publishedVoyagePlans = _publishedMessageService.Get(x => (int)x.MessageStatus != 8).
                            OrderByDescending(x => x.PublishTime).ToList();
            }
            else if(uvid != null && routeStatusInt == null)
            {
                publishedVoyagePlans = _publishedMessageService.Get(x => x.MessageID == uvid &&
                            (int)x.MessageStatus != 8).
                            OrderByDescending(x => x.PublishTime).ToList();
            }
            else if(uvid == null && routeStatusInt != null)
            {
                publishedVoyagePlans = _publishedMessageService.Get(x => (int)x.MessageStatus != 8 &&
                            (int)x.MessageStatus == routeStatusInt).
                            OrderByDescending(x => x.PublishTime).ToList();
            }
            else
            {
                publishedVoyagePlans = _publishedMessageService.Get(x => x.MessageID == uvid &&
                            (int)x.MessageStatus == routeStatusInt).
                            OrderByDescending(x => x.PublishTime).ToList();
            }

            //Need to loop in order to distinguish the VP's with no access from the ones with access
            if (publishedVoyagePlans != null && publishedVoyagePlans.Count() > 0)
            {
                foreach (var publishedVoyagePlan in publishedVoyagePlans)
                {
                    // Now look up if orgId is authorized to this voyageplan
                    var aclObject = _aclObjectService.Get(x => x.Subscriber.ID == identity.ID && x.MessageID == publishedVoyagePlan.MessageID);
                    if (aclObject == null || aclObject.Count() == 0)
                    {
                        //No access to this one, send notification to STM module
                        var msg = "Authorization failed: ACL";
                        log.Debug(msg);

                        //Notify STM Module
                        var notification = new Common.Services.Internal.Interfaces.Notification();
                        notification.FromOrgName = identity.Name;
                        notification.FromOrgId = identity.UID;
                        notification.FromServiceId = InstanceContext.CallerServiceId;
                        notification.NotificationType = EnumNotificationType.UNAUTHORIZED_REQUEST;
                        notification.Subject = string.Format("Access denied for identity {0}.", identity.Name);
                        notification.NotificationSource = EnumNotificationSource.VIS;

                        _notificationService.Notify(notification);
                        _context.SaveChanges();

                        // Log error
                        _logEventService.LogError(EventNumber.VIS_getVoyagePlan_request, EventType.Error_authorization,
                            paramList, InstanceContext.CallerServiceId);
                        _context.SaveChanges();
                    }
                    else
                    {
                        accessToAnyUVID = true;
                        //Add it to response object
                        var rtzString = Serialization.ByteArrayToString(publishedVoyagePlan.Message);
                        var vp = new Models.VoyagePlan(rtzString);
                        result.VoyagePlans.Add(vp);
                    }
                }
            }
            else
            {
                //We didn't find any voyageplans i.e. return not found
                throw CreateHttpResponseException(HttpStatusCode.NotFound, "Voyageplans not found");
            }

            //Final check to verify that we did return at least one VP
            if (!accessToAnyUVID)
            {
                throw CreateHttpResponseException(HttpStatusCode.Forbidden, "Authorization failed: ACL");
            }
            return result;
        }

        private List<Models.VoyagePlan> ConvertToContract(List<PublishedRtzMessage> from)
        {
            List<Models.VoyagePlan> to = new List<Models.VoyagePlan>();
            foreach (var msg in from)
            {
                var msgString = Serialization.ByteArrayToString(msg.Message);
                Models.VoyagePlan vp = new Models.VoyagePlan(msgString);
                to.Add(vp);
            }
            return to;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Request subscription for active Voyage Plan from other services i.e. Enhanced Monitoring</remarks>
        /// <param name="callbackEndpoint">An endpoint (base URL as in Service Registry) specifying the address where the subscribed data is to be posted</param>
        /// <param name="uvid">Unique identity (URN) of a voyageplan</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden (Not authorized request forwarded to operator)</response>
        /// <response code="404">Not Found (the requested Voyage Plan is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("voyagePlans/subscription")]
        [SwaggerOperation("SubscribeToVoyagePlan")]
        [SwaggerResponse(200, type: typeof(Models.ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public virtual Models.ResponseObj SubscribeToVoyagePlan([FromUri]string callbackEndpoint, [FromUri] string uvid = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());
            bool accessToAnyUVID = false;

            var responseObj = new Models.ResponseObj("Success storing subscription request.");

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("uvid", uvid);
            paramList.Add(param);
            param = new KeyValuePair<string, string>("callbackEndpoint", callbackEndpoint);
            paramList.Add(param);

            try
            {
                if (string.IsNullOrEmpty(callbackEndpoint))
                {
                    log.Debug("Callback endpoint address is empty");

                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter CallbackEndpoint is missing.");
                }
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");

                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                _logEventService.LogInfo(EventNumber.VIS_subscribeToVoyagePlan_request, EventDataType.None, paramList, null);
                // Get identity ether from internal id talbe or from id registry
                var identity = _identityService.GetCallerIdentity();

                //Set data for notification
                var notification = new Common.Services.Internal.Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.UNAUTHORIZED_REQUEST;
                notification.Subject = "New subscription request.";
                notification.NotificationSource = EnumNotificationSource.VIS;

                #region No UVID
                if (string.IsNullOrEmpty(uvid))
                {
                    var voyagePlans = _publishedMessageService.Get(x => (int)x.MessageStatus != 8).
                        OrderByDescending(x => x.PublishTime).ToList();

                    if (voyagePlans != null && voyagePlans.Count() > 0)
                    {
                        foreach (var voyagePlan in voyagePlans)
                        {
                            var aclObj = _aclObjectService.Get(x => x.Subscriber.ID == identity.ID && x.MessageID == voyagePlan.MessageID);
                            if (aclObj == null || aclObj.Count() == 0)
                            {
                                _notificationService.Notify(notification);
                                _context.SaveChanges();
                            }
                            else
                            {
                                accessToAnyUVID = true;

                                //check if sub already exists
                                var sub = _subscriptionService.Get(x => x.CallbackEndpoint == callbackEndpoint && x.MessageID == voyagePlan.MessageID && x.SubscriberIdentity.ID == identity.ID).FirstOrDefault();
                                if (sub == null)
                                {
                                    SetSubscription(callbackEndpoint, voyagePlan.MessageID, identity, notification, paramList);
                                }
                            }
                        }
                    }
                    if (!accessToAnyUVID)
                    {
                        throw CreateHttpResponseException(HttpStatusCode.Forbidden, "Authorization failed: ACL");
                    }
                }
                #endregion

                #region UVID
                else
                {
                    var sub = _subscriptionService.Get(x => x.CallbackEndpoint == callbackEndpoint && x.MessageID == uvid && x.SubscriberIdentity.ID == identity.ID).FirstOrDefault();
                    if (sub == null)
                    {
                        //Check if uvid exist
                        var vpCheck = _publishedMessageService.Get(x => x.MessageID == uvid).FirstOrDefault();
                        if(vpCheck == null)
                        {
                            string msg = string.Format("Voyageplan with UVID {0} does not exist.", uvid);
                            log.Debug(msg);

                            _notificationService.Notify(notification);
                            _context.SaveChanges();

                            throw CreateHttpResponseException(HttpStatusCode.NotFound, msg);
                        }
                        // Check if identity has access to UVID
                        var aclId = _aclObjectService.Get(a => a.Subscriber.UID == InstanceContext.CallerOrgId && a.MessageID == uvid).FirstOrDefault();
                        if (aclId == null)
                        {
                            string msg = string.Format("Access to UVID {0} failed for user {1}", uvid, InstanceContext.CallerOrgId);
                            log.Debug(msg);

                            _notificationService.Notify(notification);
                            _context.SaveChanges();

                            throw CreateHttpResponseException(HttpStatusCode.Forbidden, msg);
                        }
                        SetSubscription(callbackEndpoint, uvid, identity, notification, paramList);
                    }
                }

                #endregion

                return responseObj;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_subscribeToVoyagePlan_request, EventType.Error_internal, paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));

                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_subscribeToVoyagePlan_request, EventType.Error_internal, paramList,
                    ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg); ;
            }
        }

        private void SetSubscription(string callbackEndpoint, string uvid, Identity identity,
            Common.Services.Internal.Interfaces.Notification notification, List<KeyValuePair<string, string>> paramList)
        {

            VisSubscription newSub = new VisSubscription();
            newSub.CallbackEndpoint = callbackEndpoint;
            newSub.MessageID = uvid;
            newSub.MessageType = _messageTypeService.Get(x => x.Name.ToLower() == "rtz").First();
            newSub.SubscriberIdentity = identity;
            newSub.TimeOfSubscriptionRequest = DateTime.UtcNow;
            newSub.IsAuthorized = true;
            _subscriptionService.Insert(newSub);

            // Send message to new subscriber
            var message = _publishedMessageService.Get(x => x.MessageID == uvid).FirstOrDefault();
            if (message != null)
            {
                _publishedMessageService.SendMessage(System.Text.Encoding.Default.GetString(message.Message),
                    uvid, newSub.CallbackEndpoint,
                    new Identity { Name = identity.Name, UID = identity.UID });
            }

            // Save to DB
            _context.SaveChanges();

            _logEventService.LogSuccess(EventNumber.VIS_subscribeToVoyagePlan_response, EventDataType.Other,
                paramList,
                newSub.MessageID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Remove subscription for active Voyage Plan from other services i.e. Enhanced Monitoring</remarks>
        /// <param name="callbackEndpoint">An endpoint (base URL as in Service Registry) specifying the address where the subscribed data was posted</param>
        /// <param name="uvid">Unique identity (URN) of a voyageplan</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found (the requested Voyage Plan is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpDelete]
        [Route("voyagePlans/subscription")]
        [SwaggerOperation("RemoveVoyagePlanSubscription")]
        [SwaggerResponse(200, type: typeof(Models.ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public virtual Models.ResponseObj RemoveVoyagePlanSubscription([FromUri]string callbackEndpoint, [FromUri]string uvid = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            var request = Request;
            var headers = request.Headers;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("uvid", uvid);
            paramList.Add(param);
            param = new KeyValuePair<string, string>("callbackEndpoint", callbackEndpoint);
            paramList.Add(param);

            // First, validate that we have mandatory in-parameters
            if (uvid != null && !FormatValidation.IsValidUvid(uvid))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
            }

            if (string.IsNullOrEmpty(callbackEndpoint))
            {
                log.Debug("Callback endpoint address is empty");
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter CallbackEndpoint is missing.");
            }

            try
            {
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                // Write to log table
                _logEventService.LogInfo(EventNumber.VIS_removeSubscribeToVoyagePlan_request, EventDataType.None, paramList, null);
                
                // Get identity ether from internal id talbe or from id registry
                var identity = _identityService.GetCallerIdentity();

                var subscriptionsToDelete = new List<VisSubscription>();

                if (uvid == null)
                {
                    subscriptionsToDelete = _subscriptionService.Get(s => s.SubscriberIdentity.ID == identity.ID &&
                        s.CallbackEndpoint == callbackEndpoint).ToList();
                }
                else
                {
                    subscriptionsToDelete = _subscriptionService.Get(s => s.SubscriberIdentity.ID == identity.ID &&
                        s.MessageID == uvid &&
                        s.CallbackEndpoint == callbackEndpoint).ToList();
                }

                if (subscriptionsToDelete != null
                    && subscriptionsToDelete.Count > 0)
                {
                    foreach (var subscription in subscriptionsToDelete)
                    {
                        _subscriptionService.Delete(subscription.ID);

                        //Save to DB
                        _context.SaveChanges();
                    }
                }
                else
                {
                    string msg = string.Format("Subscription not found for UVID {0} and user {1} with endpoint {2}", uvid, InstanceContext.CallerOrgId, callbackEndpoint);
                    log.Debug(msg);

                    throw CreateHttpResponseException(HttpStatusCode.NotFound, msg);
                }

                var responseObj = new ResponseObj("Success delete subscription(s).");
                _logEventService.LogSuccess(EventNumber.VIS_removeSubscribeToVoyagePlan_response, EventDataType.Other, paramList,
                    JsonConvert.SerializeObject(responseObj, Formatting.Indented));

                return responseObj;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_removeSubscribeToVoyagePlan_request, EventType.Error_internal, paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_removeSubscribeToVoyagePlan_request, EventType.Error_internal, paramList,
                    ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Upload VoyagePlan to VIS from other services i.e. Route Optimization service.</remarks>
        /// <param name="voyagePlan">Voyage Plan in RTZ format</param>
        /// <param name="deliveryAckEndPoint">Acknowledgement expected. Optionally an URL (base URL for VIS as in Service Registry) can be provided if acknowledgement is expected when message reached consumer.</param>
        /// <param name="callbackEndpoint">Callback expected. Optionally an URL (base URL for VIS as in Service Registry) can be provided where response is expected</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("voyagePlans")]
        [SwaggerOperation("UploadVoyagePlan")]
        [SwaggerResponse(200, type: typeof(Models.ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "text/xml", Exclusive = true)]
        public virtual Models.ResponseObj UploadVoyagePlan([FromBody]string voyagePlan, [FromUri]string deliveryAckEndPoint = null,
           [FromUri] string callbackEndpoint = null )
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            var messageType = _messageTypeService.Get(x => x.Name.ToLower() == "rtz").First();

            var request = Request;
            var headers = request.Headers;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("deliveryAckEndPoint", deliveryAckEndPoint);
            paramList.Add(param);
            param = new KeyValuePair<string, string>("callbackEndpoint", callbackEndpoint);
            paramList.Add(param);

            //First, validate that we have mandatory in-parameters
            var parser = RtzParserFactory.Create(voyagePlan);

            var uvid = parser.VesselVoyage;
            if (string.IsNullOrEmpty(uvid))
            {
                log.Debug("UVID is empty");

                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter UVID is missing.");
            }

            if (!FormatValidation.IsValidUvid(uvid))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
            }

            if (voyagePlan == null)
            {
                log.Debug("VoyagePlan is empty");

                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter VoyagePlan is missing.");
            }

            try
            {
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");

                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                //Write to log table
                _logEventService.LogInfo(EventNumber.VIS_uploadVoyagePlan_request, EventDataType.RTZ, paramList, voyagePlan);
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
                result.Message = Serialization.StrToByteArray(voyagePlan);
                result.MessageType = _messageTypeService.Get(x => x.Name.ToLower() == "rtz").First();
                result.Notified = false;
                result.ReceiveTime = DateTime.UtcNow;
                result.MessageID = uvid;
                result.CallbackEndpoint = string.IsNullOrEmpty(callbackEndpoint) ? string.Empty : callbackEndpoint;

                _uploadedMessageService.InsertRTZ(result);
                
                //Save to DB
                _context.SaveChanges();

                //Notify STM module
                var notification = new Common.Services.Internal.Interfaces.Notification();
                notification.FromOrgName = identity.Name;
                notification.FromOrgId = identity.UID;
                notification.FromServiceId = InstanceContext.CallerServiceId;
                notification.NotificationType = EnumNotificationType.MESSAGE_WAITING;
                notification.Subject = "New voyageplan uploaded.";
                notification.NotificationSource = EnumNotificationSource.VIS;

                var notified = _notificationService.Notify(notification);

                if (notified)
                {
                    _context.Entry(result).Reload();
                    result.Notified = true;
                    _uploadedMessageService.Update(result);
                }

                var responsObj = new ResponseObj("Success store message");

                //Save to DB
                _context.SaveChanges();

                _logEventService.LogSuccess(EventNumber.VIS_uploadVoyagePlan_response, EventDataType.Other, null, 
                    JsonConvert.SerializeObject(responsObj, Formatting.Indented));

                return responsObj;
            }

            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadVoyagePlan_request, EventType.Error_internal, paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_uploadVoyagePlan_request, EventType.Error_internal, paramList,
                    ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Remove subscription for active Voyage Plan from other services i.e. Enhanced Monitoring</remarks>
        /// <param name="callbackEndpoint">An endpoint (base URL as in Service Registry) specifying the address where the subscribed data was posted</param>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found (the requested Voyage Plan is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpGet]
        [Route("voyagePlans/subscription")]
        [SwaggerOperation("CheckSubscriptionToVoyagePlan")]
        [SwaggerResponse(200, type: typeof(List<Models.GetSubscriptionResponseObj>))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public virtual List<Models.GetSubscriptionResponseObj> GetSubscriptionToVoyagePlan([FromUri] string callbackEndpoint)
        {
            List<GetSubscriptionResponseObj> result = new List<GetSubscriptionResponseObj>();

            log.Info("Incoming request to " + GetCurrentMethod());

            var request = Request;
            var headers = request.Headers;

            var paramList = new List<KeyValuePair<string, string>>();
            var param = new KeyValuePair<string, string>("callbackEndpoint", callbackEndpoint);
            paramList.Add(param);

            //Do we have a callbackEndpoint?
            if (string.IsNullOrEmpty(callbackEndpoint))
            {
                log.Debug("Callback endpoint address is empty");
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required parameter CallbackEndpoint is missing.");
            }

            try
            {
                if (string.IsNullOrEmpty(InstanceContext.CallerOrgId))
                {
                    log.Debug("Calling organization identity missing in header.");
                    throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Required header incomingOrganizationId is missing.");
                }

                //Write to log table
                _logEventService.LogInfo(EventNumber.VIS_checkSubscribeToVoyagePlan_request, EventDataType.None, paramList, null);
                // Get identity ether from internal id talbe or from id registry
                var identity = _identityService.GetCallerIdentity();

                var subscriptionsToCheck = _subscriptionService.Get(s => s.SubscriberIdentity.ID == identity.ID &&
                        s.CallbackEndpoint == callbackEndpoint).ToList();

                if (subscriptionsToCheck != null && subscriptionsToCheck.Count() > 0)
                {
                    //result.DataIds = new List<string>();
                    foreach(var sub in subscriptionsToCheck)
                    {
                        //result.DataIds.Add(sub.MessageID);
                        result.Add(new GetSubscriptionResponseObj { DataId = sub.MessageID });
                    }
                }

                _logEventService.LogSuccess(EventNumber.VIS_checkSubscribeToVoyagePlan_response, EventDataType.Other, paramList,
                    JsonConvert.SerializeObject(result, Formatting.Indented));
                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_checkSubscribeToVoyagePlan_request, EventType.Error_internal,
                    paramList,
                    JsonConvert.SerializeObject(ex.Response, Formatting.Indented));
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                _logEventService.LogError(EventNumber.VIS_checkSubscribeToVoyagePlan_request, EventType.Error_internal,
                    paramList,
                    ex.Message);

                string msg = "VIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}