using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.Contract
{
    public partial class VisSubscriptionObject : IEquatable<VisSubscriptionObject>
    {
        public VisSubscriptionObject()
        {

        }
        [DataMember(Name = "identityId")]
        public string IdentityId { get; set; }

        [DataMember(Name = "identityName")]
        public string IdentityName { get; set; }

        [DataMember(Name = "endpointURL")]
        public Uri EndpointURL { get; set; }

        public bool Equals(VisSubscriptionObject other)
        {
            return true;
        }
    }
}
