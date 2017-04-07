using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace STM.Common.XmlParsers
{
    public class XmlParserBase
    {
        private XmlDocument _document;
        private XmlNamespaceManager _namespaceManager;

        public XmlParserBase(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            _document = doc;
        }

        public void SetNamespaces (SortedList<string, string> namespaces = null)
        {
            _namespaceManager = new XmlNamespaceManager(_document.NameTable);
            if (namespaces != null)
            {
                foreach (var item in namespaces)
                    _namespaceManager.AddNamespace(item.Key, item.Value);
            }
        }

        public string GetXml(string xpath)
        {
            var node = GetNode(xpath);
            if (node == null)
                return string.Empty;

            return node.OuterXml;
        }

        public string GetValue(string xpath)
        {
            var node = GetNode(xpath);
            if (node == null)
                return string.Empty;

            return node.InnerText;
        }

        public XmlNode GetNode(string xpath)
        {
            XmlNode root = _document.DocumentElement;

            XmlNode node = null;
            if (_namespaceManager != null)
            {
                node = root.SelectSingleNode(xpath, _namespaceManager);
            }
            else
            {
                node = root.SelectSingleNode(xpath);
            }

            return node;
        }
    }
}
