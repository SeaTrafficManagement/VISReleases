using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace STM.StmModule.Simulator.Utils
{
    public class XmlUtil
    {
        public static string FormatXml(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            StringBuilder sb = new StringBuilder();
            TextWriter tr = new StringWriterWithEncoding(sb, Encoding.UTF8);
            XmlTextWriter wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.Indented;
            doc.Save(wr);
            wr.Close();
            return sb.ToString();
        }
    }

    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }

}
