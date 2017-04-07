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
    public partial class CallServiceResponseObj :  IEquatable<CallServiceResponseObj>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallServiceResponseObj" /> class.
        /// </summary>
        /// <param name="Body">Body.</param>
        /// <param name="StatusCode">StatusCode.</param>
        public CallServiceResponseObj(string Body = null, long? StatusCode = null)
        {
            this.Body = Body;
            this.StatusCode = StatusCode;
            
        }

        /// <summary>
        /// Gets or Sets Body
        /// </summary>
        [DataMember(Name="body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or Sets StatusCode
        /// </summary>
        [DataMember(Name="statusCode")]
        public long? StatusCode { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CallServiceResponseObj {\n");
            sb.Append("  Body: ").Append(Body).Append("\n");
            sb.Append("  StatusCode: ").Append(StatusCode).Append("\n");
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
            return Equals((CallServiceResponseObj)obj);
        }

        /// <summary>
        /// Returns true if CallServiceResponseObj instances are equal
        /// </summary>
        /// <param name="other">Instance of CallServiceResponseObj to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CallServiceResponseObj other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Body == other.Body ||
                    this.Body != null &&
                    this.Body.Equals(other.Body)
                ) && 
                (
                    this.StatusCode == other.StatusCode ||
                    this.StatusCode != null &&
                    this.StatusCode.Equals(other.StatusCode)
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
                    if (this.Body != null)
                    hash = hash * 59 + this.Body.GetHashCode();
                    if (this.StatusCode != null)
                    hash = hash * 59 + this.StatusCode.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CallServiceResponseObj left, CallServiceResponseObj right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CallServiceResponseObj left, CallServiceResponseObj right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
