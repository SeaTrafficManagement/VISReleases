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
    public partial class MessageEnvelope :  IEquatable<MessageEnvelope>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope" /> class.
        /// </summary>
        /// <param name="NumberOfMessages">NumberOfMessages (required).</param>
        /// <param name="RemainingNumberOfMessages">RemainingNumberOfMessages (required).</param>
        /// <param name="Message">Message.</param>
        public MessageEnvelope(int? NumberOfMessages = null, int? RemainingNumberOfMessages = null, List<Message> Messages = null)
        {
            // to ensure "NumberOfMessages" is required (not null)
            if (NumberOfMessages == null)
            {
                //throw new InvalidDataException("NumberOfMessages is a required property for MessageEnvelope and cannot be null");
            }
            else
            {
                this.NumberOfMessages = NumberOfMessages;
            }
            // to ensure "RemainingNumberOfMessages" is required (not null)
            if (RemainingNumberOfMessages == null)
            {
                //throw new InvalidDataException("RemainingNumberOfMessages is a required property for MessageEnvelope and cannot be null");
            }
            else
            {
                this.RemainingNumberOfMessages = RemainingNumberOfMessages;
            }
            this.Messages = Messages;
            
        }

        /// <summary>
        /// Gets or Sets NumberOfMessages
        /// </summary>
        [DataMember(Name="numberOfMessages")]
        public int? NumberOfMessages { get; set; }

        /// <summary>
        /// Gets or Sets RemainingNumberOfMessages
        /// </summary>
        [DataMember(Name="remainingNumberOfMessages")]
        public int? RemainingNumberOfMessages { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name="message")]
        public List<Message> Messages { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MessageEnvelope {\n");
            sb.Append("  NumberOfMessages: ").Append(NumberOfMessages).Append("\n");
            sb.Append("  RemainingNumberOfMessages: ").Append(RemainingNumberOfMessages).Append("\n");
            sb.Append("  Message: ").Append(Messages).Append("\n");
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
            return Equals((MessageEnvelope)obj);
        }

        /// <summary>
        /// Returns true if MessageEnvelope instances are equal
        /// </summary>
        /// <param name="other">Instance of MessageEnvelope to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MessageEnvelope other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.NumberOfMessages == other.NumberOfMessages ||
                    this.NumberOfMessages != null &&
                    this.NumberOfMessages.Equals(other.NumberOfMessages)
                ) && 
                (
                    this.RemainingNumberOfMessages == other.RemainingNumberOfMessages ||
                    this.RemainingNumberOfMessages != null &&
                    this.RemainingNumberOfMessages.Equals(other.RemainingNumberOfMessages)
                ) && 
                (
                    this.Messages == other.Messages ||
                    this.Messages != null &&
                    this.Messages.Equals(other.Messages)
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
                    if (this.NumberOfMessages != null)
                    hash = hash * 59 + this.NumberOfMessages.GetHashCode();
                    if (this.RemainingNumberOfMessages != null)
                    hash = hash * 59 + this.RemainingNumberOfMessages.GetHashCode();
                    if (this.Messages != null)
                    hash = hash * 59 + this.Messages.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(MessageEnvelope left, MessageEnvelope right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MessageEnvelope left, MessageEnvelope right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
