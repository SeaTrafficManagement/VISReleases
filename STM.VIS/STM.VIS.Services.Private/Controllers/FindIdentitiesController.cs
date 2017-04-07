using Microsoft.Practices.Unity;
using STM.SSC.Internal;
using STM.Common;
using STM.VIS.Services.Private.Models;
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
using STM.Common.DataAccess.Entities;
using Newtonsoft.Json;
using STM.Common.Services.Internal.Interfaces;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HMACAuthentication]
    public class FindIdentitiesController : LoggingControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISccPrivateService _sscService;
        private IConnectionInformationService _connectionInformationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sscService"></param>
        /// <param name="connectionInformationService"></param>
        [InjectionConstructor]
        public FindIdentitiesController(ISccPrivateService sscService, IConnectionInformationService connectionInformationService)
        {
            _sscService = sscService;
            _connectionInformationService = connectionInformationService;
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
        [SwaggerResponse(200, type: typeof(List<FindIdentitiesResponseObj>))]
        [SwaggerResponseContentType(responseType: "application/json", Exclusive = true)]
        public FindIdentitiesResponseObj FindIdentities()
        {
            log.Info("Incoming request to " + GetCurrentMethod());

            _logEventService.LogInfo(EventNumber.VIS_consumeService, EventDataType.None, null, null);

            try
            {
                var responsObjects = _sscService.FindIdentities();

                _logEventService.LogSuccess(EventNumber.VIS_consumeService, EventDataType.Other, null, JsonConvert.SerializeObject(responsObjects, Formatting.Indented));

                //set last interaction time
                var conInfo = _connectionInformationService.Get().FirstOrDefault();
                if (conInfo == null)
                    _connectionInformationService.Insert(new ConnectionInformation { LastInteraction = DateTime.UtcNow });
                else
                {
                    conInfo.LastInteraction = DateTime.UtcNow;
                    _connectionInformationService.Update(conInfo);
                }
                _context.SaveChanges();

                return responsObjects;
            }
            catch(HttpResponseException ex)
            {
                _logEventService.LogError(EventNumber.VIS_consumeService, EventType.Error_internal,
                    null, JsonConvert.SerializeObject(ex.Response, Formatting.Indented));

                throw;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);

                string msg = "VIS internal server error. " + ex.Message;

                _logEventService.LogError(EventNumber.VIS_consumeService, EventType.Error_internal,
                    null, ex.Message);

                throw CreateHttpResponseException(HttpStatusCode.InternalServerError, msg);
            }
        }
    }
}