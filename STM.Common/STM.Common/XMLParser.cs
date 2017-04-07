//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace STM.Common
//{
//    public static class XMLParser
//    {
//        private const string RTZ_10_NAMESPACE = "http://www.cirm.org/RTZ/1/0";
//        private const string RTZ_11_NAMESPACE = "http://www.cirm.org/RTZ/1/1";
//        private const string TXT_NAMESPACE = "http://tempuri.org/textMessageSchema.xsd";

//        #region Text message
//        public static string GetTextMessageId(string textMessageXML)
//        {
//            var node = GetNode(textMessageXML, "//mstns:textMessage//mstns:textMessageId",
//                new SortedList<string, string>
//                {
//                    {"mstns", TXT_NAMESPACE }
//                });

//            if (node != null)
//                return node.InnerText;

//            return string.Empty;
//        }
//        #endregion

//        #region Area message
//        public static string GetAreaMessageID(string dataSet)
//        {
//            var node = GetNode(dataSet, "//referenceUVID");

//            if (node != null)
//                return node.InnerText;

//            return string.Empty;
//        }
//        #endregion

//        #region RTZ
//        public static string GetRouteStatus(string dataSet)
//        {

//            var node = GetNode(dataSet, "//rt:routeInfo",
//                new SortedList<string, string>
//                {
//                    {"rt", RTZ_10_NAMESPACE }
//                });

//            if (node != null)
//                return node.Attributes["routeStatus"] != null ? node.Attributes["routeStatus"].Value : null;

//            return string.Empty;
//        }

//        public static string GetRouteStatusEnum(string dataSet)
//        {
//            var node = GetNode(dataSet, "//rt:routeInfo/rt:extensions/rt:extension[@manufacturer='STM'",
//            new SortedList<string, string>
//            {
//                {"rt", RTZ_10_NAMESPACE }
//            });

//            if (node != null)
//                return node.Attributes["routeStatusEnum"] != null ? node.Attributes["routeStatusEnum"].Value : null;

//            else return string.Empty;
//        }

//        public static string GetRouteInfo(string dataSet)
//        {
//            var node = GetNode(dataSet, "//rt:routeInfo",
//            new SortedList<string, string>
//            {
//                {"rt", RTZ_10_NAMESPACE }
//            });

//            if (node != null)
//                return node.OuterXml;

//            return string.Empty;
//        }

//        public static string GetWayPoints(string dataSet)
//        {
//            var node = GetNode(dataSet, "//rt:waypoints",
//                new SortedList<string, string>
//                {
//                    {"rt", RTZ_10_NAMESPACE }
//                });

//            if (node != null)
//                return node.OuterXml;

//            return string.Empty;
//        }

//        public static string GetVesselVoyage(string dataSet)
//        {
//            var node = GetNode(dataSet, "//");

//            if (node != null)
//                return node.Attributes["vesselVoyage"] != null ? node.Attributes["vesselVoyage"].Value : null;
//            else
//                return string.Empty;
//        }

//        public static DateTime? GetValidityPeriodStart(string dataSet)
//        {
//            string result;
//            var node = GetNode(dataSet, "//");

//            if (node != null)
//            {
//                result = node.Attributes["validityPeriodStart"] != null ? node.Attributes["validityPeriodStart"].Value : null;
//                return Convert.ToDateTime(result);
//            }

//            return null;
//        }

//        public static DateTime? GetValidityPeriodStop(string dataSet)
//        {
//            string result;
//            var node = GetNode(dataSet, "//");

//            if (node != null)
//            {
//                result = node.Attributes["validityPeriodStop"] != null ? node.Attributes["validityPeriodStop"].Value : null;
//                return Convert.ToDateTime(result);
//            }

//            return null;
//        }
//        #endregion

//        #region PCM
//        public static string GetPcmMessageId(string pcmMessageObject)
//        {
//            var node = GetNode(pcmMessageObject, "//");

//            if (node != null)
//            {
//                return node.Attributes["messageId"] != null ? node.Attributes["messageId"].Value : null;
//            }

//            return null;
//        }
//        #endregion

//        private static XmlNode GetNode(string xml,
//            string xpath,
//            SortedList<string, string> namespaces = null)
//        {
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(xml);
//            XmlNode root = doc.DocumentElement;

//            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

//            if (namespaces != null)
//            {
//                foreach (var item in namespaces)
//                {
//                    nsmgr.AddNamespace(item.Key, item.Value);
//                }
//            }

//            var node = root.SelectSingleNode(xpath, nsmgr);
//            return node;
//        }
//    }
//}