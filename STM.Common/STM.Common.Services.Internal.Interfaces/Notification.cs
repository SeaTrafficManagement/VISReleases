using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
	/// <summary>
	/// To inside/private application, such as STM Module
	/// </summary>
    public class Notification
    {
        /// <summary>
        /// Notification body, optional
        /// </summary>
        [Required]
        public string Body { get; set; }
        /// <summary>
        /// Source organisation of notification and source for retrieving the complete message,
        /// mandatory, according to the STM MRN identifier. Example: urn:mrn:stm:org:
        /// <organization>
        /// </summary>
        [Required]
        public string FromOrgId { get; set; }
        /// <summary>
        /// Friendly name of sender for presentation
        /// </summary>
        public string FromOrgName { get; set; }

        /// <summary>
        /// Source service of notification and source for retrieving the complete message,
        /// mandatory, according to the STM MRN identifier. Example: urn:mrn:stm:org:
        /// </summary>
        [Required]
        public string FromServiceId { get; set; }
        /// <summary>
        /// >0 if a message is waiting in server, otherwise 0, mandatory
        /// </summary>
        [Required]
        public int MessageWaiting { get; set; }
        /// <summary>
        /// Notification created at date and time, mandatory
        /// </summary>
        [Required]
        public DateTime NotificationCreatedAt { get; set; }
        /// <summary>
        /// Type of notification by enumeration
        /// </summary>
        [Required]
        public EnumNotificationType NotificationType { get; set; }
        /// <summary>
        /// Date and time for the reception of the message.
        /// </summary>
        [Required]
        public DateTime ReceivedAt { get; set; }
        /// <summary>
        /// Notification subject, mandatory
        /// </summary>
        [Required]
        public string Subject { get; set; }

        [Required]
        public EnumNotificationSource NotificationSource { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
