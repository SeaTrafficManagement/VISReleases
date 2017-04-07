﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.DataAccess
{
    public class MigrateDbToLatestInitializerConnString<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
            where TContext : DbContext
            where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration config;

        /// <summary>
        ///     Initializes a new instance of the MigrateDatabaseToLatestVersion class.
        /// </summary>
        public MigrateDbToLatestInitializerConnString()
        {
            config = new TMigrationsConfiguration();
        }

        /// <summary>
        ///     Initializes a new instance of the MigrateDatabaseToLatestVersion class that will
        ///     use a specific connection string from the configuration file to connect to
        ///     the database to perform the migration.
        /// </summary>
        /// <param name="connectionString"> connection string to use for migration. </param>
        public MigrateDbToLatestInitializerConnString(string connectionString)
        {
            var v = new TMigrationsConfiguration();
            config = new TMigrationsConfiguration
            {
                TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient"),
            };
        }

        public void InitializeDatabase(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("Context passed to InitializeDatabase can not be null");
            }

            var migrator = new DbMigrator(config);
            migrator.Update();
        }
    }
}