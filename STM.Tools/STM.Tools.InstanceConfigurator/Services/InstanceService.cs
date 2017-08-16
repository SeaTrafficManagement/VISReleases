using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using System.Windows;

namespace STM.Tools.InstanceConfigurator.Services
{
    public class InstanceService
    {
        public static List<string> GetAllDbInstances()
        {
            var result = new List<string>();

            var conString = ConfigurationManager.ConnectionStrings["StmConnectionString"].ConnectionString;
            using (var con = new SqlConnection(conString.Replace("Initial Catalog={database};", "")))
            {
                con.Open();
                DataTable databases = con.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    string databaseName = database.Field<string>("database_name");
                    if (databaseName.ToLower() == "master"
                        || databaseName.ToLower() == "msdb"
                        || databaseName.ToLower() == "tempdb"
                        || databaseName.ToLower() == "model")
                        continue;


                    string conStringDb = conString.Replace("{database}", databaseName);
                    using (var conDB = new SqlConnection(conStringDb))
                    {
                        conDB.Open();

                        using (var command = new SqlCommand())
                        {
                            command.Connection = conDB;
                            var sql = string.Format(
                                    "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{0}'",
                                     "VisInstanceSettings");
                            command.CommandText = sql;
                            var count = Convert.ToInt32(command.ExecuteScalar());
                            if (count > 0)
                                result.Add(databaseName);
                        }
                    }
                }
            }

            result.Sort();

            return result;
        }

        public static bool AddInstance(string dbName)
        {
            try
            {
                var conString = ConfigurationManager.ConnectionStrings["StmConnectionString"].ConnectionString;
                using (var con = new SqlConnection(conString.Replace("Initial Catalog={database};", "")))
                {
                    con.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandTimeout = 180;
                        var sql = string.Format(
                                "CREATE DATABASE " + dbName);
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }

                var stmDbContext = new StmDbContext();
                stmDbContext.init(dbName);
                stmDbContext.ReInitializeDatabase();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when creating database" + Environment.NewLine + ex.Message, "Add instance", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool DeleteInstance(string dbName)
        {
            try
            {
                var conString = ConfigurationManager.ConnectionStrings["StmConnectionString"].ConnectionString;
                using (var con = new SqlConnection(conString.Replace("{database}", "master")))
                {
                    con.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandTimeout = 180;
                        var sql1 = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbName);
                        var sql2 = "DROP DATABASE " + dbName;
                        command.CommandText = sql1;
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch {}
                        command.CommandText = sql2;
                        command.ExecuteNonQuery();

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when deleting database" + Environment.NewLine + ex.Message, "Delete instance", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static VisInstanceSettings GetVisInstanceSettings(string dbName)
        {
            var result = new VisInstanceSettings();

            try
            {
                var stmDbContext = new StmDbContext();
                stmDbContext.init(dbName);

                result = stmDbContext.VisInstanceSettings.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public static SpisInstanceSettings GetSpisInstanceSettings(string dbName)
        {
            var result = new SpisInstanceSettings();

            try
            {
                var stmDbContext = new StmDbContext();
                stmDbContext.init(dbName);

                result = stmDbContext.SpisInstanceSettings.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public static void SaveVisInstanceSettings(string dbName, VisInstanceSettings settings)
        {
            try
            {
                using (var stmDbContext = new StmDbContext())
                {
                    stmDbContext.init(dbName);

                    var current = stmDbContext.VisInstanceSettings.FirstOrDefault();
                    if (current == null)
                    {
                        stmDbContext.VisInstanceSettings.Add(settings);
                    }
                    else
                    {
                        current.ClientCertificate = settings.ClientCertificate;
                        current.Password = settings.Password;
                        current.ServiceId = settings.ServiceId;
                        current.ServiceName = settings.ServiceName;
                        current.StmModuleUrl = settings.StmModuleUrl;
                        current.ApiKey = settings.ApiKey;
                        current.ApplicationId = settings.ApplicationId;
                        current.UseHMACAuthentication = settings.UseHMACAuthentication;
                        current.NotImplementetOperations = settings.NotImplementetOperations;
                    }

                    stmDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void SaveSpisInstanceSettings(string dbName, SpisInstanceSettings settings)
        {
            try
            {
                using (var stmDbContext = new StmDbContext())
                {
                    stmDbContext.init(dbName);

                    var current = stmDbContext.SpisInstanceSettings.FirstOrDefault();
                    if (current == null)
                    {
                        stmDbContext.SpisInstanceSettings.Add(settings);
                    }
                    else
                    {
                        current.ClientCertificate = settings.ClientCertificate;
                        current.Password = settings.Password;
                        current.ServiceId = settings.ServiceId;
                        current.ServiceName = settings.ServiceName;
                        current.StmModuleUrl = settings.StmModuleUrl;
                        current.IMO = settings.IMO;
                        current.MMSI = settings.MMSI;
                        current.ApiKey = settings.ApiKey;
                        current.ApplicationId = settings.ApplicationId;
                        current.UseHMACAuthentication = settings.UseHMACAuthentication;
                    }

                    stmDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}