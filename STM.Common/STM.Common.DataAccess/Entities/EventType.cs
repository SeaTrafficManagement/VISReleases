using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Entities
{
    public enum EventType
    {
        Successful=1,
        Error_schema=2,
        Error_parameters=3,
        Error_communication=4,
        Error_internal=5,
        Error_authorization = 6,
        Info=7
    }
}
