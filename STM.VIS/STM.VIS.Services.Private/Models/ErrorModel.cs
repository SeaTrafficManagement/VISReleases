using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace STM.VIS.Services.Private.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class ErrorModel :  IEquatable<ErrorModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorModel" /> class.
        /// </summary>
        /// <param name="ErrorModelId">ErrorModelId (required).</param>
        /// <param name="Code">Error code (required).</param>
        /// <param name="Message">Error message contents (required).</param>
        public ErrorModel(long? ErrorModelId = null, int? Code = null, string Message = null)
        {
            // to ensure "ErrorModelId" is required (not null)
            if (ErrorModelId == null)
            {
                throw new InvalidDataException("ErrorModelId is a required property for ErrorModel and cannot be null");
            }
            else
            {
                this.ErrorModelId = ErrorModelId;
            }
            // to ensure "Code" is required (not null)
            if (Code == null)
            {
                throw new InvalidDataException("Code is a required property for ErrorModel and cannot be null");
            }
            else
            {
                this.Code = Code;
            }
            // to ensure "Message" is required (not null)
            if (Message == null)
            {
                throw new InvalidDataException("Message is a required property for ErrorModel and cannot be null");
            }
            else
            {
                this.Message = Message;
            }
            
        }

        /// <summary>
        /// Gets or Sets ErrorModelId
        /// </summary>
        [DataMember(Name="errorModelId")]
        public long? ErrorModelId { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        /// <value>Error code</value>
        [DataMember(Name="code")]
        public int? Code { get; set; }

        /// <summary>
        /// Error message contents
        /// </summary>
        /// <value>Error message contents</value>
        [DataMember(Name="message")]
        public string Message { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ErrorModel {\n");
            sb.Append("  ErrorModelId: ").Append(ErrorModelId).Append("\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ErrorModel)obj);
        }

        /// <summary>
        /// Returns true if ErrorModel instances are equal
        /// </summary>
        /// <param name="other">Instance of ErrorModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ErrorModel other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.ErrorModelId == other.ErrorModelId ||
                    this.ErrorModelId != null &&
                    this.ErrorModelId.Equals(other.ErrorModelId)
                ) && 
                (
                    this.Code == other.Code ||
                    this.Code != null &&
                    this.Code.Equals(other.Code)
                ) && 
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
                    if (this.ErrorModelId != null)
                    hash = hash * 59 + this.ErrorModelId.GetHashCode();
                    if (this.Code != null)
                    hash = hash * 59 + this.Code.GetHashCode();
                    if (this.Message != null)
                    hash = hash * 59 + this.Message.GetHashCode();
                return hash;
            }
        }

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ErrorModel left, ErrorModel right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ErrorModel left, ErrorModel right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
