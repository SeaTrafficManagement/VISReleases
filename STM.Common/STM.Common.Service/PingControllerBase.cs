using Microsoft.Practices.Unity;
using System;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using System.Web.Http.Description;

namespace STM.Common.Services
{
    public class PingControllerBase : ApiController
    {
        [Dependency]
        protected StmDbContext _context { get; set; }

        [Dependency]
        protected IMessageTypeService _messageTypeService { get; set; }

        public PingControllerBase()
        {
        }

        [HttpGet]
        [Route("ping")]
        public virtual string ping(string instance)
        {
            try
            {
                _context.init(instance);

                var types = _messageTypeService.Get();
                return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                var errorMsg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };

                throw new HttpResponseException(errorMsg);
            }
        }
    }
}