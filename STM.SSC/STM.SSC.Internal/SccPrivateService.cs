using Newtonsoft.Json;
using STM.SSC.Internal.Models;
using STM.Common;
using STM.SSC.Internal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace STM.SSC.Internal
{
    public class SccPrivateService : ISccPrivateService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SccPrivateService()
        {
            if (!InternalWebserver.Startup.IsStarted)
                InternalWebserver.Startup.StartWebservice();
        }

        public virtual CallServiceResponseObj CallService(CallServiceRequestObj data)
        {
            var result = new CallServiceResponseObj();

            var url = data.EndpointMethod;
            log.Info(string.Format("Sending REST request to service: {0} {1}", data.RequestType, url));

            string headers = string.Empty;
            var headerCollection = new WebHeaderCollection();
            if (data.Headers != null)
            {
                foreach (var h in data.Headers)
                {
                    headers += h + " ";
                    headerCollection.Add(h.Key, h.Value);
                }
            }

            log.Info(string.Format("- using headers: {0}", headers));

            WebRequestHelper.WebResponse response = null;

            if (data.RequestType == "GET")
                response = WebRequestHelper.Get(url, headerCollection, true);
            else if (data.RequestType == "POST")
                response = WebRequestHelper.Post(url, data.Body, headerCollection, true);
            else if (data.RequestType == "DELETE")
                response = WebRequestHelper.Delete(url, data.Body, headerCollection, true);
            else if (data.RequestType == "PUT")
                response = WebRequestHelper.Put(url, data.Body, headerCollection, true);
            else
                throw new Exception(string.Format("The request type {0} is not supported.", data.RequestType));

            result.Body = response.Body;
            result.StatusCode = (int)response.HttpStatusCode;
            return result;
        }

        public virtual FindIdentitiesResponseObj FindIdentities()
        {
            log.Info(string.Format("Sending findIdentities request to IdentityRegistry"));
            var result = new FindIdentitiesResponseObj();

            try
            {
                var url = "/orgs";

                var idRegService = new IdentityRegistryService();
                var response = idRegService.MakeGenericCall(url, "GET");
                if(response.HttpStatusCode == HttpStatusCode.OK 
                    && !string.IsNullOrEmpty(response.Body) 
                    && response.Body.Length > 35)
                {
                    var responseObj = JsonConvert.DeserializeObject<IdRegistryResponeObject>(response.Body);
                    result.Organizations = responseObj.content;
                    result.StatusMessage = response.ErrorMessage;
                    result.StatusCode = (int)response.HttpStatusCode;
                }

                return result;

            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                string msg = "VIS internal server error. " + ex.Message;
                var errorMsg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(msg),
                    ReasonPhrase = "Internal error."
                };

                throw new HttpResponseException(errorMsg);
            }
        }

        public virtual IdentityRegistryGeneralResponseObj IdentityRegistryGeneralRequest(IdentityRegistryGeneralRequestObj data)
        {
            return new IdentityRegistryGeneralResponseObj();
        }

        public virtual FindServicesResponseObj FindServices(FindServicesRequestObj request)
        {
            if (request == null)
            {
                string msg = "Invalid request.";
                var errorMsg = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(msg),
                    ReasonPhrase = "Bad request."
                };

                throw new HttpResponseException(errorMsg);
            }

            log.Info(string.Format("Sending findServices {0}, {1}, {2} request to ServiceRegistry", request.Filter, request.Page, request.PageSize));
            var result = new FindServicesResponseObj();

            try
            {
                var serviceRegService = new ServiceRegistryService();
                var response = serviceRegService.FindServices(request);

                if(response.HttpStatusCode == HttpStatusCode.OK 
                    && !string.IsNullOrEmpty(response.Body))
                {
                    var responseObj = JsonConvert.DeserializeObject<List<ServiceInstance>>(response.Body);
                    result.ServicesInstances = responseObj;
                }

                result.StatusCode = (int)response.HttpStatusCode;
                result.StatusMessage = response.ErrorMessage;

                return result;
            }
            catch (HttpResponseException ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                string msg = "VIS internal server error. " + ex.Message;
                var errorMsg = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(msg),
                    ReasonPhrase = "Internal error."
                };

                throw new HttpResponseException(errorMsg);
            }
        }

        public virtual ServiceRegistryGeneralResponseObj ServiceRegistryGeneralRequest(ServiceRegistryGeneralRequestObj request)
        {
            return new ServiceRegistryGeneralResponseObj();
        }
    }
}