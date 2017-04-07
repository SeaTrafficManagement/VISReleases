using Microsoft.Practices.Unity;
using STM.Common.DataAccess;
using System.Data.Entity;
using System.Web.Http;
using Unity.WebApi;
using STM.Common.Services.Internal;
using STM.SSC.Internal;
using STM.Common;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Data.SqlClient;
using System.Net.Http;
using System;
using STM.Common.Services.Internal.Interfaces;

namespace STM.VIS.Services.Private
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
                .RegisterType<ISccPrivateService, SccPrivateService>()
                .RegisterType<IMessageTypeService, MessageTypeService>()
                .RegisterType<IUploadedMessageService, UploadedMessageService>()
                .RegisterType<INotificationService, NotificationService>()
                .RegisterType<ILogEventService, LogEventService>()
                .RegisterType<IVisSubscriptionService, VisSubscriptionService>()
                .RegisterType<IConnectionInformationService, ConnectionInformationService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}