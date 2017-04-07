using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface IUploadedMessageService : IInternalServiceBase<UploadedMessage>
    {
        void InsertPCM(UploadedMessage message);
        void InsertRTZ(UploadedMessage message);
        void InsertTXT(UploadedMessage message);
        List<UploadedMessage> GetMessagesByLimitNumber(int numberOfMessages, List<MessageType> mTypes);
        List<UploadedMessage> GetAllUnFetchedMessages(List<MessageType> mTypes);
        int GetNumberOfRemainingMessages(List<MessageType> mTypes);
        void InsertArea(UploadedMessage result);
        void SendAck(IList<UploadedMessage> messages);
    }
}
