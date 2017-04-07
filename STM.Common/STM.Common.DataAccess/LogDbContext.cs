using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace STM.Common.DataAccess
{
    public class LogDbContext : DbContext
    {
        public DbSet<LogEvent> LogEvent { get; set; }

        public LogDbContext()
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

            Database.SetInitializer(new MigrateDbToLatestInitializerConnString<LogDbContext,
                Migrations.LogConfiguration>(Database.Connection.ConnectionString));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SaveChanges();

            base.Dispose(disposing);
        }
    }
}