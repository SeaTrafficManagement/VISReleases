using STM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using STM.SSC.Internal.Models;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Http;

namespace STM.SSC.Internal
{
    public class ServiceRegistryService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string PATH_SEARCH_GENERIC = "/api/_search/serviceInstance?query=";
        private const string PATH_SEARCH_GeoJSON = "/api/_searchGeometryGeoJSON/serviceInstance?geometry=[GEOMETRY]&query=";
        private const string PATH_SEARCH_WKT = "/api/_searchGeometryWKT/serviceInstance?geometry=[GEOMETRY]&query=";
        private const string PATH_ALL = "/api/serviceInstance";

        private string serviceRegistryBasePath;
        private IdentityRegistryService IdentityRegistryService;

        public ServiceRegistryService()
        {
            serviceRegistryBasePath = ConfigurationManager.AppSettings.Get("ServiceRegistryBaseUrl");
            IdentityRegistryService = new IdentityRegistryService();
        }

        public WebRequestHelper.WebResponse MakeGenericCall(string url, string method, string body = null, WebHeaderCollection headers = null)
        {
            log.Info("Make generic call to ServiceRegistry");

            WebRequestHelper.WebResponse response = null;

            var token = IdentityRegistryService.GetAccessToken();
            if (headers == null)
                headers = new WebHeaderCollection();

            headers.Add("Authorization", "Bearer " + token);

            url = serviceRegistryBasePath + url;
            if (method == "GET")
                response = WebRequestHelper.Get(url, headers, false);
            else if (method == "POST")
                response = WebRequestHelper.Post(url, body, headers: headers, UseCertificate: false);

            return response;
        }

        public WebRequestHelper.WebResponse FindServices(FindServicesRequestObj data)
        {
            string url = string.Empty;
            bool isGeoSearch = false;

            if (data.Filter.CoverageArea != null 
                && !string.IsNullOrEmpty(data.Filter.CoverageArea.Value))
            {
                if (data.Filter.CoverageArea.CoverageType == "WKT")
                    url = PATH_SEARCH_WKT.Replace("[GEOMETRY]", HttpUtility.UrlEncode(data.Filter.CoverageArea.Value));
                if (data.Filter.CoverageArea.CoverageType == "GeoJSON")
                    url = PATH_SEARCH_GeoJSON.Replace("[GEOMETRY]", HttpUtility.UrlEncode(data.Filter.CoverageArea.Value));

                isGeoSearch = true;
            }
            else
            {
                url = PATH_SEARCH_GENERIC;
            }

            string query = string.Empty;
            if (!string.IsNullOrEmpty(data.Filter.FreeText))
            {
                if(FormatValidation.IsValidFreeText(data.Filter.FreeText))
                {
                    query = data.Filter.FreeText.Replace(":", "\\:");
                    query = HttpUtility.UrlEncode(query);
                }
                else
                {
                    string msg = "Forbidden character(s) in freetext search string.";
                    var errorMsg = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(msg),
                        ReasonPhrase = "Bad request."
                    };
                    throw new HttpResponseException(errorMsg);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(data.Filter.UnLoCode))
                {
                    query = AddToQuery(query, "UnLoCode", data.Filter.UnLoCode, "AND");
                }

                if (!string.IsNullOrEmpty(data.Filter.ServiceDesignId))
                {
                    query = AddToQuery(query, "designId", data.Filter.ServiceDesignId.Replace(":", "\\:"), "AND");
                }

                if (!string.IsNullOrEmpty(data.Filter.ServiceInstanceId))
                {
                    query = AddToQuery(query, "instanceId", data.Filter.ServiceInstanceId.Replace(":", "\\:"), "AND");
                }

                if (!string.IsNullOrEmpty(data.Filter.ServiceType))
                {
                    query = AddToQuery(query, "serviceType", data.Filter.ServiceType, "AND");
                }

                if (!string.IsNullOrEmpty(data.Filter.ServiceStatus))
                {
                    query = AddToQuery(query, "status", data.Filter.ServiceStatus, "AND");
                }

                if (!string.IsNullOrEmpty(data.Filter.Mmsi) && !string.IsNullOrEmpty(data.Filter.Imo))
                {
                    if (query == string.Empty)
                        query += "(";
                    else
                        query += " AND (";

                    query = AddToQuery(query, "mmsi", data.Filter.Mmsi, "OR");
                    query = AddToQuery(query, "imo", data.Filter.Imo, "OR");
                    query += ")";
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.Filter.Mmsi))
                    {
                        query = AddToQuery(query, "mmsi", data.Filter.Mmsi, "AND");
                    }

                    if (!string.IsNullOrEmpty(data.Filter.Imo))
                    {
                        query = AddToQuery(query, "imo", data.Filter.Imo, "AND");
                    }
                }

                if (data.Filter.ServiceProviderIds != null && data.Filter.ServiceProviderIds.Count > 0)
                {
                    if (query == string.Empty)
                        query += "(";
                    else
                        query += " AND (";

                    foreach (var id in data.Filter.ServiceProviderIds)
                    {
                        query = AddToQuery(query, "organizationId", id.Replace(":", "\\:"), "OR");
                    }
                    query += ")";
                }

                if (data.Filter.ServiceProviderIds != null && data.Filter.ServiceProviderIds.Count > 0)
                {
                    if (query == string.Empty)
                        query += "(";
                    else
                        query += " AND (";

                    foreach (var id in data.Filter.ServiceProviderIds)
                    {
                        query = AddToQuery(query, "organizationId", id.Replace(":", "\\:"), "OR");
                    }
                    query += ")";
                }

                if (data.Filter.KeyWords != null && data.Filter.KeyWords.Count > 0)
                {
                    if (query == string.Empty)
                        query += "(";
                    else
                        query += " AND (";

                    foreach (var id in data.Filter.KeyWords)
                    {
                        query = AddToQuery(query, "keywords", id.Replace(":", "\\:"), "AND");
                    }
                    query += ")";
                }
            }

            if (string.IsNullOrEmpty(query) && !isGeoSearch)
            {
                url = PATH_ALL;
            }

            if (data.Page != null)
            { 
                if (url == PATH_ALL)
                    query += "?page=" + data.Page.ToString();
                else
                    query += "&page=" + data.Page.ToString();
            }
            if (data.PageSize != null)
            {
                query += "&size=" + data.PageSize.ToString();
            }

            return MakeGenericCall(url + query, "GET");
        }

        private string AddToQuery(string query, string key, string value, string op)
        {
            var sb = new StringBuilder(query);
            if (sb.Length > 0 && !query.EndsWith("("))
                sb.Append(string.Format(" {0} {1}:{2}", op, key, value));
            else
                sb.Append(string.Format("{0}:{1}", key, value));

            return sb.ToString();
        }
    }
}