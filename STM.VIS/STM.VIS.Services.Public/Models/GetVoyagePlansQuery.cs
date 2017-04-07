using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STM.VIS.Services.Public.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class GetVoyagePlansQuery
    {
        /// <summary>
        /// 
        /// </summary>
        public string uvid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string routeStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? routeStatusId { get; set; }
    }
}