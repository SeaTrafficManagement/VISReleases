namespace STM.Common.DataAccess.Migrations
{
    using EfEnumToLookup.LookupGenerator;
    using STM.Common.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    internal sealed class LogConfiguration : DbMigrationsConfiguration<LogDbContext>
    {
        public LogConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(LogDbContext context)
        {
            var enumToAdd = new EnumToLookup();
            var migrationSql = enumToAdd.GenerateMigrationSql(context);

            enumToAdd.Apply(context);
            base.Seed(context);
        }
    }
}