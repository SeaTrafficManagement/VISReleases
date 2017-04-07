using Microsoft.Practices.Unity;
using STM.VIS.Services.Private.Models;
using STM.Common.Services.Internal.Interfaces;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using STM.Common.DataAccess.Entities;
using STM.Common.Services;
using STM.Common;
using System.Net;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class SubscriptionController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IVisSubscriptionService _subscriptionService;
        private IMessageTypeService _messageTypeService;
        private IIdentityService _identityService;
        private IPublishedRtzMessageService _publishedMessageService;
        private IACLObjectService _aCLObjectService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedMessageService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="identityService"></param>
        /// <param name="aCLObjectService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public SubscriptionController(IPublishedRtzMessageService publishedMessageService, IVisSubscriptionService subscriptionService, IMessageTypeService messageTypeService,
            IIdentityService identityService, IACLObjectService aCLObjectService,
            IConnectionInformationService connectionInformationService)
        {
            _publishedMessageService = publishedMessageService;
            _subscriptionService = subscriptionService;
            _messageTypeService = messageTypeService;
            _identityService = identityService;
            _aCLObjectService = aCLObjectService;
            _connectionInformationService = connectionInformationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptions">Identities to add as subscribers on dataId</param>
        /// <param name="dataId"></param>
        /// <response code="200">Subscription added successfully!</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("subscription")]
        [SwaggerOperation("AddSubscription")]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public ResponseObj AddSubscription([FromBody] List<SubscriptionObject> subscriptions, [FromUri]string dataId)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (string.IsNullOrEmpty(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Missing required parameter UVID.");
            }

            if (!FormatValidation.IsValidUvid(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
            }

            try
            {
                foreach (var subscription in subscriptions)
                {
                    var uri = subscription.EndpointURL.ToString().ToLower();

                    var sub = _subscriptionService.Get(s => 
                        s.SubscriberIdentity.UID == subscription.IdentityId 
                        && s.MessageID == dataId
                        && s.CallbackEndpoint.ToLower() == uri, includeProperties: "SubscriberIdentity, MessageType").FirstOrDefault();

                    if (sub == null)
                    {
                        var acl = _aCLObjectService.Get(i => 
                            i.MessageID == dataId 
                            && i.Subscriber.UID == subscription.IdentityId).FirstOrDefault();

                        if (acl != null)
                        {
                            _subscriptionService.Insert(ConvertToEntity(subscription, dataId));
                        }
                        else
                        {
                            log.Debug(string.Format("No access for identity {0}", subscription.IdentityId));
                        }
                    }
                    else if (sub.IsAuthorized == false)
                    {
                        sub.IsAuthorized = true;
                    }

                    // Send message to new subscriber
                    var message = _publishedMessageService.Get(x => x.MessageID == dataId).FirstOrDefault();
                    if (message != null)
                    {
                        _publishedMessageService.SendMessage(System.Text.Encoding.Default.GetString(message.Message), dataId, subscription.EndpointURL.ToString(), new Identity { Name = subscription.IdentityName, UID = subscription.IdentityId });
                    }
                }

                SetLastInteractionTime();
                _context.SaveChanges();

                return new ResponseObj(dataId);
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
        /// 
        /// </summary>
        /// <remarks>Find list of subscriber identities to Voyage Plans</remarks>
        /// <param name="dataId">Data id usually an uvid</param>
        /// <response code="200">Find list of subscription identities successful!</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found ( requested identities are not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [SwaggerOperation("GetSubscriptions")]
        [SwaggerResponse(200, type: typeof(List<SubscriptionObject>))]
        [HttpGet]
        [Route("subscription")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public IList<SubscriptionObject> GetSubscriptions([FromUri]string dataId = null)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (!string.IsNullOrEmpty(dataId) && !FormatValidation.IsValidUvid(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
            }

            try
            {
                // Find all ACL objects for specified MessageID
                IEnumerable<VisSubscription> subList;

                if (string.IsNullOrEmpty(dataId))
                {
                    subList = _subscriptionService.Get(includeProperties: "SubscriberIdentity");
                }
                else
                {
                    subList = _subscriptionService.Get(s => s.MessageID == dataId, includeProperties: "SubscriberIdentity");
                }

                // Create result list
                var response = new List<SubscriptionObject>();

                if (subList != null)
                {
                    foreach (var item in subList)
                    {
                        var subObj = new SubscriptionObject
                        {
                            EndpointURL = new Uri(item.CallbackEndpoint),
                            IdentityId = item.SubscriberIdentity.UID,
                            IdentityName = item.SubscriberIdentity.Name
                        };
                        response.Add(subObj);
                    }
                }
                SetLastInteractionTime();
                _context.SaveChanges();
                return response;
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
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="subscriptionObjects"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found ( requested identities are not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerOperation("RemoveSubscriptions")]
        [SwaggerResponse(200, type: typeof(List<SubscriptionObject>))]
        [Route("subscription")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public ResponseObj RemoveSubscriptions([FromUri] string dataId, [FromBody] List<SubscriptionObject> subscriptionObjects)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (!FormatValidation.IsValidUvid(dataId))
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Invalid UVID format");
            }

            try
            {
                foreach(var subscriptionObject in subscriptionObjects)
                {
                    var identityToDelete = _identityService.Get(i => i.UID == subscriptionObject.IdentityId).FirstOrDefault();
                    if(identityToDelete == null)
                    {
                        log.Info(string.Format("Identity {0} did not exist for messageId {1}.", subscriptionObject.IdentityId, dataId));
                        return new ResponseObj(dataId);
                    }

                    var url = subscriptionObject.EndpointURL.ToString().ToLower();
                    var currentSub = _subscriptionService.Get(s => s.MessageID == dataId 
                        && s.SubscriberIdentity.UID == subscriptionObject.IdentityId
                        && s.CallbackEndpoint.ToLower() == url).FirstOrDefault();

                    if(currentSub != null)
                    {
                        //Delete it
                        _subscriptionService.Delete(currentSub);
                        var msg = string.Format("Subscription removed for identity {0} on messageId {1}.", subscriptionObject.IdentityId, dataId);
                        log.Info(msg);
                    }
                }

                SetLastInteractionTime();
                _context.SaveChanges();
                return new ResponseObj(dataId);
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

        private VisSubscription ConvertToEntity(SubscriptionObject subscriptionObject, string dataId)
        {
            var to = new VisSubscription();
            to.MessageID = dataId;
            to.IsAuthorized = true;
            to.CallbackEndpoint = subscriptionObject.EndpointURL.ToString();
            to.MessageType = _messageTypeService.Get(x => x.Name.ToLower() == "rtz").FirstOrDefault();
            to.SubscriberIdentity = _identityService.Get(x => x.UID == subscriptionObject.IdentityId).FirstOrDefault();
            return to;
        }
    }
}