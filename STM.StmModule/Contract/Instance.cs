/*
 * Maritime Cloud Service Registry API
 *
 * Maritime Cloud Service Registry, developed under the Horizon 2020 Project EfficienSea2, cofunded by the European Union.
 *
 * OpenAPI spec version: 1.0
 * Contact: josef.jahn@frequentis.com
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
    /// Holds a description of an service instance.An instance can be compatible to one or morespecification templates.It has at least a technical representation of thedescriptiion in form of an XML and a filled out templateas e.g. word document.
    /// </summary>
    [DataContract]
    public partial class Instance :  IEquatable<Instance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instance" /> class.
        /// </summary>
        /// <param name="Comment">Comment.</param>
        /// <param name="Designs">Designs.</param>
        /// <param name="Docs">Docs.</param>
        /// <param name="EndpointType">EndpointType.</param>
        /// <param name="EndpointUri">EndpointUri.</param>
        /// <param name="Geometry">Geometry.</param>
        /// <param name="GeometryContentType">GeometryContentType.</param>
        /// <param name="Id">Id.</param>
        /// <param name="ImplementedSpecificationVersion">ImplementedSpecificationVersion.</param>
        /// <param name="InstanceAsDoc">InstanceAsDoc.</param>
        /// <param name="InstanceAsXml">InstanceAsXml.</param>
        /// <param name="InstanceId">InstanceId.</param>
        /// <param name="Keywords">Keywords.</param>
        /// <param name="Name">Name.</param>
        /// <param name="OrganizationId">OrganizationId.</param>
        /// <param name="Status">Status.</param>
        /// <param name="Unlocode">Unlocode.</param>
        /// <param name="Version">Version.</param>
        public Instance(string Comment = null, List<Doc> Docs = null, string EndpointType = null, string EndpointUri = null, JsonNode Geometry = null, string GeometryContentType = null, long? Id = null, Doc InstanceAsDoc = null, Xml InstanceAsXml = null, string InstanceId = null, string Keywords = null, string Name = null, string OrganizationId = null, string Status = null, string Unlocode = null, string Version = null)
        {
            this.Comment = Comment;
            this.Docs = Docs;
            this.EndpointType = EndpointType;
            this.EndpointUri = EndpointUri;
            this.Geometry = Geometry;
            this.GeometryContentType = GeometryContentType;
            this.Id = Id;
            this.InstanceAsDoc = InstanceAsDoc;
            this.InstanceAsXml = InstanceAsXml;
            this.InstanceId = InstanceId;
            this.Keywords = Keywords;
            this.Name = Name;
            this.OrganizationId = OrganizationId;
            this.Status = Status;
            this.Unlocode = Unlocode;
            this.Version = Version;
            
        }

        /// <summary>
        /// Gets or Sets Comment
        /// </summary>
        [DataMember(Name="comment")]
        public string Comment { get; set; }


        /// <summary>
        /// Gets or Sets Docs
        /// </summary>
        [DataMember(Name="docs")]
        public List<Doc> Docs { get; set; }

        /// <summary>
        /// Gets or Sets EndpointType
        /// </summary>
        [DataMember(Name="endpointType")]
        public string EndpointType { get; set; }

        /// <summary>
        /// Gets or Sets EndpointUri
        /// </summary>
        [DataMember(Name="endpointUri")]
        public string EndpointUri { get; set; }

        /// <summary>
        /// Gets or Sets Geometry
        /// </summary>
        [DataMember(Name="geometry")]
        public JsonNode Geometry { get; set; }

        /// <summary>
        /// Gets or Sets GeometryContentType
        /// </summary>
        [DataMember(Name="geometryContentType")]
        public string GeometryContentType { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id")]
        public long? Id { get; set; }


        /// <summary>
        /// Gets or Sets InstanceAsDoc
        /// </summary>
        [DataMember(Name="instanceAsDoc")]
        public Doc InstanceAsDoc { get; set; }

        /// <summary>
        /// Gets or Sets InstanceAsXml
        /// </summary>
        [DataMember(Name="instanceAsXml")]
        public Xml InstanceAsXml { get; set; }

        /// <summary>
        /// Gets or Sets InstanceId
        /// </summary>
        [DataMember(Name="instanceId")]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or Sets Keywords
        /// </summary>
        [DataMember(Name="keywords")]
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets OrganizationId
        /// </summary>
        [DataMember(Name="organizationId")]
        public string OrganizationId { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name="status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or Sets Unlocode
        /// </summary>
        [DataMember(Name="unlocode")]
        public string Unlocode { get; set; }

        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        [DataMember(Name="version")]
        public string Version { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Instance {\n");
            sb.Append("  Comment: ").Append(Comment).Append("\n");
            sb.Append("  Docs: ").Append(Docs).Append("\n");
            sb.Append("  EndpointType: ").Append(EndpointType).Append("\n");
            sb.Append("  EndpointUri: ").Append(EndpointUri).Append("\n");
            sb.Append("  Geometry: ").Append(Geometry).Append("\n");
            sb.Append("  GeometryContentType: ").Append(GeometryContentType).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  InstanceAsDoc: ").Append(InstanceAsDoc).Append("\n");
            sb.Append("  InstanceAsXml: ").Append(InstanceAsXml).Append("\n");
            sb.Append("  InstanceId: ").Append(InstanceId).Append("\n");
            sb.Append("  Keywords: ").Append(Keywords).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  OrganizationId: ").Append(OrganizationId).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Unlocode: ").Append(Unlocode).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
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
            return Equals((Instance)obj);
        }

        /// <summary>
        /// Returns true if Instance instances are equal
        /// </summary>
        /// <param name="other">Instance of Instance to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Instance other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Comment == other.Comment ||
                    this.Comment != null &&
                    this.Comment.Equals(other.Comment)
                ) && 
                (
                    this.Docs == other.Docs ||
                    this.Docs != null &&
                    this.Docs.SequenceEqual(other.Docs)
                ) && 
                (
                    this.EndpointType == other.EndpointType ||
                    this.EndpointType != null &&
                    this.EndpointType.Equals(other.EndpointType)
                ) && 
                (
                    this.EndpointUri == other.EndpointUri ||
                    this.EndpointUri != null &&
                    this.EndpointUri.Equals(other.EndpointUri)
                ) && 
                (
                    this.Geometry == other.Geometry ||
                    this.Geometry != null &&
                    this.Geometry.Equals(other.Geometry)
                ) && 
                (
                    this.GeometryContentType == other.GeometryContentType ||
                    this.GeometryContentType != null &&
                    this.GeometryContentType.Equals(other.GeometryContentType)
                ) && 
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) && 
                (
                    this.InstanceAsDoc == other.InstanceAsDoc ||
                    this.InstanceAsDoc != null &&
                    this.InstanceAsDoc.Equals(other.InstanceAsDoc)
                ) && 
                (
                    this.InstanceAsXml == other.InstanceAsXml ||
                    this.InstanceAsXml != null &&
                    this.InstanceAsXml.Equals(other.InstanceAsXml)
                ) && 
                (
                    this.InstanceId == other.InstanceId ||
                    this.InstanceId != null &&
                    this.InstanceId.Equals(other.InstanceId)
                ) && 
                (
                    this.Keywords == other.Keywords ||
                    this.Keywords != null &&
                    this.Keywords.Equals(other.Keywords)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) && 
                (
                    this.OrganizationId == other.OrganizationId ||
                    this.OrganizationId != null &&
                    this.OrganizationId.Equals(other.OrganizationId)
                ) && 
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
                ) && 
                (
                    this.Unlocode == other.Unlocode ||
                    this.Unlocode != null &&
                    this.Unlocode.Equals(other.Unlocode)
                ) && 
                (
                    this.Version == other.Version ||
                    this.Version != null &&
                    this.Version.Equals(other.Version)
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
                    if (this.Comment != null)
                    hash = hash * 59 + this.Comment.GetHashCode();
                    if (this.Docs != null)
                    hash = hash * 59 + this.Docs.GetHashCode();
                    if (this.EndpointType != null)
                    hash = hash * 59 + this.EndpointType.GetHashCode();
                    if (this.EndpointUri != null)
                    hash = hash * 59 + this.EndpointUri.GetHashCode();
                    if (this.Geometry != null)
                    hash = hash * 59 + this.Geometry.GetHashCode();
                    if (this.GeometryContentType != null)
                    hash = hash * 59 + this.GeometryContentType.GetHashCode();
                    if (this.Id != null)
                    hash = hash * 59 + this.Id.GetHashCode();
                    if (this.InstanceAsDoc != null)
                    hash = hash * 59 + this.InstanceAsDoc.GetHashCode();
                    if (this.InstanceAsXml != null)
                    hash = hash * 59 + this.InstanceAsXml.GetHashCode();
                    if (this.InstanceId != null)
                    hash = hash * 59 + this.InstanceId.GetHashCode();
                    if (this.Keywords != null)
                    hash = hash * 59 + this.Keywords.GetHashCode();
                    if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                    if (this.OrganizationId != null)
                    hash = hash * 59 + this.OrganizationId.GetHashCode();
                    if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                    if (this.Unlocode != null)
                    hash = hash * 59 + this.Unlocode.GetHashCode();
                    if (this.Version != null)
                    hash = hash * 59 + this.Version.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Instance left, Instance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Instance left, Instance right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}