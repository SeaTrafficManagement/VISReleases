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
    public partial class FindServicesRequestObjFilter :  IEquatable<FindServicesRequestObjFilter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindServicesRequestObjFilter" /> class.
        /// </summary>
        /// <param name="CoverageArea">CoverageArea.</param>
        /// <param name="UnLoCode">UnLoCode.</param>
        /// <param name="ServiceProviderIds">ServiceProviderIds.</param>
        /// <param name="ServiceDesignId">ServiceDesignId.</param>
        /// <param name="ServiceInstanceId">ServiceInstanceId.</param>
        /// <param name="Mmsi">Mmsi.</param>
        /// <param name="Imo">Imo.</param>
        /// <param name="ServiceType">ServiceType.</param>
        /// <param name="ServiceStatus">ServiceStatus.</param>
        /// <param name="KeyWords">KeyWords.</param>
        /// <param name="FreeText">FreeText.</param>
        public FindServicesRequestObjFilter(FindServicesRequestObjFilterCoverageArea CoverageArea = null, string UnLoCode = null, List<string> ServiceProviderIds = null, string ServiceDesignId = null, string ServiceInstanceId = null, string Mmsi = null, string Imo = null, string ServiceType = null, string ServiceStatus = null, List<string> KeyWords = null, string FreeText = null)
        {
            this.CoverageArea = CoverageArea;
            this.UnLoCode = UnLoCode;
            this.ServiceProviderIds = ServiceProviderIds;
            this.ServiceDesignId = ServiceDesignId;
            this.ServiceInstanceId = ServiceInstanceId;
            this.Mmsi = Mmsi;
            this.Imo = Imo;
            this.ServiceType = ServiceType;
            this.ServiceStatus = ServiceStatus;
            this.KeyWords = KeyWords;
            this.FreeText = FreeText;
        }

        /// <summary>
        /// Gets or Sets CoverageArea
        /// </summary>
        [DataMember(Name="coverageArea")]
        public FindServicesRequestObjFilterCoverageArea CoverageArea { get; set; }

        /// <summary>
        /// Gets or Sets UnLoCode
        /// </summary>
        [DataMember(Name="UnLoCode")]
        public string UnLoCode { get; set; }

        /// <summary>
        /// Gets or Sets ServiceProviderIds
        /// </summary>
        [DataMember(Name="ServiceProviderIds")]
        public List<string> ServiceProviderIds { get; set; }

        /// <summary>
        /// Gets or Sets ServiceDesignId
        /// </summary>
        [DataMember(Name="serviceDesignId")]
        public string ServiceDesignId { get; set; }

        /// <summary>
        /// Gets or Sets ServiceInstanceId
        /// </summary>
        [DataMember(Name="serviceInstanceId")]
        public string ServiceInstanceId { get; set; }

        /// <summary>
        /// Gets or Sets Mmsi
        /// </summary>
        [DataMember(Name="mmsi")]
        public string Mmsi { get; set; }

        /// <summary>
        /// Gets or Sets Imo
        /// </summary>
        [DataMember(Name="imo")]
        public string Imo { get; set; }

        /// <summary>
        /// Gets or Sets ServiceType
        /// </summary>
        [DataMember(Name="serviceType")]
        public string ServiceType { get; set; }

        /// <summary>
        /// Gets or Sets ServiceStatus
        /// </summary>
        [DataMember(Name="serviceStatus")]
        public string ServiceStatus { get; set; }

        /// <summary>
        /// Gets or Sets KeyWords
        /// </summary>
        [DataMember(Name="keyWords")]
        public List<string> KeyWords { get; set; }

        /// <summary>
        /// Gets or Sets FreeText
        /// </summary>
        [DataMember(Name="freeText")]
        public string FreeText { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FindServicesRequestObjFilter {\n");
            sb.Append("  CoverageArea: ").Append(CoverageArea).Append("\n");
            sb.Append("  UnLoCode: ").Append(UnLoCode).Append("\n");
            sb.Append("  ServiceProviderIds: ").Append(ServiceProviderIds).Append("\n");
            sb.Append("  ServiceDesignId: ").Append(ServiceDesignId).Append("\n");
            sb.Append("  ServiceInstanceId: ").Append(ServiceInstanceId).Append("\n");
            sb.Append("  Mmsi: ").Append(Mmsi).Append("\n");
            sb.Append("  Imo: ").Append(Imo).Append("\n");
            sb.Append("  ServiceType: ").Append(ServiceType).Append("\n");
            sb.Append("  ServiceStatus: ").Append(ServiceStatus).Append("\n");
            sb.Append("  KeyWords: ").Append(KeyWords).Append("\n");
            sb.Append("  FreeText: ").Append(FreeText).Append("\n");
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
            return Equals((FindServicesRequestObjFilter)obj);
        }

        /// <summary>
        /// Returns true if FindServicesRequestObjFilter instances are equal
        /// </summary>
        /// <param name="other">Instance of FindServicesRequestObjFilter to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FindServicesRequestObjFilter other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.CoverageArea == other.CoverageArea ||
                    this.CoverageArea != null &&
                    this.CoverageArea.Equals(other.CoverageArea)
                ) && 
                (
                    this.UnLoCode == other.UnLoCode ||
                    this.UnLoCode != null &&
                    this.UnLoCode.Equals(other.UnLoCode)
                ) && 
                (
                    this.ServiceProviderIds == other.ServiceProviderIds ||
                    this.ServiceProviderIds != null &&
                    this.ServiceProviderIds.SequenceEqual(other.ServiceProviderIds)
                ) && 
                (
                    this.ServiceDesignId == other.ServiceDesignId ||
                    this.ServiceDesignId != null &&
                    this.ServiceDesignId.Equals(other.ServiceDesignId)
                ) && 
                (
                    this.ServiceInstanceId == other.ServiceInstanceId ||
                    this.ServiceInstanceId != null &&
                    this.ServiceInstanceId.Equals(other.ServiceInstanceId)
                ) && 
                (
                    this.Mmsi == other.Mmsi ||
                    this.Mmsi != null &&
                    this.Mmsi.Equals(other.Mmsi)
                ) && 
                (
                    this.Imo == other.Imo ||
                    this.Imo != null &&
                    this.Imo.Equals(other.Imo)
                ) && 
                (
                    this.ServiceType == other.ServiceType ||
                    this.ServiceType != null &&
                    this.ServiceType.Equals(other.ServiceType)
                ) && 
                (
                    this.ServiceStatus == other.ServiceStatus ||
                    this.ServiceStatus != null &&
                    this.ServiceStatus.Equals(other.ServiceStatus)
                ) && 
                (
                    this.KeyWords == other.KeyWords ||
                    this.KeyWords != null &&
                    this.KeyWords.Equals(other.KeyWords)
                ) && 
                (
                    this.FreeText == other.FreeText ||
                    this.FreeText != null &&
                    this.FreeText.Equals(other.FreeText)
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
                    if (this.CoverageArea != null)
                    hash = hash * 59 + this.CoverageArea.GetHashCode();
                    if (this.UnLoCode != null)
                    hash = hash * 59 + this.UnLoCode.GetHashCode();
                    if (this.ServiceProviderIds != null)
                    hash = hash * 59 + this.ServiceProviderIds.GetHashCode();
                    if (this.ServiceDesignId != null)
                    hash = hash * 59 + this.ServiceDesignId.GetHashCode();
                    if (this.ServiceInstanceId != null)
                    hash = hash * 59 + this.ServiceInstanceId.GetHashCode();
                    if (this.Mmsi != null)
                    hash = hash * 59 + this.Mmsi.GetHashCode();
                    if (this.Imo != null)
                    hash = hash * 59 + this.Imo.GetHashCode();
                    if (this.ServiceType != null)
                    hash = hash * 59 + this.ServiceType.GetHashCode();
                    if (this.ServiceStatus != null)
                    hash = hash * 59 + this.ServiceStatus.GetHashCode();
                    if (this.KeyWords != null)
                    hash = hash * 59 + this.KeyWords.GetHashCode();
                    if (this.FreeText != null)
                    hash = hash * 59 + this.FreeText.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(FindServicesRequestObjFilter left, FindServicesRequestObjFilter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FindServicesRequestObjFilter left, FindServicesRequestObjFilter right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
