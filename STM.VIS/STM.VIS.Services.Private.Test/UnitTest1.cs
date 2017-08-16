using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using STM.Common;
using System.IO;
using STM.Common.Services.Internal;
using STM.Common.DataAccess;
using STM.Common.DataAccess.Entities;
using System.Linq;
using STM.Common.Exceptions;
using STM.Common.XmlParsers;
using System.Text;

namespace STM.VIS.Services.Private.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private string _path = Path.Combine(Environment.CurrentDirectory, @"..\..\");
        private StmDbContext _dbContext;
        private LogDbContext _logContext;

        [TestInitialize]
        public void Init()
        {
            _dbContext = new StmDbContext();
            _logContext = new LogDbContext();
        }

        [TestMethod]
        public void TestRTZ_2_0()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\RTZ v1.0 STM several extensions-1.rtz");
            var msgString = Serialization.ByteArrayToString(msg);

            var parser = RtzParserFactory.Create(msgString);
            string uvid = parser.VesselVoyage;

            string routeStatus = parser.RouteStatus;

            //Assert.AreEqual("urn:mrn:stm:voyage:id:wallenius:0001", uvid);
        }

        [TestMethod]
        public void TestRTZMessageInincosistentUVID()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\msg1_invalidUVID.rtz");
            var msgString = Serialization.ByteArrayToString(msg);
            var parser = RtzParserFactory.Create(msgString);
            string uvid = parser.VesselVoyage;
            var validator = new StmSchemaValidator();

            try
            {
                validator.ValidateUVID("urn:mrn:stm:voyage:id:vis1:0001", msgString);
            }
            catch(StmSchemaValidationException ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void TestRTZMessageIsValid()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\msg1.rtz");
            SubmitMessageToVIS(msg);
        }

        [TestMethod]
        [ExpectedException(typeof(StmSchemaValidationException))]
        public void TestRTZMessageIsNotValid()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\msg1_notValid.rtz");
            SubmitMessageToVIS(msg);
        }

        [TestMethod]
        public void TestRTZMessageMissingRouteStatus()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\msg1_missingRouteStatus.rtz");
            var msgString = Serialization.ByteArrayToString(msg);
            var parser = RtzParserFactory.Create(msgString);
            string routeStatus = parser.RouteStatus;

            Assert.AreEqual(routeStatus, string.Empty);
        }

        [TestMethod]
        public void TestRTZMessageWithOkRouteStatus()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\msg1_RouteStatus_7.rtz");
            var msgString = Serialization.ByteArrayToString(msg);
            var parser = RtzParserFactory.Create(msgString);
            string routeStatus = parser.RouteStatus;

            Assert.AreEqual<string>("7", routeStatus);
        }

        [TestMethod]
        public void TestGetPublishedMessages()
        {
            var messages = _dbContext.PublishedRtzMessage.Where(m => (int)m.MessageStatus < 8).ToList();
            foreach(var msg in messages)
            {
                var result = new PublishedRtzMessage
                {
                    Message = msg.Message,
                    MessageID = msg.MessageID,
                    MessageLastUpdateTime = msg.MessageLastUpdateTime,
                    MessageStatus = msg.MessageStatus,
                    MessageType = msg.MessageType,
                    MessageValidFrom = msg.MessageValidFrom,
                    MessageValidTo = msg.MessageValidTo,
                    PublishTime = msg.PublishTime
                };
                string status = msg.MessageStatus.ToString();
                string type = msg.MessageType != null ? msg.MessageType.Name : string.Empty;
            }
        }
        private void SubmitMessageToVIS(byte[] msg)
        {
            PublishedRtzMessageService service = new PublishedRtzMessageService(_dbContext, _logContext);

            var messageType = _dbContext.MessageType.Where(m => m.Name == "RTZ").FirstOrDefault();
            //var parsedMsg = Serialization.Deserialize<Route>(Serialization.ByteArrayToString(msg));

            var parser = RtzParserFactory.Create(Serialization.ByteArrayToString(msg));
            string routeStatus = parser.RouteStatus;
            RouteStatus status;
            if (string.IsNullOrEmpty(routeStatus))
            {
                status = RouteStatus.Unknown;
            }
            else
            {
                status = (RouteStatus)Enum.Parse(typeof(RouteStatus), routeStatus);
            }
            var entityToInsert = new PublishedRtzMessage
            {
                Message = msg,
                MessageID = "urn:mrn:stm:voyage:id:vis1:0001",
                MessageLastUpdateTime = DateTime.UtcNow,
                MessageStatus = status,
                MessageType = messageType,
                MessageValidFrom = DateTime.UtcNow.AddDays(-3),
                MessageValidTo = DateTime.UtcNow.AddDays(3),
                PublishTime = DateTime.UtcNow.AddHours(-3)
            };

            try
            {
                service.Insert(entityToInsert);
                _dbContext.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }

        }

        private byte[] GetMessageRaw(string file)
        {
            string msg = GetXmlStrFromDisk(file);
            return Serialization.StrToByteArray(msg);
        }
        //private Route GetMessage(string file)
        //{
        //    string msg = GetXmlStrFromDisk(file);
        //    return Serialization.Deserialize<Route>(msg);
        //}

        private string GetXmlStrFromDisk(string file)
        {
            string xmlStr = File.ReadAllText(Path.Combine(_path, file));
            return xmlStr;
        }

        [TestMethod]
        public void TestPath()
        {
            string path1 = System.AppDomain.CurrentDomain.BaseDirectory;
            string path2 = Environment.CurrentDirectory;
        }

        [TestMethod]
        public void ValidOrgMrnTest()
        {
            var result = FormatValidation.IsValidOrgId("urn:mrn:stm:org:sma");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InvalidOrgMrnTest()
        {
            var result = FormatValidation.IsValidOrgId("urn:mrn:stm:fel:sma");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidServiceMrnTest()
        {
            var result = FormatValidation.IsValidServiceId("urn:mrn:stm:service:instance:sma:vistest001");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InvalidServiceMrnTest()
        {
            var result = FormatValidation.IsValidServiceId("urn:mrn:stm:service:fel:sma:vistest001");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidUvidTest()
        {
            var result = FormatValidation.IsValidUvid("urn:mrn:stm:voyage:id:sma:001");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InvalidUvidTest()
        {
            var result = FormatValidation.IsValidUvid("urn:mrn:stm:voyage:id:fel:sma:001");
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Rtz10Parser()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\RTZ v1.0 STM several extensions-1.rtz");
            var msgString = Serialization.ByteArrayToString(msg);

            var validator = new StmSchemaValidator();
            validator.ValidateRTZMessageXML(msgString);

            var parser = RtzParserFactory.Create(msgString);
            var status = parser.RouteStatus;
        }

        [TestMethod]
        public void Rtz11Parser()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\rtz_route_stm_1_1_28032017.rtz");
            var msgString = Serialization.ByteArrayToString(msg);

            var validator = new StmSchemaValidator();
            validator.ValidateRTZMessageXML(msgString);

            var parser = new Rtz11Parser(msgString);
            var status = parser.RouteStatus;
        }

        [TestMethod]
        public void TxtParser()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\textMessage with area-1.xml");
            var msgString = Serialization.ByteArrayToString(msg);

            var validator = new StmSchemaValidator();
            validator.ValidateTextMessageXML(msgString);

            var parser = new TxtParser(msgString);
            var id = parser.TextMessageId;
        }

        [TestMethod]
        public void S124Parser()
        {
            byte[] msg = GetMessageRaw(@"STMMessageSamples\S124_01.gml");
            var msgString = Serialization.ByteArrayToString(msg);

            var validator = new StmSchemaValidator();
            validator.ValidateAreaMessageXML(msgString);

            var parser = new S124Parser(msgString);
            var id = parser.AreaMessageId;
        }
    }
}