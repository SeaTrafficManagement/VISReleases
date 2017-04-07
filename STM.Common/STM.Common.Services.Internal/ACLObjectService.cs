using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class ACLObjectService : InternalServiceBase<ACLObject, StmDbContext>, IACLObjectService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public ACLObjectService(StmDbContext dbContext) : base(dbContext)
        {
        }

    }
}
