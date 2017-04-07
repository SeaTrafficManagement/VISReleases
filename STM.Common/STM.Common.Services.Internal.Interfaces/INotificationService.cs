using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.Services.Internal.Interfaces
{
    public interface INotificationService : IInternalServiceBase<DataAccess.Entities.Notification>
    {
        bool Notify(Notification notification);
    }
}