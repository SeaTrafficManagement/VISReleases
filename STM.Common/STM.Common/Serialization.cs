using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace STM.Common
{
    public class Serialization
    {
        private Serialization()
        { }

        /// <summary>
        /// Deserialize a given XML-string into an object of the given type.
        /// </summary>
        /// <param name="xmlToDeserialize"></param>
        /// <param name="typeToDeserialize"></param>
        /// <returns></returns>
        /// 
        public static object DeserializeXML(string xmlToDeserialize, Type typeToDeserialize)
        {
            MemoryStream memstream = null;
            object resultObject = null;

            try
            {
                XmlSerializer typeDeserializer = new XmlSerializer(typeToDeserialize);

                memstream = new MemoryStream();

                byte[] xmlBytes = StrToByteArray(xmlToDeserialize);

                memstream.Write(xmlBytes, 0, xmlBytes.Length);
                memstream.Position = 0;

                resultObject = typeDeserializer.Deserialize(memstream);
            }
            finally
            {
                if(memstream != null)
                {
                    memstream.Close();
                }
            }

            return resultObject;
        }

        /// Write a serialized version of an object to file
        /// </summary>
        /// <param name="objIn"></param>
        /// <param name="fileName">A complete file name</param>
        /// <param name="appendToFile">set to true if content should be appended to an existing file</param>
        public static void SerializeObj(object objIn, string fileName, bool appendToFile)
        {
            StreamWriter os = null;
            try
            {
                os = new StreamWriter(fileName, appendToFile, UTF8Encoding.UTF8);

                if(objIn != null)
                {
                    XmlSerializer s = new XmlSerializer(objIn.GetType());
                    s.Serialize(os, objIn);
                }
            }
            finally
            {
                if(os != null)
                {
                    os.Flush();
                    os.Close();
                }
            }
        }

        /// <summary>
        /// Serialixe an object to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        /// 
        public static string Serialize<T>(T message)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();

            using (StringWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, message);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Serialixe an object to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        /// 
        public static XmlDocument SerializeXML<T>(T message)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();

            using (StringWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, message);
            }
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sb.ToString());

            return xd;

        }

        /// <summary>
        /// Deserialize XML to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string message)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader rdr = new StringReader(message))
            {
                return (T)ser.Deserialize(rdr);
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            UTF8Encoding enc = new UTF8Encoding();
            return enc.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] arr)
        {
            return System.Text.Encoding.UTF8.GetString(arr);
        }
    }
}
