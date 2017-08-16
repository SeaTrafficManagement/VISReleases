using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Linq;
using System.Web;
using System;

namespace STM.Common.Services
{
    public class ServiceTypeFilter : ActionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var action = actionContext.ActionDescriptor.ActionName;

            if (string.IsNullOrEmpty(InstanceContext.NotImplementetOperations))
            {
                return;
            }

            var notImplemented = InstanceContext.NotImplementetOperations.Split(';').ToList();
            if (notImplemented.FirstOrDefault(x => x.ToLower().Trim() == action.ToLower().Trim()) != null)
            {
                throw new NotImplementedException();
            }
        }
    }
}