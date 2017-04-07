using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.XmlParsers
{
    public class S124Parser : XmlParserBase
    {
        public S124Parser(string xml) : base(xml)
        {
        }

        public string AreaMessageId
        {
            get
            {
                return GetValue("/referenceUVID");
            }
        }
    }
}
