using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess.Entities
{
    public enum EventDataType
    {
        RTZ=1,
        TXT=2,
        PCM=3,
        S124=4,
        ErrorMessage=5,
        Other=6,
        Unknown = 7,
        None = 8
    }
}
