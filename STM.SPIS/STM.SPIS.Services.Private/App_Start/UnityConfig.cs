using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using STM.SSC.Internal;
using STM.Common.DataAccess;
using STM.Common.Services.Internal.Interfaces;
using STM.Common.Services.Internal;

namespace STM.SPIS.Services.Private
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
                .RegisterType<ISccPrivateService, SccPrivateService>()
                .RegisterType<IIdentityService, IdentityService>()
                .RegisterType<IACLObjectService, ACLObjectService>()
                .RegisterType<ISpisSubscriptionService, SpisSubscriptionService>()
                .RegisterType<IMessageTypeService, MessageTypeService>()
                .RegisterType<IPublishedPcmMessageService, PublishedPcmMessageService>()
                .RegisterType<ILogEventService, LogEventService>();


            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}