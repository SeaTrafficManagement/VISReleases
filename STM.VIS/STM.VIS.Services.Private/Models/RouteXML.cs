using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization;

namespace STM.VIS.Services.Private.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = "http://www.cirm.org/RTZ/1/0")]
    public partial class RouteXML
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public XmlElement RouteInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public XmlElement Waypoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public XmlElement Schedules { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public XmlElement Extensions { get; set; }
    }
}
