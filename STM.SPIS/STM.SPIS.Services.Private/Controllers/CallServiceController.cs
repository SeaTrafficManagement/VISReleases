using STM.SSC.Internal;
using STM.Common;
using STM.SPIS.Services.Private.Models;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
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
using STM.Common.Services;
using Newtonsoft.Json;

namespace STM.SPIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class CallServiceController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ISccPrivateService _sscService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sscService"></param>
        public CallServiceController (ISccPrivateService sscService)
        {
            _sscService = sscService;
        }

        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized (the user cannot be auhtenticated in the Identity Registry)</response>
        /// <response code="403">Forbidden (the user is not authorized to requested service)</response>
        /// <response code="404">Not Found (the requested service endpoint is not found)</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="default">unexpected error</response>
        [HttpPost]
        [Route("callService")]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        [SwaggerRequestContentType(requestType: "application/json", Exclusive = true)]
        public CallServiceResponseObj CallService([FromBody]CallServiceRequestObj callServiceObj)
        {
            log.Info("Incoming request to " + GetCurrentMethod());
            var requestString = JsonConvert.SerializeObject(callServiceObj, Formatting.Indented);
            //_logEventService.Init(EventDataType.PCM);
            _logEventService.LogInfo(EventNumber.SPIS_consumeService, EventDataType.Unknown, null, requestString);

            try
            {
                var result = _sscService.CallService(callServiceObj);
                return result;
            }
            catch(HttpResponseException ex)
            {
                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal,
                    null, requestString + " " + ex.Response.ToString());

                throw;
            }
            catch(Exception ex)
            {
                string msg = "SPIS internal server error. " + ex.Message;

                _logEventService.LogError(EventNumber.SPIS_consumeService, EventType.Error_internal,
                    null, requestString + " " + ex.Message);

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}