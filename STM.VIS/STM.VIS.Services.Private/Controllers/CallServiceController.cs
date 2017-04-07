using STM.SSC.Internal;
using STM.Common;
using STM.VIS.Services.Private.Models;
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
using STM.Common.Services.Internal.Interfaces;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class CallServiceController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ISccPrivateService _sscService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sscService"></param>
        /// <param name="connectionInformationService"></param>
        public CallServiceController (ISccPrivateService sscService, IConnectionInformationService connectionInformationService)
        {
            _sscService = sscService;
            _connectionInformationService = connectionInformationService;
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

            _logEventService.LogInfo(EventNumber.VIS_consumeService, EventDataType.Unknown, null, requestString);
            try
            {
                var result = _sscService.CallService(callServiceObj);
                var responseString = JsonConvert.SerializeObject(result, Formatting.Indented);
                _logEventService.LogSuccess(EventNumber.VIS_consumeService, EventDataType.Unknown, null, responseString);

                //Set last interaction time
                var conInfo = _connectionInformationService.Get().FirstOrDefault();
                if (conInfo == null)
                    _connectionInformationService.Insert(new ConnectionInformation { LastInteraction = DateTime.UtcNow });
                else
                {
                    conInfo.LastInteraction = DateTime.UtcNow;
                    _connectionInformationService.Update(conInfo);
                }
                _context.SaveChanges();

                return result;
            }
            catch(HttpResponseException ex)
            {
                var errorString = JsonConvert.SerializeObject(ex.Response, Formatting.Indented);
                _logEventService.LogError(EventNumber.VIS_consumeService, EventType.Error_internal, 
                    null, errorString);
                throw;
            }
            catch(Exception ex)
            {
                string msg = "VIS internal server error. " + ex.Message;

                _logEventService.LogError(EventNumber.VIS_consumeService, EventType.Error_internal,
                    null, ex.Message);

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}