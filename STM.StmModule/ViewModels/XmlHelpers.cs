﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace STM.StmModule.Simulator.ViewModels
{
    public static class XmlHelpers
    {
        public static string FormatXml(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            StringBuilder sb = new StringBuilder();
            System.IO.TextWriter tr = new System.IO.StringWriter(sb);
            XmlTextWriter wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.Indented;
            doc.Save(wr);
            wr.Close();
            return sb.ToString();
        }
    }
}
