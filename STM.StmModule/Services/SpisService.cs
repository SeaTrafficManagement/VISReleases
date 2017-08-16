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

namespace STM.StmModule.Simulator.Services
{
    public class SpisService
    {
        public static string DbName = string.Empty;
        private readonly string _spisUrl;

        public SpisService()
        {
            _spisUrl = ConfigurationManager.AppSettings.Get("SpisPrivateUrl").Replace("{database}", DbName); ;
        }

        public MessageEnvelope GetMessages(string dataId, int limitQuery, DateTime? fromTime, DateTime? toTime)
        {
            string url = string.Empty;

            if (fromTime == null)
            {
                url = _spisUrl + string.Format("/getMessage?limitQuery={0}&dataId={1}", limitQuery, Uri.EscapeDataString(dataId ?? string.Empty));
            }
            else
            {
                if (toTime == null)
                    toTime = DateTime.MaxValue;

                url = _spisUrl + string.Format("/getMessageInTimeIntervall?limitQuery={0}&dataId={1}&fromDate={2}&toDate={3}",
                    limitQuery, Uri.EscapeDataString(dataId ?? string.Empty),
                    Uri.EscapeDataString(fromTime.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    Uri.EscapeDataString(toTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));
            }

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

            MessageBox.Show(response.HttpStatusCode + " " + response.ErrorMessage);
            return null;
        }

        public IList<PublishedMessageContract> GetPublishedMessages()
        {
            var url = _spisUrl + string.Format("/getPublishedMessages");
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
            var url = _spisUrl + string.Format("/publishMessage?dataId={0}&messageType={1}", Uri.EscapeDataString(dataId ?? string.Empty), messageType);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");

            //byte[] bytes = Encoding.Default.GetBytes(message);
            //message = Encoding.UTF8.GetString(bytes);

            var response = WebRequestHelper.Post(url, message, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string DeleteMessage(string dataId)
        {
            var url = _spisUrl + string.Format("/publishedMessage?dataId={0}", Uri.EscapeDataString(dataId ?? string.Empty));

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Delete(url, string.Empty, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public List<IdentityDescriptionObject> GetAcl(string dataId)
        {
            var url = _spisUrl + string.Format("/authorizeIdentities?dataId={0}", Uri.EscapeDataString(dataId ?? string.Empty));
            var response = WebRequestHelper.Get(url);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<IdentityDescriptionObject>>(response.Body);
            }

            return new List<IdentityDescriptionObject>();
        }

        public List<SpisSubscriptionObject> GetSubscriptions(string dataId)
        {
            var url = _spisUrl + string.Format("/subscription?dataId={0}", Uri.EscapeDataString(dataId ?? string.Empty));
            var response = WebRequestHelper.Get(url);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<SpisSubscriptionObject>>(response.Body);
            }

            return new List<SpisSubscriptionObject>();
        }

        public List<Organization> FindIdentties()
        {
            var result = new List<Organization>();

            var url = _spisUrl + string.Format("/findIdentities");
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

            var url = _spisUrl + string.Format("/findServices");

            var area = new FindServicesRequestObjFilterCoverageArea(coverageAreaType, coverageArea);
            var filter = new FindServicesRequestObjFilter(area, unloCode, serviceProviderIds, serviceDesignId, serviceInstanceId,
                mmsi, imo, serviceType, serviceStatus, keyWords, freeText);
            var request = new FindServicesRequestObj(filter, page, pageSize);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Post(url, request.ToJson(), headers);

            var responseObj = JsonConvert.DeserializeObject<FindServicesResponseObj>(response.Body);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                try
                {
                    foreach (var service in responseObj.ServicesInstances)
                    {
                        result.Add(service);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(responseObj.ServicesInstances.ToArray().ToString(), ex);
                }
            }
            return result;
        }

        public string CallService(string body, string url, string requestType, string contentType)
        {
            var Spisurl = _spisUrl + string.Format("/callService");

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
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Post(Spisurl, request.ToJson(), headers);
            return response.Body;
        }

        public string AuthorizeIdentities(string messageId, List<IdentityDescriptionObject> identities)
        {
            var url = _spisUrl + string.Format("/authorizeIdentities?dataId={0}", Uri.EscapeDataString(messageId));
            var json = JsonConvert.SerializeObject(identities, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Post(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string RemoveAuthorization(string messageId, List<IdentityDescriptionObject> identities)
        {
            var url = _spisUrl + string.Format("/authorizeIdentities?dataId={0}", Uri.EscapeDataString(messageId));
            var json = JsonConvert.SerializeObject(identities, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Delete(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string AddSubscription(string messageId, List<SpisSubscriptionObject> subscriptions)
        {
            var url = _spisUrl + string.Format("/subscription?dataId={0}", Uri.EscapeDataString(messageId));
            var json = JsonConvert.SerializeObject(subscriptions, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Post(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public string RemoveSubscription(string messageId, List<SpisSubscriptionObject> subscriptions)
        {
            var url = _spisUrl + string.Format("/subscription?dataId={0}", Uri.EscapeDataString(messageId));
            var json = JsonConvert.SerializeObject(subscriptions, Formatting.Indented);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");

            var response = WebRequestHelper.Delete(url, json, headers);
            return response.HttpStatusCode.ToString() + " " + response.ErrorMessage + response.Body;
        }

        public List<Notification> GetNotifications()
        {
            var url = _spisUrl + string.Format("/getNotification");
            var response = WebRequestHelper.Get(url);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<Notification>>(response.Body);
            }

            return null;
        }
    }
}