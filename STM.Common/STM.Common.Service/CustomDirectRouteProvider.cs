using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace STM.Common.Services
{
    public class CustomDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var routePrefix = base.GetRoutePrefix(controllerDescriptor);

            var controllerBaseType = controllerDescriptor.ControllerType.BaseType;

            if (controllerBaseType == typeof(LoggingControllerBase))
            {
                routePrefix = "{instance}";
            }

            return routePrefix;
        }
    }
}
