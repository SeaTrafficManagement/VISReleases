/*
 * STM Voyage Information Service STM Onboard API
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v2
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
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
    public partial class FindServicesRequestObjFilterCoverageArea :  IEquatable<FindServicesRequestObjFilterCoverageArea>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindServicesRequestObjFilterCoverageArea" /> class.
        /// </summary>
        /// <param name="CoverageType">CoverageType.</param>
        /// <param name="Value">Value.</param>
        public FindServicesRequestObjFilterCoverageArea(string CoverageType = null, string Value = null)
        {
            this.CoverageType = CoverageType;
            this.Value = Value;
            
        }

        /// <summary>
        /// Gets or Sets CoverageType
        /// </summary>
        [DataMember(Name="coverageType")]
        public string CoverageType { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [DataMember(Name="value")]
        public string Value { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FindServicesRequestObjFilterCoverageArea {\n");
            sb.Append("  CoverageType: ").Append(CoverageType).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
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
            return Equals((FindServicesRequestObjFilterCoverageArea)obj);
        }

        /// <summary>
        /// Returns true if FindServicesRequestObjFilterCoverageArea instances are equal
        /// </summary>
        /// <param name="other">Instance of FindServicesRequestObjFilterCoverageArea to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FindServicesRequestObjFilterCoverageArea other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.CoverageType == other.CoverageType ||
                    this.CoverageType != null &&
                    this.CoverageType.Equals(other.CoverageType)
                ) && 
                (
                    this.Value == other.Value ||
                    this.Value != null &&
                    this.Value.Equals(other.Value)
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
                    if (this.CoverageType != null)
                    hash = hash * 59 + this.CoverageType.GetHashCode();
                    if (this.Value != null)
                    hash = hash * 59 + this.Value.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(FindServicesRequestObjFilterCoverageArea left, FindServicesRequestObjFilterCoverageArea right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FindServicesRequestObjFilterCoverageArea left, FindServicesRequestObjFilterCoverageArea right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
