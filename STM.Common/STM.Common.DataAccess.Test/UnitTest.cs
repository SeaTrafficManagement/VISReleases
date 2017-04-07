using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using STM.Common;
using STM.Common.DataAccess;
using System.Windows.Forms;

namespace STM.Common.DataAccess.Test
{
    [TestClass]
    public class UnitTest
    {
        private StmDbContext _dbContext;
        private LogDbContext _logContext;

        [TestInitialize]
        public void Init()
        {
            _dbContext = new StmDbContext();
            _logContext = new LogDbContext();
        }

        [TestMethod]
        public void PassingTest()
        {
            var items = _dbContext.PublishedRtzMessage.ToList();
            Assert.AreEqual(4, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        [Ignore]
        [TestMethod]
        public void LogEventServiceTester()
        {
            var service = new STM.Common.Services.Internal.LogEventService(_logContext);
            var parameters = new List<KeyValuePair<string, string>>();
            var p1 = new KeyValuePair<string, string>("uvid", "mrn:stm:voyage:id:vis12345566");
            var p2 = new KeyValuePair<string, string>("callbackEndpoint", @"http://vistest.cloudapp.net/vis1/voyageplans");
            var p3 = new KeyValuePair<string, string>("testaTom", "");
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"C:\Users\extperflo01\Dropbox\STM Utveckling\Test Data 2016-12-22a\Test Data";
            string data = string.Empty;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;
                string[] filelines = File.ReadAllLines(filename);
                foreach (var line in filelines)
                {
                    data += line;
                }
            }
            parameters.Add(p1);
            parameters.Add(p3);
            parameters.Add(p2);
            service.LogInfo(Entities.EventNumber.VIS_publishMessage, Entities.EventDataType.RTZ, parameters, data);

        }
    }
}
