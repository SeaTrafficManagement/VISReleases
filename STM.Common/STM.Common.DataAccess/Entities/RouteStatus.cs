using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Entities
{
    public enum RouteStatus
    {
        Original = 1,
        Planned_for_voyage = 2,
        Optimized = 3,
        Cross_Checked = 4,
        Safety_Checked = 5,
        Approved = 6,
        Used_for_monitoring = 7,
        Inactive = 8,
        Unknown
    }
}
