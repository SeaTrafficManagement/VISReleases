using Microsoft.Practices.Unity;
using STM.Common.DataAccess;
using STM.Common.Services.Internal;
using System.Data.Entity;
using System.Web.Http;
using Unity.WebApi;
using STM.Common.Services.Internal.Interfaces;
using STM.SSC.Internal;

namespace STM.VIS.Services.Public
{
    static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container
                .RegisterType<StmDbContext, StmDbContext>(new HierarchicalLifetimeManager())
                .RegisterType<LogDbContext, LogDbContext>(new HierarchicalLifetimeManager())
                .RegisterType<IACLObjectService, ACLObjectService>()
                .RegisterType<IIdentityService, IdentityService>()
                .RegisterType<IPublishedRtzMessageService, PublishedRtzMessageService>()
                .RegisterType<IVisSubscriptionService, VisSubscriptionService>()
                .RegisterType<IMessageTypeService, MessageTypeService>()
                .RegisterType<IStmModuleService, StmModuleService>()
                .RegisterType<IUploadedMessageService, UploadedMessageService>()
                .RegisterType<INotificationService, NotificationService>()
                .RegisterType<ISccPrivateService, SccPrivateService>()
                .RegisterType<ILogEventService, LogEventService>()
                .RegisterType<IConnectionInformationService, ConnectionInformationService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}