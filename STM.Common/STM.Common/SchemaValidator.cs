using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.IO;
using System.Xml;
using System.Reflection;

namespace STM.Common
{
    /// <summary>
    /// A validator class that checks that a given XML document
    /// conforms to a given schema.
    /// </summary>
    public class SchemaValidator
    {
        // Private members.
        private int _numberOfErrors;
        private StringBuilder _sb;
        private XmlSchemaSet _schemaSet;
        private List<XmlSchema> _tempSchemas; // Schemas added when default constructor is used

        // Initialize Log object
        //private static LogEntry logger = Logging.GetDefaultClassLogger();

        /// <summary>
        /// Constructor. The list of schemas will be added to a schema-set and prepared 
        /// for validation.
        /// </summary>
        /// <param name="schemas">A list of schemas that will be used for validation of messages</param>
        public SchemaValidator(List<XmlSchema> schemas)
        {
            // Prepare the schema set by adding the schemas and compiling it...
            PrepareSchemaSet(schemas);
        }

        /// <summary>
        /// Constructor. Prepare a SchemaValidator with a set of schemas from embeddes resource paths
        /// </summary>
        /// <param name="schemaFilePath"></param>
        public SchemaValidator(Assembly assbmbly, List<string> schemaResourcePaths)
        {
            List<XmlSchema> schemas = LoadEmbeddedSchemas(assbmbly, schemaResourcePaths);

            // Prepare the schema set by adding the schemas and compiling it...
            PrepareSchemaSet(schemas);
        }

        /// <summary>
        /// Constructor. Prepare a SchemaValidator with a set of schemas at the given path
        /// </summary>
        /// <param name="schemaFilePath"></param>
        public SchemaValidator(string schemaFilePath)
        {
            List<XmlSchema> schemas = LoadSchemasFromDisk(schemaFilePath);

            // Prepare the schema set by adding the schemas and compiling it...
            PrepareSchemaSet(schemas);

        }

        /// <summary>
        /// Default constructor. Requires manual adding of schemas and
        /// then a call to Prepare() to make it ready for validation.
        /// </summary>
        public SchemaValidator()
        {
        }


        /// <summary>
        /// Call Prepare() when using the default constructor and schemas are added
        /// manually through AddSchema(). Only call Prepare after all schemas that will 
        /// be used in validation have been added!
        /// </summary>
        public void Prepare()
        {
            // Prepare by sending in all added schemas
            PrepareSchemaSet(_tempSchemas);
        }


        /// <summary>
        /// Used in combination with default constructor where
        /// schemas are added afterwards, also requiring a call to 
        /// Prepare() before SchemaValidator is ready to 
        /// validate schemas.
        /// </summary>
        /// <param name="schemaXML">A string with the schema XML</param>
        public void AddSchema(string schemaXML)
        {
            if (_tempSchemas == null)
            {
                // Create new list
                _tempSchemas = new List<XmlSchema>();
            }

            try
            {
                // Read the XML schema as a string...
                XmlSchema schema = GetSchema(schemaXML);
                // And add to temp schemas...
                _tempSchemas.Add(schema);
            }
            catch (Exception ex)
            {
                throw new Exception("Attempted to add an invalid schema to SchemaValidator", ex);
            }

        }

        public List<XmlSchema> LoadEmbeddedSchemas(Assembly assembly, List<string> resourceNames)
        {
            var result = new List<XmlSchema>();

            foreach (var resourceName in resourceNames)
            {
                Stream schemaStream = null;

                try
                { 
                    schemaStream = assembly.GetManifestResourceStream(resourceName);
                    using (XmlReader schemaReader = XmlReader.Create(schemaStream))
                    {
                        if (schemaReader != null)
                        {
                            StringBuilder sb = new StringBuilder();

                            while (schemaReader.Read())
                                sb.AppendLine(schemaReader.ReadOuterXml());

                            StringReader strReader = new StringReader(sb.ToString());
                            result.Add(XmlSchema.Read(strReader, null));
                        }
                    }
                }
                finally
                {
                    if (schemaStream != null)
                        schemaStream.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// Load a set of schemas from disk at the given path
        /// </summary>
        /// <param name="path"></param>
        public List<XmlSchema> LoadSchemasFromDisk(string path)
        {
            List<XmlSchema> schemas = new List<XmlSchema>();

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                string schemaStr = File.ReadAllText(file);
                XmlSchema schema = GetSchema(schemaStr);
                schemas.Add(schema);
            }

            return schemas;
        }

        protected static string cacheKey_SchemaValidator = "SchemaValidator";

        /// <summary>
        /// Get the cached instance, or null
        /// </summary>
        /// <param name="cachekey">optional cachekey to enable different validatorsets in cache</param>
        /// <returns></returns>
        public static SchemaValidator GetSchemaValidator(string cachekey = null)
        {
            return (SchemaValidator)CacheUtils.GetFromCache<SchemaValidator>(cacheKey_SchemaValidator + cachekey);
        }

        private static object lockObject = new object();

        /// <summary>
        /// Get the cached instance with schema validator prepared with loaded schemas from disk
        /// Create one if the first time.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cachekey">optional cachekey to enable different validatorsets in cache</param>
        /// <returns></returns>
        public static SchemaValidator GetSchemaValidator(string path, string cachekey = null)
        {
            SchemaValidator validator = GetSchemaValidator(path + cachekey);

            if (validator == null)
            {
                lock (lockObject)
                {
                    validator = new SchemaValidator(path);

                    SchemaValidator.CacheSchemaValidator(validator, cachekey);
                }
            }

            return validator;
        }


        /// <summary>
        /// Utility method for converting a string to an XML schema object
        /// </summary>
        /// <param name="schemaXML">A string with the schema XML</param>
        /// <returns></returns>
        public XmlSchema GetSchema(string schemaXML)
        {
            // Read the XML schema as a string...
            XmlSchema schema = new XmlSchema();
            StringReader strReader = new StringReader(schemaXML);
            schema = XmlSchema.Read(strReader, null);

            return schema;
        }


        /// <summary>
        /// Set a cached instance
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="cachekey">optional cachekey to enable different validatorsets in cache</param>
        public static void CacheSchemaValidator(SchemaValidator validator, string cachekey = null)
        {
            CacheUtils.AddToTimerCache(cacheKey_SchemaValidator + cachekey, validator, 14400);
        }

        /// <summary>
        /// Clear the cache. Used during administration to force a schema config re-read.
        /// <param name="cachekey">optional cachekey to enable different validatorsets in cache</param>
        /// </summary>
        public static void ClearSchemaValidatorCache(string cachekey = null)
        {
            CacheUtils.ClearFromCache(cacheKey_SchemaValidator + cachekey);
        }

        /// <summary>
        /// Utility method for getting the Qualified root name of a XML message
        /// on the format root-ns/root-element-name. This is the method used
        /// throughout the system to uniquely identify messages.
        /// 
        /// Example: "sjofartsverket.se/ES/FT/PrelData/1.0/PrelDataAdded"
        /// </summary>
        /// <param name="root">The XmlNode to start from.</param>
        /// <returns>The qualified root name</returns>
        public static string GetQualifiedRootName(XmlNode root)
        {
            // Make sure we have something to work on...
            if (root == null)
            {
                return null;
            }

            string qualRootName = null;

            string ns = root.NamespaceURI;
            string name = root.Name;

            if (name != null)
            {
                int nsPrefixPos = name.IndexOf(":");
                if (nsPrefixPos > -1)
                {
                    // Remove the first part since we do not want
                    // ns prefix as part of the qualified root name!
                    name = name.Substring(nsPrefixPos + 1);
                }
            }

            qualRootName = ns + "/" + name;

            return qualRootName;
        }


        /// <summary>
        /// Clear result to prepare for a new validation from the same instance
        /// of this class
        /// </summary>
        private void ClearValidationResult()
        {
            _numberOfErrors = 0;
            _sb = new StringBuilder();
        }

        /// <summary>
        /// Takes all schemas and places them in a SchemaSet. 
        /// </summary>
        /// <returns></returns>
        private void PrepareSchemaSet(List<XmlSchema> schemas)
        {
            if (schemas == null)
            {
                // Nothing to work with!
                return;
            }

            // Create a new schema set
            _schemaSet = new XmlSchemaSet();

            // And fill it with schemas
            foreach (XmlSchema schema in schemas)
            {
                _schemaSet.Add(schema);
                //Logging.Debug(logger, String.Format("Loaded schema with target namespace '{0}'", schema.TargetNamespace));
            }

            // Prepare the SchemaSet by compiling it
            _schemaSet.Compile();

        }

        /// <summary>
        /// Validate a given xml document in string form againt one or more schemas already loaded into
        /// a SchemaSet during contruction of this class.
        /// </summary>
        /// <param name="xmlToValidate">The xml to validate, in string form</param>
        /// <returns></returns>
        public string ValidateXML(string xmlToValidate, bool ignoreWarnings = false)
        {
            XmlReader xmlReader = null;

            // Init and clear
            ClearValidationResult();

            try
            {
                if(_schemaSet == null)
                {
                    return "There is no schema to validate against!";
                }

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xmlToValidate);
                //XmlNode root = doc.DocumentElement;

                //XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                //nsmgr.AddNamespace("ns1", "http://www.iho.int/S124/gml/1.0");
                //nsmgr.AddNamespace("ns2", "http://www.iho.int/S124/gml/1.0");
                //nsmgr.AddNamespace("ns3", "http://www.opengis.net/gml/3.2");
                //nsmgr.AddNamespace("ns4", "http://www.iho.int/S-100/profile/s100_gmlProfile");
                //nsmgr.AddNamespace("ns5", "http://www.iho.int/s100gml/1.0");


                // Create XML reader settings
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.ValidationType = ValidationType.Schema;

                if (!ignoreWarnings)
                    xmlSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

                xmlSettings.Schemas = _schemaSet;
                xmlSettings.ValidationEventHandler += new
                    ValidationEventHandler(ValidationErrorHandler);

                //XmlParserContext inputContext = new XmlParserContext(nsmgr.NameTable, nsmgr,
                //    "en", XmlSpace.None);
                // We need an XmlReader as input to the validation...
                //xmlReader = XmlReader.Create(new StringReader(xmlToValidate), xmlSettings, inputContext);
                xmlReader = XmlReader.Create(new StringReader(xmlToValidate), xmlSettings);

                // And now just read through the whole file to see
                // if it is valid. If there are errors, stop when 
                // a total of max 5 has been reached.
                while (xmlReader.Read() && _numberOfErrors < 5) ;


            }
            catch (Exception ex)
            {
                _numberOfErrors++;
                _sb.Append(" Exception occured: " + ex.Message);
            }
            finally
            {
                // Clean up resources
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }

            }

            // Now check if there have been any validation 
            // errors:
            if (_numberOfErrors == 0)
            {
                // Nope, everything ok.
                return null;
            }
            else
            {
                // One or more errors have occured. Return
                // the complete string describing the errors.
                return _sb.ToString();
            }
        }

        /// <summary>
        /// This is the event handler for schema validation errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidationErrorHandler(object sender, ValidationEventArgs e)
        {
            // Get the message of the validation errors.
            string message = e.Message;

            // Increase error counter...
            _numberOfErrors++;

            // ...and append the descriptive text of the validation error:
            _sb.Append(" " + e.Severity + " " + _numberOfErrors + ": " + message);

        }
    }
}
