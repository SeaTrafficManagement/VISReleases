using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.Contract
{
    public partial class SpisSubscriptionObject : IEquatable<SpisSubscriptionObject>
    {
        public SpisSubscriptionObject()
        {

        }
        [DataMember(Name = "identityId")]
        public string IdentityId { get; set; }

        [DataMember(Name = "identityName")]
        public string IdentityName { get; set; }

        [DataMember(Name = "mbEndpointURL")]
        public Uri MbEndpointURL { get; set; }

        [DataMember(Name = "amssEndpointURL")]
        public Uri AmssEndpointURL { get; set; }


        public bool Equals(SpisSubscriptionObject other)
        {
            return true;
        }
    }
}
