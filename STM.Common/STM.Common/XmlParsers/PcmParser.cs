using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.XmlParsers
{
    public class PcmParser : XmlParserBase
    {
        public PcmParser(string xml) : base(xml)
        {
        }

        public string PcmMessageId
        {
            get
            {
                return GetValue(@"//@messageId");
            }
        }
    }
}