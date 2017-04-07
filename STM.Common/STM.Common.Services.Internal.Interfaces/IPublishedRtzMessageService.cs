using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface IPublishedRtzMessageService : IInternalServiceBase<PublishedRtzMessage>
    {
        void SendMessageToSubsribers(string message, string dataId);
        void SendMessage(string message, string dataId, string endpoint, Identity identity);
        void SaveAndUpdate(PublishedRtzMessage entity, bool isNew, bool ignoreStatusInMessage = false);
    }
}