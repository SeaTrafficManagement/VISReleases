using Microsoft.Practices.Unity;
using STM.Common;
using STM.SPIS.Services.Private.Models;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using STM.Common.Services.Internal;
using STM.Common.Services.Internal.Interfaces;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using STM.Common.Services;

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class AuthorizeIdentitiesController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IIdentityService _identityService;
        private IACLObjectService _aclObjectService;
        private ISpisSubscriptionService _SpisSubscriptionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityService"></param>
        /// <param name="aclObjectService"></param>
        /// <param name="SpisSubscriptionService"></param>
        [InjectionConstructor]
        public AuthorizeIdentitiesController(IIdentityService identityService,
            IACLObjectService aclObjectService, ISpisSubscriptionService SpisSubscriptionService)
        {
            _identityService = identityService;
            _aclObjectService = aclObjectService;
            _SpisSubscriptionService = SpisSubscriptionService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Authorize identities to Voyage Plan</remarks>
        /// <param name="dataId">Data id of the PCM message</param>
        /// <param name="identityDescriptionObjects">Attributes to describe an organization in STM</param>
        /// <response code="200">Authorization of identities successful!</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("authorizeIdentities")]
        [SwaggerOperation("AuthorizeIdentities")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        [SwaggerResponse(200, type: typeof(ResponseObj))]
        public ResponseObj AuthorizeIdentities([FromUri]string dataId,
            [FromBody]List<IdentityDescriptionObject> identityDescriptionObjects)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            try
            {
                foreach (var identityObject in identityDescriptionObjects)
                {
                    // CHeck if the identity already exists in the local identity table
                    var identity = _identityService.Get(x => x.UID == identityObject.IdentityId).FirstOrDefault();

                    // If not add new identity
                    if (identity == null)
                    {
                        identity = new STM.Common.DataAccess.Entities.Identity
                        {
                            UID = identityObject.IdentityId,
                            Name = identityObject.IdentityName
                        };

                        _identityService.Insert(identity);
                    };

                    // Get ACL for identity and messageID
                    var current = _aclObjectService.Get(x
                        => x.MessageID == dataId && x.Subscriber.ID == identity.ID).FirstOrDefault();

                    // If not already exists add new ACL
                    if (current == null)
                    {
                        var acl = new ACLObject();
                        acl.MessageID = dataId;
                        acl.LastUpdateTime = DateTime.UtcNow;
                        acl.Subscriber = identity;
                        _aclObjectService.Insert(acl);
                    }

                    log.Info(string.Format("ACL added for identity {0} on messageId {1}", identity.Name, dataId));

                }

                // Save to DB
                _context.SaveChanges();

                // Create response
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
        /// <remarks>Find list of authorized identities to data</remarks>
        /// <param name="dataId">Data id of PCM message</param>
        /// <response code="200">Find list of authorized identities successful!</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found ( requested identities are not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [SwaggerOperation("FindAuthorizedIdentities")]
        [SwaggerResponse(200, type: typeof(List<IdentityDescriptionObject>))]
        [HttpGet]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [Route("authorizeIdentities")]
        public IList<IdentityDescriptionObject> FindAuthorizedIdentities([FromUri]string dataId)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (string.IsNullOrEmpty(dataId))
            {
                var msg = "Missing required query parameter dataId.";
                log.Error(msg);
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, msg);
            }

            try
            {
                // Find all ACL objects for specified MessageID
                var aclList = _aclObjectService.Get(x =>
                    x.MessageID == dataId, includeProperties: "Subscriber");

                // Create result list
                var response = new List<IdentityDescriptionObject>();

                if (aclList != null)
                {
                    foreach (var item in aclList)
                    {
                        response.Add(new IdentityDescriptionObject(item.Subscriber.UID, item.Subscriber.Name));
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

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }

        /// <summary>
        /// Remove Authorize identities to data
        /// </summary>
        /// <param name="dataId">Data id of the PCM message</param>
        /// <param name="identityDescriptionObjects">Attributes to describe an organization in STM</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found ( requested identities are not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpDelete]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        [Route("authorizeIdentities")]
        public ResponseObj RemoveAuthorizedIdentitites([FromUri]string dataId,
            [FromBody]List<IdentityDescriptionObject> identityDescriptionObjects)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            if (dataId == null)
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "Missing dataID");
            }

            if (identityDescriptionObjects == null || identityDescriptionObjects.Count == 0)
            {
                throw CreateHttpResponseException(HttpStatusCode.BadRequest, "No identities to remove");
            }

            try
            {
                foreach (var identityDescriptionObject in identityDescriptionObjects)
                {
                    // Get matching ACL-object from DB
                    var identityToDelete = _identityService.Get(i => i.UID == identityDescriptionObject.IdentityId).FirstOrDefault();
                    if(identityToDelete == null)
                    {
                        log.Info(string.Format("Identity {0} did not exist for messageId {1}.", identityDescriptionObject.IdentityId, dataId));
                        return new ResponseObj(dataId);
                    }

                    var current = _aclObjectService.Get(x
                        => x.MessageID == dataId && x.Subscriber.ID == identityToDelete.ID, includeProperties: "Subscriber").FirstOrDefault();

                    // If it exists, delete it
                    if (current != null)
                    {
                        var subscribers = _SpisSubscriptionService.Get(s => 
                            s.MessageID == dataId 
                            && s.SubscriberIdentity.ID == current.Subscriber.ID);

                        _aclObjectService.Delete(current);
                        var msg = string.Format("ACL removed for identity {0} on messageId {1}.", identityDescriptionObject.IdentityId, dataId);
                        log.Info(msg);

                        // Remove subscriber
                        if(subscribers != null)
                        {
                            foreach (var subscriber in subscribers)
                            {
                                msg = string.Format("Subscriber {0} removed on messageId {1}", subscriber.SubscriberIdentity.UID, dataId);
                                _SpisSubscriptionService.Delete(subscriber);
                                log.Info(msg);
                            }
                        }
                    }
                }

                _context.SaveChanges();

                return new ResponseObj(dataId); ;
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