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
            SetNamespaces(new SortedList<string, string>
            {
                {"S124", "http://www.iho.int/S124/gml/1.0" }
            });
        }

        public string AreaMessageId
        {
            get
            {
                return GetValue(@"//S124:DataSet/imember/S124:S124_NWPreamble/id");
            }
        }
    }
}
