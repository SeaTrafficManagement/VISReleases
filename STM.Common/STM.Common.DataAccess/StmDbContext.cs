using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace STM.Common.DataAccess
{
    public class StmDbContext : DbContext
    {

        public DbSet<ACLObject> ACLObject { get; set; }
        public DbSet<ConnectionInformation> ConnectionInformation { get; set; }
        public DbSet<Identity> Identity { get; set; }
        public DbSet<MessageType> MessageType { get; set; }
        public DbSet<PublishedRtzMessage> PublishedRtzMessage { get; set; }
        public DbSet<PublishedPcmMessage> PublishedPcmMessage { get; set; }
        public DbSet<VisSubscription> VisSubscription { get; set; }
        public DbSet<SpisSubscription> SpisSubscription { get; set; }
        public DbSet<UploadedMessage> UploadedMessage { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<VisInstanceSettings> VisInstanceSettings { get; set; }
        public DbSet<SpisInstanceSettings> SpisInstanceSettings { get; set; }

        public StmDbContext()
            : base("StmConnectionString")
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public void init(string instance)
        {
            Database.Connection.ConnectionString = Database.Connection.ConnectionString.Replace("{database}", instance);

            if (!Database.Exists())
            {
                throw new Exception("The dataabase " + instance + " does not exist");
            }

            Database.SetInitializer(new MigrateDbToLatestInitializerConnString<StmDbContext,
                Migrations.StmConfiguration>(Database.Connection.ConnectionString));
        }

        public void ReInitializeDatabase()
        {
            Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}