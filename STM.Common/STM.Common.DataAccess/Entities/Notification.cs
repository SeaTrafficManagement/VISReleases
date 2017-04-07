using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace STM.Common.DataAccess.Entities
{
    public class Notification
    {
        /// <summary>
        /// Unique internal ID
        /// </summary>
        public long ID { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string FromOrgId { get; set; }

        [Required]
        public string FromOrgName { get; set; }

        public string FromServiceId { get; set; }

        [Required]
        public DateTime NotificationCreatedAt { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public DateTime ReceivedAt { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public bool FetchedByShip { get; set; }

        public DateTime? FetchTime { get; set; }

        public NotificationSource NotificationSource { get; set; }

    }//end Identity
}