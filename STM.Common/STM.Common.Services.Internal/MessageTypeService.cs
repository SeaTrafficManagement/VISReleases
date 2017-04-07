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
    public class MessageTypeService : InternalServiceBase<MessageType, StmDbContext>, IMessageTypeService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public MessageTypeService(StmDbContext dbContext) : base(dbContext)
        {

        }
    }
}
