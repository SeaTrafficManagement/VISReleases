
using STM.SSC.Internal.Models;

namespace STM.SSC.Internal
{
    public interface ISccPrivateService
    {
        CallServiceResponseObj CallService(CallServiceRequestObj request);
        FindIdentitiesResponseObj FindIdentities();
        FindServicesResponseObj FindServices(FindServicesRequestObj request);
        IdentityRegistryGeneralResponseObj IdentityRegistryGeneralRequest(IdentityRegistryGeneralRequestObj request);
        ServiceRegistryGeneralResponseObj ServiceRegistryGeneralRequest(ServiceRegistryGeneralRequestObj request);
    }
}