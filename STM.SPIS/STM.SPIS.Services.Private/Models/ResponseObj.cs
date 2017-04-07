/* 
 * STM Ship Port Information Service STM Onboard API
 *
 * Ship Port Information Service API facing STM Onboard systems exposing interfaces to ships
 *
 * OpenAPI spec version: 1.0.0
 * Contact: per.lofbom@sjofartsverket.se
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace STM.SPIS.Services.Private.Models
{
    /// <summary>
    /// ResponseObj
    /// </summary>
    [DataContract]
    public partial class ResponseObj :  IEquatable<ResponseObj>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseObj" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ResponseObj() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseObj" /> class.
        /// </summary>
        /// <param name="DataId">DataId typically an ID </param>
        public ResponseObj(string DataId = default(string))
        {
            // to ensure "DataId" is required (not null)
            if (DataId == null)
            {
                throw new InvalidDataException("DataId is a required property for ResponseObj and cannot be null");
            }
            else
            {
                this.DataId = DataId;
            }
        }
        
        /// <summary>
        /// Data Id typically a UPCID
        /// </summary>
        /// <value>Data Id typically a UPCID</value>
        [DataMember(Name="dataId", EmitDefaultValue=false)]
        public string DataId { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ResponseObj {\n");
            sb.Append("  DataId: ").Append(DataId).Append("\n");
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
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as ResponseObj);
        }

        /// <summary>
        /// Returns true if ResponseObj instances are equal
        /// </summary>
        /// <param name="other">Instance of ResponseObj to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ResponseObj other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.DataId == other.DataId ||
                    this.DataId != null &&
                    this.DataId.Equals(other.DataId)
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
                if (this.DataId != null)
                    hash = hash * 59 + this.DataId.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            // DataId (string) pattern
            Regex regexDataId = new Regex(@"urn:mrn:", RegexOptions.CultureInvariant);
            if (false == regexDataId.Match(this.DataId).Success)
            {
                yield return new ValidationResult("Invalid value for DataId, must match a pattern of /urn:mrn:/.", new [] { "DataId" });
            }

            yield break;
        }
    }

}