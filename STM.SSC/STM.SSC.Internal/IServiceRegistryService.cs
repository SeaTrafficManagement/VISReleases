using STM.SSC.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace STM.SSC.Internal
{
    public interface IServiceRegistryService
    {
        Common.WebRequestHelper.WebResponse MakeGenericCall(string url, string method, string body = null, WebHeaderCollection headers = null);

        Common.WebRequestHelper.WebResponse FindServices(FindServicesRequestObj data);
    }
}
