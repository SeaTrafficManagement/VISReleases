/*
 * STM Voyage Information Service SeaSWIM API
 *
 * Voyage Information Service API facing SeaSWIM through SSC exposing interfaces to SeaSWIM stakeholders
 *
 * OpenAPI spec version: 1.0.0
 * Contact: per.lofbom@sjofartsverket.se
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

namespace STM.VIS.Services.Public.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class TextMessageObject :  IEquatable<TextMessageObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageObject" /> class.
        /// </summary>
        /// <param name="TextMessage">TextMessage (required).</param>
        public TextMessageObject(string TextMessage = null)
        {
            // to ensure "TextMessage" is required (not null)
            if (TextMessage == null)
            {
                throw new InvalidDataException("TextMessage is a required property for TextMessageObject and cannot be null");
            }
            else
            {
                this.TextMessage = TextMessage;
            }
            
        }

        /// <summary>
        /// Gets or Sets TextMessage
        /// </summary>
        [DataMember(Name="textMessage")]
        public string TextMessage { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TextMessageObject {\n");
            sb.Append("  TextMessage: ").Append(TextMessage).Append("\n");
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
            return Equals((TextMessageObject)obj);
        }

        /// <summary>
        /// Returns true if TextMessageObject instances are equal
        /// </summary>
        /// <param name="other">Instance of TextMessageObject to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TextMessageObject other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.TextMessage == other.TextMessage ||
                    this.TextMessage != null &&
                    this.TextMessage.Equals(other.TextMessage)
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
                    if (this.TextMessage != null)
                    hash = hash * 59 + this.TextMessage.GetHashCode();
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
        public static bool operator ==(TextMessageObject left, TextMessageObject right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TextMessageObject left, TextMessageObject right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
