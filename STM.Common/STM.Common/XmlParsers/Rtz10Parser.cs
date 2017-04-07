using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.XmlParsers
{
    public class Rtz10Parser : XmlParserBase, IRtzParser
    {
        public Rtz10Parser(string xml) : base(xml)
        {
            SetNamespaces(new SortedList<string, string>
            {
                {"rtz", "http://www.cirm.org/RTZ/1/0" }
            });
        }

        public string RouteStatus
        {
            get
            {
                return GetValue("//rtz:route//rtz:routeInfo/@routeStatus");
            }
        }

        public string RouteInfo
        {
            get
            {
                return GetXml("//rtz:routeInfo");
            }
        }

        public string WayPoints
        {
            get
            {
                return GetXml("//rtz:waypoints");
            }
        }

        public string VesselVoyage
        {
            get
            {
                return GetValue("//rtz:route//rtz:routeInfo/@vesselVoyage");
            }
        }

        public DateTime? ValidityPeriodStart
        {
            get
            {
                var value = GetValue("//rtz:route//rtz:routeInfo//@validityPeriodStart");

                if (value != string.Empty)
                    return Convert.ToDateTime(value);

                return null;
            }
        }

        public DateTime? ValidityPeriodStop
        {
            get
            {
                var value = GetValue("//rtz:route//rtz:routeInfo//@validityPeriodStop");

                if (value != string.Empty)
                    return Convert.ToDateTime(value);

                return null;
            }
        }
    }
}
