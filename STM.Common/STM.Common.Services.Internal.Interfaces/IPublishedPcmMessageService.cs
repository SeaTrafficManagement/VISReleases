using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface IPublishedPcmMessageService : IInternalServiceBase<PublishedPcmMessage>
    {
        void SendMessageToSubsribers(string message, string dataId);
        string SendMessage(string message, string dataId, string mbEndpoint, string amssEndpoint, Identity identity);
    }
}
