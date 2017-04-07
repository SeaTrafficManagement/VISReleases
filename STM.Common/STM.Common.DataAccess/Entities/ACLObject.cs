///////////////////////////////////////////////////////////
//  ACLObject.cs
//  Implementation of the Class ACLObject
//  Generated by Enterprise Architect
//  Created on:      18-nov-2016 13:07:31
//  Original author: M02AMIOL
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace STM.Common.DataAccess.Entities
{
    	/// <summary>
	/// ACL objects for each information/data object.
	/// </summary>
    public class ACLObject
    {

        /// <summary>
        /// Unique internal ID
        /// </summary>
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// Time of last update
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// Reference to data related to the ACL object
        /// </summary>
        [Required]
        public string MessageID { get; set; }

        /// <summary>
        /// Identities that have access to the information object
        /// </summary>
        [Required]
        public Identity Subscriber { get; set; }

    }//end ACLObject
}