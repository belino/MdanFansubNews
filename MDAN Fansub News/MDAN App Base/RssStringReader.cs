using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MDAN.Base
{
    public class RssStringReader
    {
        public List<RSSItem> ReadSiteRss(string rssSourceString)
        {
            var xmlParseResults = XElement.Parse(rssSourceString).Descendants("item").ToList();
            var imageDecoder = new ImageDecoder();
            var rssItemsList = new List<RSSItem>();
            foreach (var xmlResult in xmlParseResults)
            {
                var noticia = new RSSItem
                {
                    Title = xmlResult.Element("title").Value.TrimStart(),
                    PubDate = xmlResult.Element("pubDate").Value.Substring(0, 22),
                    Description =
                        WebUtility.HtmlDecode(
                            Regex.Replace(xmlResult.Element("description").Value.Replace("\r", "").Replace("\n", " "), @"<[^>]+>|&nbsp;", "").Trim()),
                    Link = xmlResult.Element("link").Value
                };
                noticia.Image = imageDecoder.GetImagesInHTMLString(xmlResult.Value).Count > 0 ? imageDecoder.GetImagesInHTMLString(xmlResult.Value)[0] : @"http://cdn.meme.am/instances/250x250/62004543.jpg";
                rssItemsList.Add(noticia);
            }
            return rssItemsList;
        }
    }
}
