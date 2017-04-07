using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Entities
{
    public enum EventCategory
    {
        Success = 1,
        Request = 2,
        Response = 3,
        AuthorizationError = 4,
        AuthenticationError = 5,
        RequestError = 6,
        UnknownError = 7,
        CommunicationError = 8,
        DatabaseError = 9,
        InternalError = 10,
        General = 11

    }
}