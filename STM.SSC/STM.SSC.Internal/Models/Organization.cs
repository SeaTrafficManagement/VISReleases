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

namespace STM.SSC.Internal.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class Organization :  IEquatable<Organization>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Organization" /> class.
        /// </summary>
        /// <param name="Country">Country (required).</param>
        /// <param name="Email">Email (required).</param>
        /// <param name="Mrn">The Maritime Resource Name (required).</param>
        /// <param name="Name">The name of the organization (required).</param>
        public Organization(string Country = null, string Email = null, string Mrn = null, string Name = null)
        {
            // to ensure "Country" is required (not null)
            if (Country == null)
            {
                throw new InvalidDataException("Country is a required property for Organization and cannot be null");
            }
            else
            {
                this.Country = Country;
            }
            // to ensure "Email" is required (not null)
            if (Email == null)
            {
                throw new InvalidDataException("Email is a required property for Organization and cannot be null");
            }
            else
            {
                this.Email = Email;
            }
            // to ensure "Mrn" is required (not null)
            if (Mrn == null)
            {
                throw new InvalidDataException("Mrn is a required property for Organization and cannot be null");
            }
            else
            {
                this.Mrn = Mrn;
            }
            // to ensure "Name" is required (not null)
            if (Name == null)
            {
                throw new InvalidDataException("Name is a required property for Organization and cannot be null");
            }
            else
            {
                this.Name = Name;
            }
            
        }

        /// <summary>
        /// Gets or Sets Country
        /// </summary>
        [DataMember(Name="country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        [DataMember(Name="email")]
        public string Email { get; set; }

        /// <summary>
        /// The Maritime Resource Name
        /// </summary>
        /// <value>The Maritime Resource Name</value>
        [DataMember(Name="mrn")]
        public string Mrn { get; set; }

        /// <summary>
        /// The name of the organization
        /// </summary>
        /// <value>The name of the organization</value>
        [DataMember(Name="name")]
        public string Name { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Organization {\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  Mrn: ").Append(Mrn).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
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
            return Equals((Organization)obj);
        }

        /// <summary>
        /// Returns true if Organization instances are equal
        /// </summary>
        /// <param name="other">Instance of Organization to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Organization other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Country == other.Country ||
                    this.Country != null &&
                    this.Country.Equals(other.Country)
                ) && 
                (
                    this.Email == other.Email ||
                    this.Email != null &&
                    this.Email.Equals(other.Email)
                ) && 
                (
                    this.Mrn == other.Mrn ||
                    this.Mrn != null &&
                    this.Mrn.Equals(other.Mrn)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
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
                    if (this.Country != null)
                    hash = hash * 59 + this.Country.GetHashCode();
                    if (this.Email != null)
                    hash = hash * 59 + this.Email.GetHashCode();
                    if (this.Mrn != null)
                    hash = hash * 59 + this.Mrn.GetHashCode();
                    if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Organization left, Organization right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Organization left, Organization right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}