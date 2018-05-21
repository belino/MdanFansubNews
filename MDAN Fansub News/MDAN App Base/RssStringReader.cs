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

        public List<RSSItem> ReadTrackerRss(string rssSourceString)
        {
            if (rssSourceString.Length != 0)
            {

                var rssData = from rss in XElement.Parse(rssSourceString).Descendants("item")
                              select new RSSItem
                              {
                                  Title = rss.Element("title")?.Value.TrimStart(),
                                  //pubDate1 = rss.Element("decription").Value.Substring(0, 22),
                                  Description = StringTreatment(WebUtility.HtmlDecode(Regex.Replace(rss.Element("description")?.Value.Replace("\r", "").Replace("\n", " "), @"(<[^>]+>|&nbsp;)", "").Trim())),
                                  //Description1 = Regex.Replace(Description1, "([[+a-zA-Z/(?:d*\\.)?\\d+]+])", ""),
                                  Link = rss.Element("link")?.Value,
                                  Image = GetImagesInDesc(rss.Element("description")?.ToString())
                              };

                var rssItems = rssData as IList<RSSItem> ?? rssData.ToList();
                return rssItems.ToList();
            }
            return null;
        }

        private String StringTreatment(string s)
        {
            //int index = s.LastIndexOf("[img]");
            //if (index > 0)
            //    s = s.Substring(0, index);

            var k = WebUtility.HtmlDecode(Regex.Replace(s, "(http)(.*?)(?=\\[\\/IMG)|(http)(.*?)(?=\\[\\/img)|(\\[i\\])|(\\[IMG+\\]|\\[/IMG])|(\\[img+\\]|\\[/img])|(\\[size=(?:\\d*\\.)?\\d+|\\[/size)\\]|(\\[b+\\]|\\[/b])|((https)(.*?)(?=\\]))|(\\[.+?\\])", ""));
            k = Regex.Replace(k, "\\[.+?\\]", "");
            if (k.Length > 150)
            {
                k = k.Remove(k.Length - 5, 5) + "[...]";
            }
            //string m = Regex.Replace(k, "(http)(.*?)(?=\\[\\/IMG)|(http)(.*?)(?=\\[\\/img)", "");

            return k.Trim();
        }

        private string GetImagesInDesc(string desc)
        {
            string image = @"Images\";
            if (desc.Contains("Category: Episódios"))
            {
                image += "serie.jpg";
                return image;
            }
            if (desc.Contains("Category: Filmes"))
            {
                image += "movie.jpg";
                return image;
            }
            if (desc.Contains("Category: OST"))
            {
                image += "ost.jpg";
                return image;
            }
            if (desc.Contains("Category: Live"))
            {
                image += "live.jpg";
                return image;
            }
            if (desc.Contains("Category: Mangá"))
            {
                image += "manga.jpg";
                return image;
            }
            if (desc.Contains("Category: OVAs"))
            {
                image += "ova.jpg";
                return image;
            }
            if (desc.Contains("Category: Completo"))
            {
                image += "completo.jpg";
                return image;
            }

            return image;
        }
    }
}
