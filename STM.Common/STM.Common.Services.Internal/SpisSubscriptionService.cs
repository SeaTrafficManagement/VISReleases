using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using STM.Common.Services.Internal;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class SpisSubscriptionService : InternalServiceBase<SpisSubscription, StmDbContext>, ISpisSubscriptionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public SpisSubscriptionService(StmDbContext dbContext) : base(dbContext)
        {

        }
    }
}
