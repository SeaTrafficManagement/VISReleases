using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.Common.XmlParsers
{
    public class RtzParserFactory
    {
        public static IRtzParser Create(string xml)
        {
            if (xml.Contains("xmlns=\"http://www.cirm.org/RTZ/1/0\""))
            {
                return new Rtz10Parser(xml);
            }
            else if (xml.Contains("xmlns=\"http://www.cirm.org/RTZ/1/1\""))
            {
                return new Rtz11Parser(xml);
            }
            else
                throw new Exception("Invalid RTZ");
        }
    }
}