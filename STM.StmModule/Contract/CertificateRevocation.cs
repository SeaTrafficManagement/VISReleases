/*
 * Maritime Cloud Identity Registry API
 *
 * Maritime Cloud Identity Registry API can be used for managing entities in the Maritime Cloud.
 *
 * OpenAPI spec version: 0.0.1
 * Contact: info@maritimecloud.net
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace STM.StmModule.Simulator.Contract
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class CertificateRevocation :  IEquatable<CertificateRevocation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateRevocation" /> class.
        /// </summary>
        /// <param name="RevokationReason">The reason the certificates has been revoked (required).</param>
        /// <param name="RevokedAt">The date the certificate revocation should be activated. (required).</param>
        public CertificateRevocation(string RevokationReason = null, DateTime? RevokedAt = null)
        {
            // to ensure "RevokationReason" is required (not null)
            if (RevokationReason == null)
            {
                throw new InvalidDataException("RevokationReason is a required property for CertificateRevocation and cannot be null");
            }
            else
            {
                this.RevokationReason = RevokationReason;
            }
            // to ensure "RevokedAt" is required (not null)
            if (RevokedAt == null)
            {
                throw new InvalidDataException("RevokedAt is a required property for CertificateRevocation and cannot be null");
            }
            else
            {
                this.RevokedAt = RevokedAt;
            }
            
        }

        /// <summary>
        /// The reason the certificates has been revoked
        /// </summary>
        /// <value>The reason the certificates has been revoked</value>
        [DataMember(Name="revokationReason")]
        public string RevokationReason { get; set; }

        /// <summary>
        /// The date the certificate revocation should be activated.
        /// </summary>
        /// <value>The date the certificate revocation should be activated.</value>
        [DataMember(Name="revokedAt")]
        public DateTime? RevokedAt { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CertificateRevocation {\n");
            sb.Append("  RevokationReason: ").Append(RevokationReason).Append("\n");
            sb.Append("  RevokedAt: ").Append(RevokedAt).Append("\n");
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
            return Equals((CertificateRevocation)obj);
        }

        /// <summary>
        /// Returns true if CertificateRevocation instances are equal
        /// </summary>
        /// <param name="other">Instance of CertificateRevocation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CertificateRevocation other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.RevokationReason == other.RevokationReason ||
                    this.RevokationReason != null &&
                    this.RevokationReason.Equals(other.RevokationReason)
                ) && 
                (
                    this.RevokedAt == other.RevokedAt ||
                    this.RevokedAt != null &&
                    this.RevokedAt.Equals(other.RevokedAt)
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
                    if (this.RevokationReason != null)
                    hash = hash * 59 + this.RevokationReason.GetHashCode();
                    if (this.RevokedAt != null)
                    hash = hash * 59 + this.RevokedAt.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CertificateRevocation left, CertificateRevocation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CertificateRevocation left, CertificateRevocation right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
