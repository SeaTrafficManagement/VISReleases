using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace STM.VIS.Services.Private.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class AuthorizationObject : IEquatable<AuthorizationObject>
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthorizationObject()
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
        [DataMember(Name ="identityName")]
        public string IdentityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name ="serviceInstanceId")]
        public string ServiceInstanceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name ="endpointURL")]
        public Uri EndpointURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AuthorizationObject other)
        {
            return true;
        }
    }
}
