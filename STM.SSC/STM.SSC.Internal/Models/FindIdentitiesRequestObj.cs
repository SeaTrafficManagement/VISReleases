using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace STM.SSC.Internal.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class FindIdentitiesRequestObj :  IEquatable<FindIdentitiesRequestObj>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindIdentitiesRequestObj" /> class.
        /// </summary>
        /// <param name="OrganisationId">OrganisationId.</param>
        public FindIdentitiesRequestObj(string OrganisationId = null)
        {
            this.OrganisationId = OrganisationId;
            
        }

        /// <summary>
        /// Gets or Sets OrganisationId
        /// </summary>
        [DataMember(Name="organisationId")]
        public string OrganisationId { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FindIdentitiesRequestObj {\n");
            sb.Append("  OrganisationId: ").Append(OrganisationId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FindIdentitiesRequestObj)obj);
        }

        /// <summary>
        /// Returns true if FindIdentitiesRequestObj instances are equal
        /// </summary>
        /// <param name="other">Instance of FindIdentitiesRequestObj to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FindIdentitiesRequestObj other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.OrganisationId == other.OrganisationId ||
                    this.OrganisationId != null &&
                    this.OrganisationId.Equals(other.OrganisationId)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                    if (this.OrganisationId != null)
                    hash = hash * 59 + this.OrganisationId.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(FindIdentitiesRequestObj left, FindIdentitiesRequestObj right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FindIdentitiesRequestObj left, FindIdentitiesRequestObj right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
