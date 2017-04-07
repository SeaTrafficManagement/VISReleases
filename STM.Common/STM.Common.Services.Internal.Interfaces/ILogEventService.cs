using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface ILogEventService : IInternalServiceBase<LogEvent>
    {

        void LogError(EventNumber eventNumber, EventType eventType,
            List<KeyValuePair<string, string>> parameters, string eventData);
        void LogSuccess(EventNumber eventNumber, EventDataType eventDataType,
            List<KeyValuePair<string, string>> parameters, string eventData);

        void LogInfo(EventNumber eventNumber, EventDataType eventDataType,
            List<KeyValuePair<string, string>> parameters, string eventData);
        //EventCategory StatusCodeToEventCategory(HttpStatusCode statusCode);
        //void Init(EventDataType messageType);
    }
}
