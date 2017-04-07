using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.SPIS.Services.Private.Models
{
    /// <summary>
    /// Contains published messages from STM Module (e.g. routes in RTZ)
    /// </summary>
    public class PublishedMessageContract
    {
        /// <summary>
        /// The actual message in raw format
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Identity of the message (extracted from the message). In case of voyage plan
        /// it's the UVID.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// Time of update of the message (extracted from the message)
        /// </summary>
        public DateTime? MessageLastUpdateTime { get; set; }

        /// <summary>
        /// Status on the message (extracted or derived from the message). In case of
        /// voyage plan it's the routeStatus.
        /// </summary>
        public virtual int MessageStatus { get; set; }

        /// <summary>
        /// Type of message (enumeration)
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? MessageValidFrom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? MessageValidTo { get; set; }

        /// <summary>
        /// Publish time to SPIS (set by SPIS when received)
        /// </summary>
        public DateTime PublishTime { get; set; }

    }
}