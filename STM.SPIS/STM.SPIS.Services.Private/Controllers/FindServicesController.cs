using Microsoft.Practices.Unity;
using STM.SSC.Internal;
using STM.Common;
using STM.SPIS.Services.Private.Models;
using STM.Common.DataAccess;
using STM.Common.Services;
using STM.Common.Services.Internal;
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
using STM.Common.DataAccess.Entities;
using Newtonsoft.Json;

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class FindServicesController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ISccPrivateService _sscService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sscService"></param>
        [InjectionConstructor]
        public FindServicesController(ISccPrivateService sscService)
        {
            _sscService = sscService;
        }

        /// <summary>
        /// Discover services in SeaSWIM service registry
        /// </summary>
        /// <param name="findServicesObj">Filter keywords and organizationId for service instances</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be authenticated in the Service Registry)</response>
        /// <response code="403">Forbidden (the user is not authorized to requested service)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("findServices")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public FindServicesResponseObj FindServices([FromBody]FindServicesRequestObj findServicesObj)
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            var requestString = JsonConvert.SerializeObject(findServicesObj, Formatting.Indented);
            _logEventService.LogInfo(EventNumber.SPIS_consumeService, EventDataType.None, null, requestString);

            try
            {
                var result = _sscService.FindServices(findServicesObj);

                _logEventService.LogSuccess(EventNumber.SPIS_consumeService, EventDataType.Other, null, JsonConvert.SerializeObject(result, Formatting.Indented));
                return result;
            }
            catch (HttpResponseException ex)
            {
                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal, null,
                    JsonConvert.SerializeObject(requestString, Formatting.Indented) + " " + ex.Response.ToString());

                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "SPIS internal server error. " + ex.Message;
                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal, null,
                    JsonConvert.SerializeObject(requestString, Formatting.Indented) + " " + ex.Message);

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}