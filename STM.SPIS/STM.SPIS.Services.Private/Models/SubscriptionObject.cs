using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace STM.SPIS.Services.Private.Models
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SubscriptionObject : IEquatable<SubscriptionObject>
    {
        /// <summary>
        /// 
        /// </summary>
        public SubscriptionObject()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "identityId")]
        public string IdentityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "identityName")]
        public string IdentityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "mbEndpointURL")]
        public Uri MbEndpointURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "amssEndpointURL")]
        public Uri AmssEndpointURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SubscriptionObject other)
        {
            return true;
        }
    }
}
