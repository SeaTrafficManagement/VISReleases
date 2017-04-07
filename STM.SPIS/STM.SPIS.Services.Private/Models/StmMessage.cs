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
    /// StmMessage
    /// </summary>
    [DataContract]
    public partial class StmMessage :  IEquatable<StmMessage>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StmMessage" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected StmMessage() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StmMessage" /> class.
        /// </summary>
        /// <param name="Message">Message contents (required).</param>
        public StmMessage(string Message = default(string))
        {
            // to ensure "Message" is required (not null)
            if (Message == null)
            {
                throw new InvalidDataException("Message is a required property for StmMessage and cannot be null");
            }
            else
            {
                this.Message = Message;
            }
        }
        
        /// <summary>
        /// Message contents
        /// </summary>
        /// <value>Message contents</value>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StmMessage {\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
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
            return this.Equals(obj as StmMessage);
        }

        /// <summary>
        /// Returns true if StmMessage instances are equal
        /// </summary>
        /// <param name="other">Instance of StmMessage to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StmMessage other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Message == other.Message ||
                    this.Message != null &&
                    this.Message.Equals(other.Message)
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
                if (this.Message != null)
                    hash = hash * 59 + this.Message.GetHashCode();
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
            yield break;
        }
    }

}
