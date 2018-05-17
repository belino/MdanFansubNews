using System;
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
            XNamespace content = "https://www.mdan.org/feed/";

            var items = XDocument.Parse(rssSourceString)
                .Descendants("item")
                .Select(i => new RSSItem
                {
                    Title = (string)i.Element("title"),
                    Description = WebUtility.HtmlDecode(
                            Regex.Replace((string)i.Element("description").Value.Replace("\r", "").Replace("\n", " "), @"<[^>]+>|&nbsp;", "").Trim()),
                    Link = (string)i.Element("link"),
                    NewsContent = (string)i.Element("{http://purl.org/rss/1.0/modules/content/}encoded"),
                    PubDate = i.Element("pubDate").Value.Substring(0, 22),
                    Image = imageDecoder.GetImagesInHTMLString(i.Value).Count > 0 ? imageDecoder.GetImagesInHTMLString(i.Value)[0] : @"http://cdn.meme.am/instances/250x250/62004543.jpg"
                })
                .ToList();

            return items;
        }
    }
}
