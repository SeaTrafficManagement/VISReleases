using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using STM.Common.Services.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionInformationService : InternalServiceBase<ConnectionInformation, StmDbContext>, IConnectionInformationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public ConnectionInformationService(StmDbContext dbContext) : base(dbContext)
        {
        }
    }
}
