using System.Web.Http;
using System;
using System.Web.Http.Description;

namespace STM.SSC.Internal.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OpenIdController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public OpenIdController()
        {
        }

        [HttpGet]
        [Route("auth")]
        public String receiveOpenIdAuthCode([FromUri]string code)
        {
            log.Info("Received auth code: " + code);
            return code;
        }
    }
}
