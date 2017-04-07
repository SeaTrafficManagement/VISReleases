using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STM.Common.Services.Internal.Interfaces;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class VisSubscriptionService : InternalServiceBase<VisSubscription, StmDbContext>, IVisSubscriptionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public VisSubscriptionService(StmDbContext dbContext) : base(dbContext)
        {

        }
    }
}
