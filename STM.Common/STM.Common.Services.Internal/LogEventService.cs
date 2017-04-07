using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using System.Data.Entity;
using STM.Common.Services.Internal.Interfaces;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEventService : InternalServiceBase<LogEvent, LogDbContext>, ILogEventService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public LogEventService(LogDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventNumber"></param>
        /// <param name="eventType"></param>
        /// <param name="parameters"></param>
        /// <param name="eventData"></param>
        public void LogError(EventNumber eventNumber, EventType eventType,
            List<KeyValuePair<string, string>> parameters, string eventData)
        {

            var logEvent = new LogEvent
            {
                EventTime = SetDate(),
                ServiceInstanceId = Common.InstanceContext.ServiceId,
                EventNumber = eventNumber,
                EventType = eventType,
                ExternalOrgId = Common.InstanceContext.CallerOrgId,
                ExternalEntityId = Common.InstanceContext.CallerServiceId,
                EventParameters = ConvertToString(parameters),
                EventDataType = EventDataType.None,
                EventData = eventData
            };

            base.Insert(logEvent);

        }

        private string ConvertToString(List<KeyValuePair<string, string>> parameters)
        {
            string result = string.Empty;

            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var parameter in parameters)
                {
                    result += parameter.Key.ToString() + " = " + parameter.Value + ", ";
                }
                result = result.TrimEnd();
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        private DateTime SetDate()
        {
            return DateTime.UtcNow.ToUniversalTime();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventNumber"></param>
        /// <param name="eventDataType"></param>
        /// <param name="parameters"></param>
        /// <param name="eventData"></param>
        public void LogSuccess(EventNumber eventNumber, EventDataType eventDataType,
            List<KeyValuePair<string, string>> parameters, string eventData)
        {

            var logEvent = new LogEvent
            {
                EventTime = SetDate(),
                ServiceInstanceId = Common.InstanceContext.ServiceId,
                EventNumber = eventNumber,
                EventType = EventType.Successful,
                ExternalOrgId = Common.InstanceContext.CallerOrgId,
                ExternalEntityId = Common.InstanceContext.CallerServiceId,
                EventParameters = ConvertToString(parameters),
                EventDataType = eventDataType,
                EventData = eventData
            };

            base.Insert(logEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventNumber"></param>
        /// <param name="eventDataType"></param>
        /// <param name="parameters"></param>
        /// <param name="eventData"></param>
        public void LogInfo(EventNumber eventNumber, EventDataType eventDataType,
            List<KeyValuePair<string, string>> parameters, string eventData)
        {
            var logEvent = new LogEvent
            {
                EventTime = SetDate(),
                ServiceInstanceId = Common.InstanceContext.ServiceId,
                EventNumber = eventNumber,
                EventType = EventType.Info,
                ExternalOrgId = Common.InstanceContext.CallerOrgId,
                ExternalEntityId = Common.InstanceContext.CallerServiceId,
                EventParameters = ConvertToString(parameters),
                EventDataType = eventDataType,
                EventData = eventData
            };

            base.Insert(logEvent);
        }
    }
}
