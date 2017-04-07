namespace STM.Common.DataAccess.Migrations
{
    using Common.DataAccess.Entities;
    using EfEnumToLookup.LookupGenerator;
    using STM.Common.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    internal sealed class StmConfiguration : DbMigrationsConfiguration<StmDbContext>
    {
        public StmConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(STM.Common.DataAccess.StmDbContext context)
        {
            
            //throw new Exception();
            var mt1 = new MessageType
            {
                Name = "RTZ",
                SchemaXSD = "rtz.xsd"
            };

            var mt2 = new MessageType
            {
                Name = "TXT",
                SchemaXSD = "textMessageSchema.xsd"
            };

            var mt3 = new MessageType
            {
                Name = "S124",
                SchemaXSD = "s124.xsd"
            };

            var mt4 = new MessageType
            {
                Name = "PCM",
                SchemaXSD = "port-call-message_v0.16.xsd"
            };

            if (!context.MessageType.Any())
            {
                context.MessageType.Add(mt1);
                context.MessageType.Add(mt2);
                context.MessageType.Add(mt3);
                context.MessageType.Add(mt4);

                context.SaveChanges();
            }

            var enumToAdd = new EnumToLookup();
            var migrationSql = enumToAdd.GenerateMigrationSql(context);

            enumToAdd.Apply(context);

            base.Seed(context);

        }
    }
}
