
using log4net;
using Newtonsoft.Json;
using STM.Common;
using STM.SSC.Internal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace STM.SPIS.PollingQueueWorker
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            try
            {
                log.Debug("============================================================================================");
                log.Debug("Program PollingQueueWorker is starting");
                log.Debug("============================================================================================");

                //1. Loopa genom alla databaser
                var conString = ConfigurationManager.AppSettings["connectionString"];
                log.Debug(string.Format("Trying to connect to database using connection string {0}.", conString));
                using (var con = new SqlConnection(conString))
                {
                    con.Open();
                    log.Debug(string.Format("Successfully connected to {0}.", conString));
                    DataTable databases = con.GetSchema("Databases");
                    foreach (DataRow database in databases.Rows)
                    {
                        string databaseName = database.Field<String>("database_name");
                        log.Debug(string.Format("Database: {0}", databaseName));
                        InstanceContext.Instance = databaseName;

                        if (databaseName == "master")
                            continue;

                        string conStringDb = conString + string.Format(";Initial Catalog={0}", databaseName);
                        log.Debug(string.Format("Trying to connect to database using connection string {0}.", conStringDb));
                        using (var conDB = new SqlConnection(conStringDb))
                        {
                            conDB.Open();
                            log.Debug(string.Format("Successfully connected to {0}.", conStringDb));
                            //2. Kolla om tabellen SpisSubscription finns
                            if (TableExists(conDB, "SpisSubscription"))
                            {
                                log.Debug(string.Format("Loading instance settings for database {0}", databaseName));
                                // Load instance settings
                                DataTable settings = LookUpTable("SpisInstanceSettings", conDB);
                                if (settings.Rows.Count == 0)
                                {
                                    continue;
                                }

                                InstanceContext.Password = settings.Rows[0]["Password"].ToString();
                                InstanceContext.ServiceId = settings.Rows[0]["ServiceId"].ToString();
                                InstanceContext.ServiceName = settings.Rows[0]["ServiceName"].ToString();
                                InstanceContext.StmModuleUrl = settings.Rows[0]["StmModuleUrl"].ToString();

                                var b = (byte[])settings.Rows[0]["ClientCertificate"];
                                var encryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
                                var cert = new X509Certificate2(b, Encryption.DecryptString(InstanceContext.Password, encryptionPassword));
                                InstanceContext.ClientCertificate = cert;

                                Console.WriteLine("Instance: " + InstanceContext.ServiceName);
                                log.Debug(string.Format("Successfully loaded instance settings {0} for database {1}.",
                                    InstanceContext.ServiceName, databaseName));

                                string sqlQuery = "SELECT * FROM SpisSubscription";
                                SqlCommand cmd = new SqlCommand(sqlQuery, conDB);

                                log.Debug(string.Format("Trying to execute sql command {0} for database {1}.", sqlQuery, databaseName));

                                try
                                {
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.HasRows)
                                        {
                                            log.Debug(string.Format("Successfully executed sql command. Entering loop for reading each row."));
                                            //4. För varje post
                                            while (reader.Read())
                                            {
                                                string queueEndpoint = reader["MbEndpoint"].ToString();
                                                string queueId = reader["QueueId"].ToString();
                                                log.Debug(string.Format("Data reader result for queueEndpoint = {0} and queueId = {1}", queueEndpoint, queueId));
                                                //4a. Är kön skapad?
                                                if (string.IsNullOrEmpty(queueId))
                                                {
                                                    //Skapa kö
                                                    log.Debug("Queue did not exist, creating queue.");
                                                    queueId = CreateQueue(queueEndpoint, settings);
                                                }
                                                string endPoint = WebRequestHelper.CombineUrl(queueEndpoint, string.Format("/mb/mqs/{0}", queueId));
                                                log.Debug(string.Format("Polling queue by calling SSC Callservice with endpoint = {0}.", endPoint));

                                                //4b. Gör ett anrop till SSC CallService
                                                var queueClient = new SSC.Internal.SccPrivateService();
                                                try
                                                {
                                                    var queueResult = queueClient.CallService(new CallServiceRequestObj
                                                    {
                                                        Body = null,
                                                        EndpointMethod = endPoint,
                                                        Headers = null,
                                                        RequestType = "GET",
                                                    });

                                                    Console.WriteLine("Polling queue, http result: " + queueResult.StatusCode);
                                                    log.Debug("Polling queue, http result: " + queueResult.StatusCode);

                                                    //4c. Lagra respons i UploadedMessage
                                                    if (queueResult.StatusCode == 200)
                                                    {
                                                        var spisUrl = ConfigurationManager.AppSettings["spisUrl"];
                                                        spisUrl = spisUrl.Replace("{instance}", databaseName);

                                                        log.Debug(string.Format("Store response in UploadedMessage table through public api endpoint {0}", spisUrl));
                                                        var xml = new XmlDocument();
                                                        xml.LoadXml(queueResult.Body);

                                                        XmlNodeList messages = xml.SelectNodes("//*[local-name()='portCallMessage']");
                                                        Console.WriteLine(messages.Count + " messages fetched from queue");

                                                        for (int i = 0; i < messages.Count; i++)
                                                        {
                                                            if (messages[i].OuterXml.Length > 0)
                                                            {
                                                                WebHeaderCollection h = new WebHeaderCollection();
                                                                h.Add("content-type", "text/xml; charset=UTF8");

                                                                var uploadResponse = WebRequestHelper.Post(spisUrl, messages[i].OuterXml, h, true);
                                                                if (uploadResponse.HttpStatusCode != HttpStatusCode.OK)
                                                                {
                                                                    log.Debug("Failed to store message in SPIS. " + uploadResponse.Body);
                                                                }
                                                            }
                                                        }
                                                        Console.WriteLine(messages.Count + " messages sent to SPIS");
                                                        log.Debug(string.Format("{0} messages sent to SPIS.", messages.Count));

                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.ToString());
                                                    log.Error(string.Format("Catching exception for database {0}, error: {1}", databaseName, ex.ToString()));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Exception: " + ex.ToString());
                                }
                            }
                            else
                            {
                                log.Debug(string.Format("Table SpisSubscription did not exist in database {0}", databaseName));
                            }
                        }
                    }
                }
                log.Debug(Environment.NewLine);
                log.Debug("********************************************************************************************");
                log.Debug("End Program PollingQueueWorker");
                log.Debug("********************************************************************************************");
                log.Debug(Environment.NewLine);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        private static string CreateQueue(string queueEndpoint, DataTable settings)
        {
            string queueId = string.Empty;

            var IMO = settings.Rows[0]["IMO"].ToString();
            var MMSI = settings.Rows[0]["MMSI"].ToString();

            string vesselId = ConfigurationManager.AppSettings["vesselId"];
            if (!string.IsNullOrEmpty(IMO))
            {
                vesselId += "IMO:" + IMO;
            }
            else if (!string.IsNullOrEmpty(MMSI))
            {
                vesselId += "MMSI:" + MMSI;
            }
            else
            {
                Console.WriteLine("Failed to find vessel identifier.");
                return string.Empty;
            }

            var queueFilter = new List<QueueFilter>
            {
                new QueueFilter
                {
                    type="VESSEL", element=vesselId
                }
                //new QueueFilter
                //{
                //    type="TIME_TYPE", element="ESTIMATED"
                //},
                //new QueueFilter
                //{
                //    type="TIME_TYPE", element="RECOMMENDED"
                //}
            };

            var filter = JsonConvert.SerializeObject(queueFilter);

            var client = new SSC.Internal.SccPrivateService();
            try
            {
                var headers = new List<Header>
                {
                    new Header("content-type", "application/json; charset=UTF8; encoding='utf-8'")
                };

                var queueResult = client.CallService(new CallServiceRequestObj
                {
                    Body = filter,
                    EndpointMethod = WebRequestHelper.CombineUrl(queueEndpoint, "/mb/mqs"),
                    Headers = headers,
                    RequestType = "POST",
                });

                if (queueResult.StatusCode == 200 || queueResult.StatusCode == 201)
                {
                    queueId = queueResult.Body;
                }
                else
                {
                    throw new Exception("Unable to create queue. " + queueResult.StatusCode + " " + queueResult.Body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return queueId;
        }

        private static DataTable LookUpTable(string tableName, SqlConnection connection)
        {
            string query = "Select * from " + tableName;
            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            try
            {
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool IsValidQueue(DateTime validFrom, DateTime validTo)
        {
            return DateTime.UtcNow > validFrom && DateTime.UtcNow < validTo;
        }

        private static bool TableExists(SqlConnection connection, string tableName)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                var sql = string.Format(
                        "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{0}'",
                         tableName);
                command.CommandText = sql;
                var count = Convert.ToInt32(command.ExecuteScalar());
                return (count > 0);
            }
        }
    }
}
