using STM.StmModule.Simulator.Contract;
using STM.StmModule.Simulator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace STM.StmModule.Simulator.WebApi.Controllers
{
    public class StmModulePublicController : ApiController
    {
        public StmModulePublicController()
        {

        }

        [HttpPost]
        [Route("/Notify")]
        public void Notify([FromBody]Notification notification)
        {
            NotificationHandler.Instance.RaiseNewNotification(notification);
        }
    }
}