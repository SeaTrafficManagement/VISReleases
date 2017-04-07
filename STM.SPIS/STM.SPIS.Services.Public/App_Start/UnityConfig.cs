using Microsoft.Practices.Unity;
using STM.Common.DataAccess;
using STM.Common.Services.Internal;
using STM.Common.Services.Internal.Interfaces;
using STM.SSC.Internal;
using System.Web.Http;
using Unity.WebApi;

namespace STM.SPIS.Services.Public
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container
                .RegisterType<StmDbContext, StmDbContext>(new HierarchicalLifetimeManager())
                .RegisterType<LogDbContext, LogDbContext>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageTypeService, MessageTypeService>()
                .RegisterType<IUploadedMessageService, UploadedMessageService>()
                .RegisterType<INotificationService, NotificationService>()
                .RegisterType<IUploadedMessageService, UploadedMessageService>()
                .RegisterType<IIdentityService, IdentityService>()
                .RegisterType<IACLObjectService, ACLObjectService>()
                .RegisterType<ISpisSubscriptionService, SpisSubscriptionService>()
                .RegisterType<IMessageTypeService, MessageTypeService>()
                .RegisterType<IPublishedPcmMessageService, PublishedPcmMessageService>()
                .RegisterType<ISccPrivateService, SccPrivateService>()
                .RegisterType<ILogEventService, LogEventService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}