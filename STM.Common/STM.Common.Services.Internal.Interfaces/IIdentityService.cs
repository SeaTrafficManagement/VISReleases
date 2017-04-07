using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface IIdentityService : IInternalServiceBase<Identity>
    {
        Identity GetCallerIdentity();
    }
}
