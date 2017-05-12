using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STM.StmModule.Simulator.Contract;
using STM.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Web;
using STM.StmModule.Simulator.Converters;

namespace STM.StmModule.Simulator.Services
{
    public class VisService
    {
        public static string DbName = string.Empty;
        private readonly string _visUrl;

        public VisService()
        {
            _visUrl = ConfigurationManager.AppSettings.Get("VisPrivateUrl").Replace("{database}", DbName);
        }

        public MessageEnvelope GetMessages(string dataId, int limitQuery)
        {
            var url = _visUrl + string.Format("/getMessage?limitQuery={0}&dataId={1}", limitQuery, dataId);
            var response = WebRequestHelper.Get(url);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var deserializeSetting = new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Include
                };
                var result = JsonConvert.DeserializeObject<MessageEnvelope>(response.Body, deserializeSetting);
                return result;
            }

            return null;
        }

        public IList<PublishedMessageContract> GetPublishedMessages()
        {
            var url = _visUrl + string.Format("/getPublishedMessages");
            var response = WebRequestHelper.Get(url);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<PublishedMessageContract>>(response.Body);
            }

            return new List<PublishedMessageContract>();
        }

        public string PublishMessage(string dataId,
            string messageType,
            string message)
        {
            var url = _visUrl + string.Format("/publishMessage?dataId={0}&messageType={1}", dataId, messageType);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=UTF8");

            var response = WebRequestHelper.Post(url, message, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }


        public string DeleteMessage(string dataId)
        {
            var url = _visUrl + string.Format("/publishedMessage?dataId={0}", dataId);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8");

            var response = WebRequestHelper.Delete(url, string.Empty, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }


        public List<IdentityDescriptionObject> GetAcl(string dataId)
        {
            var url = _visUrl + string.Format("/authorizeIdentities?dataId={0}", dataId);
            var response = WebRequestHelper.Get(url);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<IdentityDescriptionObject>>(response.Body);
            }

            return new List<IdentityDescriptionObject>();
        }

        public List<VisSubscriptionObject> GetSubscriptions(string dataId)
        {
            var url = _visUrl + string.Format("/subscription?dataId={0}", dataId);
            var response = WebRequestHelper.Get(url);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<VisSubscriptionObject>>(response.Body);
            }

            return new List<VisSubscriptionObject>();
        }


        public List<Organization> FindIdentties()
        {
            var result = new List<Organization>();

            var url = _visUrl + string.Format("/findIdentities");
            var response = WebRequestHelper.Get(url);

            var responseObj = JsonConvert.DeserializeObject<FindIdentitiesResponseObj>(response.Body);

            if (response.HttpStatusCode == HttpStatusCode.OK 
                && !string.IsNullOrEmpty(response.Body)
                && response.Body.Length > 35)
            {
                foreach(var org in responseObj.Organizations)
                {
                    result.Add(org);
                }
            }

            return result;
        }

        public ObservableCollection<ServiceInstance> FindServices(string coverageAreaType, string coverageArea, string unloCode, List<string> serviceProviderIds, string serviceDesignId,
            string serviceInstanceId, string mmsi, string imo, string serviceType, string serviceStatus, List<string> keyWords, string freeText, long? page, long? pageSize)
        {
            var result = new ObservableCollection<ServiceInstance>();

            var url = _visUrl + string.Format("/findServices");

            var area = new FindServicesRequestObjFilterCoverageArea(coverageAreaType, coverageArea);
            var filter = new FindServicesRequestObjFilter(area, unloCode, serviceProviderIds, serviceDesignId, serviceInstanceId,
                mmsi, imo, serviceType, serviceStatus, keyWords, freeText);
            var request = new FindServicesRequestObj(filter, page, pageSize);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application / json; charset = UTF8");

            var response = WebRequestHelper.Post(url, request.ToJson(), headers);

            try
            {
                var responseObj = JsonConvert.DeserializeObject<FindServicesResponseObj>(response.Body);
                if (responseObj != null)
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        if (responseObj.StatusCode != 200)
                        {
                            MessageBox.Show(string.Format("The find service request returned status code {0}, {1}", responseObj.StatusCode, responseObj.StatusMessage));
                        }
                        try
                        {
                            if (responseObj.ServicesInstances != null)
                            {
                                foreach (var service in responseObj.ServicesInstances)
                                {
                                    result.Add(service);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(responseObj.ServicesInstances.ToArray().ToString(), ex);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("The find service request returned status code {0}, {1}", response.HttpStatusCode, response.Body));
                }
                return result;
            }
            catch(Exception ex)
            {
                string errMessage = string.Format("Failed to get valid response from Service Registry. The reponse body does not validate towards the underlying schema: {0}", ex.Message);
                var err = new Exception(errMessage);
                throw err;
            }
            
        }


        public string CallService(string body, string url, string requestType, string contentType)
        {
            var visurl = _visUrl + string.Format("/callService");

            var result = new CallServiceResponseObj();
            var request = new CallServiceRequestObj
            {
                Body = body,
                EndpointMethod = url,
                Headers = new List<Header>
                {
                    new Header("content-type", contentType)
                },
                RequestType = requestType
            };

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8; charset = UTF8");

            var response = WebRequestHelper.Post(visurl, request.ToJson(), headers);
            return response.Body;
        }


        public string AuthorizeIdentities(string messageId, List<IdentityDescriptionObject> identities)
        {
            var url = _visUrl + string.Format("/authorizeIdentities?dataId={0}", messageId);
            var json = JsonConvert.SerializeObject(identities, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8; charset = UTF8");

            var response = WebRequestHelper.Post(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string RemoveAuthorization(string messageId, List<IdentityDescriptionObject> identities)
        {
            var url = _visUrl + string.Format("/authorizeIdentities?dataId={0}", HttpUtility.UrlEncode(messageId));
            var json = JsonConvert.SerializeObject(identities, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8; charset = UTF8");

            var response = WebRequestHelper.Delete(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string AddSubscription(string messageId, List<VisSubscriptionObject> subscriptions)
        {
            var url = _visUrl + string.Format("/subscription?dataId={0}", messageId);
            var json = JsonConvert.SerializeObject(subscriptions, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8; charset = UTF8");

            var response = WebRequestHelper.Post(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string RemoveSubscription(string messageId, List<VisSubscriptionObject> subscriptions)
        {
            var url = _visUrl + string.Format("/subscription?dataId={0}", messageId);
            var json = JsonConvert.SerializeObject(subscriptions, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF8; charset = UTF8");

            var response = WebRequestHelper.Delete(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }


        public List<Notification> GetNotifications()
        {
            var url = _visUrl + string.Format("/getNotification");
            var response = WebRequestHelper.Get(url);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<Notification>>(response.Body);
            }

            return null;
        }
    }
}