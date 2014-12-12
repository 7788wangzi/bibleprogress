using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace BibleProcess
{
    public class Converter
    {
        public static string DoConversion(string text)
        {
            XmlDocument _xDoc = new XmlDocument();
            _xDoc.LoadXml(text);
            StringBuilder keyWordsBuilder = new StringBuilder();
            XmlNodeList _titleNodes = _xDoc.SelectNodes("chapter/content/b");
            bool _hasKeywordsOnPage = false;
            foreach (var node in _titleNodes)
            {
                string _keyWord = node.InnerText;
                keyWordsBuilder.Append(string.Format(@"<b>{0}</b>; ", _keyWord));
                _hasKeywordsOnPage = true;
            }
            if (_hasKeywordsOnPage)
            {
                //keyWordsBuilder.AppendLine("<br />");
                //keyWordsBuilder.AppendLine("<br />");
            }

            StringBuilder contentBuilder = new StringBuilder();
            XmlNodeList _verseNodes = _xDoc.SelectNodes("chapter/content/verse");
            foreach (var node in _verseNodes)
            {
                string _verseID = node.Attributes.Item(0).InnerText;
                contentBuilder.Append(string.Format("<p><sup>{0}</sup> {1}</p>", _verseID, node.InnerText));
                //contentBuilder.AppendLine("<br />");
                //contentBuilder.AppendLine("<br />");
            }
            return keyWordsBuilder.ToString() + contentBuilder.ToString();
        }
    }
}
