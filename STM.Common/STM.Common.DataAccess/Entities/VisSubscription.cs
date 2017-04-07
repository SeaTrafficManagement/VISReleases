///////////////////////////////////////////////////////////
//  Subscription.cs
//  Implementation of the Class Subscription
//  Generated by Enterprise Architect
//  Created on:      18-nov-2016 13:07:32
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
	/// Contains active subscriptions
	/// </summary>
    public class VisSubscription
    {


        /// <summary>
        /// The URL to subscribers uploadVoyagePlan endpoint where published voyage plans
        /// are forwarded.
        /// </summary>
        [Required]
        public string CallbackEndpoint { get; set; }

        /// <summary>
        /// Unique internal id of the subscription object
        /// </summary>
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// Message ID to subscribe on. In the case of VIS it's a UVID. Empty means
        /// subscription on all published voyage plans from the ship independent on UVID.
        /// </summary>
        [Required]
        public string MessageID { get; set; }

        /// <summary>
        /// Type of message subscribed on. In case of VIS it's currently RTZ of defined
        /// versions.
        /// </summary>
        [Required]
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Identity of subscriber. Used to check against ACL.
        /// </summary>
        [Required]
        public Identity SubscriberIdentity { get; set; }

        /// <summary>
        /// Time for last sent message to subscriber
        /// </summary>
        public DateTime? TimeOfLastSentMessage { get; set; }

        /// <summary>
        /// Time for start of subscription
        /// </summary>
        public DateTime? TimeOfSubscriptionRequest { get; set; }

        /// <summary>
        /// Indicates if the subscription has been authorized. If false no information will be sent out to this subscriber
        /// </summary>
        public bool IsAuthorized { get; set; }

    }//end Subscription
}