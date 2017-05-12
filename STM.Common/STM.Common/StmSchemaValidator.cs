using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using STM.Common;
using STM.Common.Exceptions;
using System.Reflection;
using STM.Common.XmlParsers;

namespace STM.Common
{
    public class StmSchemaValidator
    {
        private string cachekey = "StmIncomingSchema";
        private static object lockobject = new object();
        private string valResult;

        private SchemaValidator validator { get; set; }

        public StmSchemaValidator()
        {
            lock (lockobject)
            {
                validator = SchemaValidator.GetSchemaValidator(cachekey);
                if (validator == null)
                {
                    var schemaAssembly = Assembly.GetAssembly(typeof(STM.Common.Schema.Schema));
                    var names = schemaAssembly.GetManifestResourceNames();
                    names = names.OrderBy(x => x).ToArray();

                    validator = new SchemaValidator(schemaAssembly, names.ToList());
                    SchemaValidator.CacheSchemaValidator(validator, cachekey);
                }
            }
        }

        public void ValidateAreaMessageXML(string msg)
        {
            valResult = null;
            if (msg == null)
            {
                throw new StmSchemaValidationException("Area message is null. Not a valid message.");
            }

            valResult = validator.ValidateXML(msg);

            if (!string.IsNullOrEmpty(valResult))
            {
                throw new StmSchemaValidationException("Area message does not validate ok. Details: " + valResult);
            }
        }

        /// <summary>
        /// Validate all parts of a RTZ message
        /// </summary>
        /// <param name="msg"></param>
        public void ValidateRTZMessageXML(string msg)
        {
            valResult = null;
            if (msg == null)
            {
                throw new StmSchemaValidationException("Message is null. Not a valid message.");
            }

            valResult = validator.ValidateXML(msg, true);

            if (!string.IsNullOrEmpty(valResult))
            {
                throw new StmSchemaValidationException("Message does not validate ok. Details: " + valResult);
            }
        }

        public void Validate()
        {

        }

        /// <summary>
        /// Validate all parts of a RTZ message
        /// </summary>
        /// <param name="msg"></param>
        public void ValidateRTZMessage(string msg)
        {
            // Reset
            valResult = "";

            if (msg == null)
            {
                throw new StmSchemaValidationException("STM message is null. Not a valid message.");
            }

            var parser = RtzParserFactory.Create(msg);

            if (string.IsNullOrEmpty(parser.RouteInfo) || 
                string.IsNullOrEmpty(parser.WayPoints))
            {
                throw new StmSchemaValidationException("Some or all parts of the STM message are null. Not a valid message.");
            }

            var rtz11parser = parser as Rtz11Parser;
            if (rtz11parser != null)
            {
                if (string.IsNullOrEmpty(rtz11parser.StmRouteInfoExtension))
                {
                    throw new StmSchemaValidationException("Missing STM route info extension");
                }
            }

            if (!string.IsNullOrEmpty(parser.RouteStatus))
            {
                int status = Convert.ToInt32(parser.RouteStatus);
                if(status < 1 || status > 8)
                {
                    throw new ArgumentOutOfRangeException("routeInfo.routeStatus", status, "Forbidden status value.");
                }

            }
            else
            {
                throw new ArgumentNullException("RouteStatusEnum in STM extension can not be null or empty", "Mandatory element is null.");
            }

            if (valResult != "")
            {
                throw new StmSchemaValidationException("Message from external does not validate ok. Details: " + valResult);

            }
        }

        public void ValidateUVID(string messageId, string messageRTZ)
        {
            if (!string.IsNullOrEmpty(messageRTZ))
            {
                var parser = RtzParserFactory.Create(messageRTZ);
                string routeInfo = parser.RouteInfo;
                if(routeInfo == null)
                {
                    throw new StmSchemaValidationException("Could not read routeInfo from RTZ message.");
                }

                string vesselVoyage = parser.VesselVoyage;
                if(vesselVoyage == null)
                {
                    throw new StmSchemaValidationException("Could not read uvid from RTZ message.");
                }

                if (vesselVoyage.ToLower() != messageId.ToLower())
                {
                    throw new StmSchemaValidationException(string.Format("Inconsistent UVID: UVID in argument = {0} must be equal UVID in RTZ = {1}.",
                        messageId, vesselVoyage));
                }
            }
            else
            {
                throw new StmSchemaValidationException("Could not read UVID from RTZ message.");
            }
        }

        /// <summary>
        /// Validate all parts of a Text message
        /// </summary>
        /// <param name="msg"></param>
        public void ValidateTextMessage(string msg)
        {
            if (msg == null)
            {
                throw new StmSchemaValidationException("STM message is null. Not a valid message.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void ValidateTextMessageXML(string msg)
        {
            valResult = null;
            if (msg == null)
            {
                throw new StmSchemaValidationException("Text message is null. Not a valid message.");
            }

            valResult = validator.ValidateXML(msg);

            if (!string.IsNullOrEmpty(valResult))
            {
                throw new StmSchemaValidationException("Text message does not validate ok. Details: " + valResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void ValidatePCMMessageXML(string msg)
        {
            valResult = null;
            if (msg == null)
            {
                throw new StmSchemaValidationException("PCM message is null. Not a valid message.");
            }

            valResult = validator.ValidateXML(msg);

            if (!string.IsNullOrEmpty(valResult))
            {
                throw new StmSchemaValidationException("PCM message does not validate ok. Details: " + valResult);
            }
        }

        /// <summary>
        /// Validate a sub-part of STMMessage (e.g. "MAI" or "OAP"). Add validation errors to valResult.
        /// </summary>
        /// <param name="msgPart"></param>
        /// <param name="msgPartName"></param>
        private void ValidateMessagePart(XmlElement msgPart, string msgPartName)
        {
            if (msgPart != null)
            {
                string res = validator.ValidateXML(msgPart.OuterXml);

                if (res != null)
                {
                    valResult += string.Format("{0}-message: {1}", msgPartName, res);
                }
            }
        }
    }
}