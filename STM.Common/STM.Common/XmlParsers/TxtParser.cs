using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.XmlParsers
{
    public class TxtParser : XmlParserBase
    {
        public TxtParser(string xml) : base(xml)
        {
            SetNamespaces(new SortedList<string, string>
            {
                {"txt", "http://tempuri.org/textMessageSchema.xsd" }
            });
        }

        public string TextMessageId
        {
            get
            {
                return GetValue(@"//txt:textMessage/txt:textMessageId");
            }
        }
    }
}