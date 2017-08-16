using Microsoft.Practices.Unity;
using STM.SPIS.Services.Private.Models;
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

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class SubscriptionController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISpisSubscriptionService _SpisSubscriptionService;
        private IMessageTypeService _messageTypeService;
        private IIdentityService _identityService;
        private IPublishedPcmMessageService _publishedMessageService;
        private IACLObjectService _aCLObjectService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedMessageService"></param>
        /// <param name="SpisSubscriptionService"></param>
        /// <param name="messageTypeService"></param>
        /// <param name="identityService"></param>
        /// <param name="aCLObjectService"></param>
        [InjectionConstructor]
        public SubscriptionController(IPublishedPcmMessageService publishedMessageService, ISpisSubscriptionService SpisSubscriptionService, IMessageTypeService messageTypeService,
            IIdentityService identityService, IACLObjectService aCLObjectService)
        {
            _publishedMessageService = publishedMessageService;
            _SpisSubscriptionService = SpisSubscriptionService;
            _messageTypeService = messageTypeService;
            _identityService = identityService;
            _aCLObjectService = aCLObjectService;
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
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Missing required parameter dataID.");
            }

            var responseText = string.Empty;

            try
            {
                foreach (var subscription in subscriptions)
                {
                    var mbUri = subscription.MbEndpointURL.ToString().ToLower();
                    var amssUri = subscription.AmssEndpointURL.ToString().ToLower();

                    var entity = _SpisSubscriptionService.Get(s =>
                        s.SubscriberIdentity.UID == subscription.IdentityId
                        && s.MessageID == dataId
                        && s.MbEndpoint.ToLower() == mbUri
                        && s.AmssEndpoint.ToLower() == amssUri, includeProperties: "SubscriberIdentity, MessageType").FirstOrDefault();

                    if (entity == null)
                    {
                        responseText += string.Format("Subscription for dataId:{0} mb endpoint: {1} amss endpoint {2} was created\r\n", dataId, mbUri, amssUri);
                        var acl = _aCLObjectService.Get(i => 
                            i.MessageID == dataId 
                            && i.Subscriber.UID == subscription.IdentityId).FirstOrDefault();

                        if (acl == null)
                        {
                            log.Debug(string.Format("No access for identity {0}", subscription.IdentityId));
                            throw CreateHttpResponseException(HttpStatusCode.Forbidden, string.Format("No access for identity {0}", subscription.IdentityId));
                        }

                        entity = new SpisSubscription();
                        entity.MessageID = dataId;
                        entity.IsAuthorized = true;
                        entity.MbEndpoint = subscription.MbEndpointURL.ToString();
                        entity.AmssEndpoint = subscription.AmssEndpointURL.ToString();
                        entity.MessageType = _messageTypeService.Get(x => x.Name.ToLower() == "rtz").FirstOrDefault();
                        entity.SubscriberIdentity = _identityService.Get(x => x.UID == subscription.IdentityId).FirstOrDefault();

                        _SpisSubscriptionService.Insert(entity);
                        _context.SaveChanges();
                    }
                    else
                    {
                        responseText += string.Format("Subscription for dataId:{0} mb endpoint: {1} amss endpoint {2} already exists\r\n", dataId, mbUri, amssUri);

                        if (entity.IsAuthorized == false)
                        {
                            entity.IsAuthorized = true;
                            _SpisSubscriptionService.Update(entity);
                        }
                    }

                    // Send message to PortCDM 
                    var message = _publishedMessageService.Get(x => x.MessageID == dataId).FirstOrDefault();
                    string messageString = null;
                    if (message != null)
                    {
                        messageString = System.Text.Encoding.UTF8.GetString(message.Message);
                    }

                    entity.QueueId = _publishedMessageService.SendMessage(messageString, 
                        dataId, subscription.MbEndpointURL.ToString(), 
                        subscription.AmssEndpointURL.ToString(), 
                        new Identity {UID=subscription.IdentityId,
                            Name=subscription.IdentityName});

                    responseText += string.Format("Port CDM queue was created with id: {0}\r\n", entity.QueueId);

                    _SpisSubscriptionService.Update(entity);
                    _context.SaveChanges();
                }

                log.Debug(responseText);
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

                string msg = "SPIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Find list of subscriber identities to data</remarks>
        /// <param name="dataId">Data id</param>
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

            try
            {
                // Find all ACL objects for specified MessageID
                IEnumerable<SpisSubscription> subList;

                if (string.IsNullOrEmpty(dataId))
                {
                    subList = _SpisSubscriptionService.Get(includeProperties: "SubscriberIdentity");
                }
                else
                {
                    subList = _SpisSubscriptionService.Get(s => s.MessageID == dataId, includeProperties: "SubscriberIdentity");
                }

                // Create result list
                var response = new List<SubscriptionObject>();

                if (subList != null)
                {
                    foreach (var item in subList)
                    {
                        var subObj = new SubscriptionObject
                        {
                            MbEndpointURL = new Uri(item.MbEndpoint),
                            AmssEndpointURL = new Uri(item.AmssEndpoint),
                            IdentityId = item.SubscriberIdentity.UID,
                            IdentityName = item.SubscriberIdentity.Name
                        };
                        response.Add(subObj);
                        //_logEventService.LogSuccess(string.Format("Identity {0} found.", item.SubscriberIdentity.UID), InstanceContext.ServiceId, "STM Module",
                        //    dataId, EventCategory.Success);
                    }
                }
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

                string msg = "SPIS internal server error. " + ex.Message;
                //_logEventService.LogError(msg, InstanceContext.ServiceId, "STM Module", dataId, EventCategory.InternalError);

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

                    var mbUrl = subscriptionObject.MbEndpointURL.ToString().ToLower();
                    var amssUrl = subscriptionObject.AmssEndpointURL.ToString().ToLower();

                    var currentSub = _SpisSubscriptionService.Get(s => s.MessageID == dataId 
                    && s.SubscriberIdentity.UID == subscriptionObject.IdentityId
                    && s.AmssEndpoint == amssUrl
                    && s.MbEndpoint == mbUrl).FirstOrDefault();

                    if(currentSub != null)
                    {
                        //Delete it
                        _SpisSubscriptionService.Delete(currentSub);
                        var msg = string.Format("Subscription removed for identity {0} on messageId {1}.", subscriptionObject.IdentityId, dataId);
                        log.Info(msg);
                    }
                }

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

                string msg = "SPIS internal server error. " + ex.Message;
                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}