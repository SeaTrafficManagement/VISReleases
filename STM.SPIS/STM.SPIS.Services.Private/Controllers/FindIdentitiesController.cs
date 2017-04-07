using Microsoft.Practices.Unity;
using STM.SSC.Internal;
using STM.Common;
using STM.Common.DataAccess.Entities;
using STM.SPIS.Services.Private.Models;
using STM.Common.DataAccess;
using STM.Common.Services.Internal;
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
using STM.SSC.Internal.Models;
using STM.Common.Services;
using Newtonsoft.Json;

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class FindIdentitiesController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISccPrivateService _sscService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sscService"></param>
        [InjectionConstructor]
        public FindIdentitiesController(ISccPrivateService sscService)
        {
            _sscService = sscService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Seacrh for identities in STM identity registry</remarks>
        /// <response code="200">Success, organizations found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden (the user is not authorized to requested organization)</response>
        /// <response code="404">Not Found (the requested identity is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpGet]
        [Route("findIdentities")]
        [SwaggerOperation("FindIdentities")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerResponse(200, type: typeof(List<FindIdentitiesResponseObj>))]
        public FindIdentitiesResponseObj FindIdentities()
        {
            log.Info("Incoming request to " + GetCurrentMethod());
            
            //_logEventService.Init(Common.DataAccess.Entities.EventDataType.Other);
            _logEventService.LogInfo(EventNumber.SPIS_consumeService, EventDataType.None, null, null);

            try
            {
                var responsObjects = _sscService.FindIdentities();

                _logEventService.LogSuccess(EventNumber.SPIS_consumeService, EventDataType.Other, null,
                    JsonConvert.SerializeObject(responsObjects, Formatting.Indented));

                return responsObjects;
            }
            catch(HttpResponseException ex)
            {
                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal,
                    null, ex.Response.ToString());

                throw;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "SPIS internal server error. " + ex.Message;
                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal,
                    null, ex.Message);

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}