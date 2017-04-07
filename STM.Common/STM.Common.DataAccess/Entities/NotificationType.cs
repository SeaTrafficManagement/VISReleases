using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Entities
{
    public enum NotificationType
    {
        MESSAGE_WAITING = 1,
        UNAUTHORIZED_REQUEST = 2,
        ACKNOWLEDGEMENT_RECEIVED = 3,
        ERROR_MESSAGE = 4
    }
}
