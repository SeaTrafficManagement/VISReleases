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
    public partial class VesselAttribute :  IEquatable<VesselAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VesselAttribute" /> class.
        /// </summary>
        /// <param name="AttributeName">Vessel attribute name (required).</param>
        /// <param name="AttributeValue">Vessel attribute value (required).</param>
        /// <param name="CreatedAt">CreatedAt.</param>
        /// <param name="End">End.</param>
        /// <param name="Start">Start.</param>
        /// <param name="UpdatedAt">UpdatedAt.</param>
        public VesselAttribute(string AttributeName = null, string AttributeValue = null, DateTime? CreatedAt = null, DateTime? End = null, DateTime? Start = null, DateTime? UpdatedAt = null)
        {
            // to ensure "AttributeName" is required (not null)
            if (AttributeName == null)
            {
                throw new InvalidDataException("AttributeName is a required property for VesselAttribute and cannot be null");
            }
            else
            {
                this.AttributeName = AttributeName;
            }
            // to ensure "AttributeValue" is required (not null)
            if (AttributeValue == null)
            {
                throw new InvalidDataException("AttributeValue is a required property for VesselAttribute and cannot be null");
            }
            else
            {
                this.AttributeValue = AttributeValue;
            }
            this.CreatedAt = CreatedAt;
            this.End = End;
            this.Start = Start;
            this.UpdatedAt = UpdatedAt;
            
        }

        /// <summary>
        /// Vessel attribute name
        /// </summary>
        /// <value>Vessel attribute name</value>
        [DataMember(Name="attributeName")]
        public string AttributeName { get; set; }

        /// <summary>
        /// Vessel attribute value
        /// </summary>
        /// <value>Vessel attribute value</value>
        [DataMember(Name="attributeValue")]
        public string AttributeValue { get; set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name="createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets End
        /// </summary>
        [DataMember(Name="end")]
        public DateTime? End { get; set; }

        /// <summary>
        /// Gets or Sets Start
        /// </summary>
        [DataMember(Name="start")]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets or Sets UpdatedAt
        /// </summary>
        [DataMember(Name="updatedAt")]
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class VesselAttribute {\n");
            sb.Append("  AttributeName: ").Append(AttributeName).Append("\n");
            sb.Append("  AttributeValue: ").Append(AttributeValue).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  End: ").Append(End).Append("\n");
            sb.Append("  Start: ").Append(Start).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
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
            return Equals((VesselAttribute)obj);
        }

        /// <summary>
        /// Returns true if VesselAttribute instances are equal
        /// </summary>
        /// <param name="other">Instance of VesselAttribute to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(VesselAttribute other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.AttributeName == other.AttributeName ||
                    this.AttributeName != null &&
                    this.AttributeName.Equals(other.AttributeName)
                ) && 
                (
                    this.AttributeValue == other.AttributeValue ||
                    this.AttributeValue != null &&
                    this.AttributeValue.Equals(other.AttributeValue)
                ) && 
                (
                    this.CreatedAt == other.CreatedAt ||
                    this.CreatedAt != null &&
                    this.CreatedAt.Equals(other.CreatedAt)
                ) && 
                (
                    this.End == other.End ||
                    this.End != null &&
                    this.End.Equals(other.End)
                ) && 
                (
                    this.Start == other.Start ||
                    this.Start != null &&
                    this.Start.Equals(other.Start)
                ) && 
                (
                    this.UpdatedAt == other.UpdatedAt ||
                    this.UpdatedAt != null &&
                    this.UpdatedAt.Equals(other.UpdatedAt)
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
                    if (this.AttributeName != null)
                    hash = hash * 59 + this.AttributeName.GetHashCode();
                    if (this.AttributeValue != null)
                    hash = hash * 59 + this.AttributeValue.GetHashCode();
                    if (this.CreatedAt != null)
                    hash = hash * 59 + this.CreatedAt.GetHashCode();
                    if (this.End != null)
                    hash = hash * 59 + this.End.GetHashCode();
                    if (this.Start != null)
                    hash = hash * 59 + this.Start.GetHashCode();
                    if (this.UpdatedAt != null)
                    hash = hash * 59 + this.UpdatedAt.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(VesselAttribute left, VesselAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VesselAttribute left, VesselAttribute right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
