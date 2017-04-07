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
    public partial class CallServiceRequestObj :  IEquatable<CallServiceRequestObj>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallServiceRequestObj" /> class.
        /// </summary>
        /// <param name="Body">Body.</param>
        /// <param name="EndpointMethod">EndpointMethod.</param>
        /// <param name="Headers">Headers.</param>
        /// <param name="RequestType">RequestType.</param>
        public CallServiceRequestObj(string Body = null, string EndpointMethod = null, List<Header> Headers = null, string RequestType = null, string ContentType = "application/json; charset=UTF8;charset=UTF8")
        {
            this.Body = Body;
            this.EndpointMethod = EndpointMethod;
            this.Headers = Headers;
            this.RequestType = RequestType;
        }

        /// <summary>
        /// Gets or Sets Body
        /// </summary>
        [DataMember(Name="body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or Sets EndpointMethod
        /// </summary>
        [DataMember(Name="endpointMethod")]
        public string EndpointMethod { get; set; }

        /// <summary>
        /// Gets or Sets Headers
        /// </summary>
        [DataMember(Name="headers")]
        public List<Header> Headers { get; set; }

        /// <summary>
        /// Gets or Sets RequestType
        /// </summary>
        [DataMember(Name="requestType")]
        public string RequestType { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CallServiceRequestObj {\n");
            sb.Append("  Body: ").Append(Body).Append("\n");
            sb.Append("  EndpointMethod: ").Append(EndpointMethod).Append("\n");
            sb.Append("  Headers: ").Append(Headers).Append("\n");
            sb.Append("  RequestType: ").Append(RequestType).Append("\n");
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
            return Equals((CallServiceRequestObj)obj);
        }

        /// <summary>
        /// Returns true if CallServiceRequestObj instances are equal
        /// </summary>
        /// <param name="other">Instance of CallServiceRequestObj to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CallServiceRequestObj other)
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
                    this.EndpointMethod == other.EndpointMethod ||
                    this.EndpointMethod != null &&
                    this.EndpointMethod.Equals(other.EndpointMethod)
                ) && 
                (
                    this.Headers == other.Headers ||
                    this.Headers != null &&
                    this.Headers.SequenceEqual(other.Headers)
                ) && 
                (
                    this.RequestType == other.RequestType ||
                    this.RequestType != null &&
                    this.RequestType.Equals(other.RequestType)
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
                    if (this.EndpointMethod != null)
                    hash = hash * 59 + this.EndpointMethod.GetHashCode();
                    if (this.Headers != null)
                    hash = hash * 59 + this.Headers.GetHashCode();
                    if (this.RequestType != null)
                    hash = hash * 59 + this.RequestType.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CallServiceRequestObj left, CallServiceRequestObj right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CallServiceRequestObj left, CallServiceRequestObj right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
