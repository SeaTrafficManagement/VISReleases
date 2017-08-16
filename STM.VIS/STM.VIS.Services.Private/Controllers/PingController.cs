using STM.Common.Services;
using System.Web.Http;
using System.Web.Http.Description;

namespace STM.VIS.Services.Private.Controllers
{
    /// <summary>
    /// This operation is used to ping the service. 
    /// It also verifies the connection to the database
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PingController : PingControllerBase
    {
        /// <summary>
        /// Ping service
        /// </summary>
        /// <param name="instance">Name of the database instance to use for ping</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ping")]
        public override string ping(string instance)
        {
            return base.ping(instance);
        }
    }
}