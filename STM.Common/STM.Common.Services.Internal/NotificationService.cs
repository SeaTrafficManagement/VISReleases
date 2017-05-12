using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STM.Common.DataAccess;
using STM.SSC.Internal;
using STM.Common.DataAccess.Entities;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationService : InternalServiceBase<DataAccess.Entities.Notification, StmDbContext>, Interfaces.INotificationService
    {
        private LogEventService _logEventService;
        private StmModuleService _stmModuleService;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logContext"></param>
        public NotificationService(StmDbContext dbContext, LogDbContext logContext) : base(dbContext)
        {
            _logEventService = new LogEventService(logContext);
            _stmModuleService = new StmModuleService(dbContext, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public bool Notify(Interfaces.Notification notification)
        {
            try
            {
                var sentToClient = _stmModuleService.Notify(notification);

                if (sentToClient)
                {
                    log.Info("Success send notification to STM Module");
                }
                else
                {
                    log.Info("Notification saved to VIS database. Unable to send it to STM Module. STM Module needs to pull the notification from VIS.");
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
