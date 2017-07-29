using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MDAN_App_Base
{
    public static class DataDownloader
    {
        public static List<string> GetImagesInHtmlString(string htmlString)
        {
            var images = new List<string>();

            const string pattern = @"http://i.imgur\b[^>]*>";
            const string pattern2 = @"http://imgur\b[^>]*>";
            const string pattern3 = @"https://s^(([0-9]*)|(([0-9]*)\.([0-9]*)))$\b[^>]*>";
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = rgx.Matches(htmlString);
            if (matches.Count == 0)
            {
                rgx = new Regex(pattern2, RegexOptions.IgnoreCase);
                matches = rgx.Matches(htmlString);
                if (matches.Count == 0)
                {
                    rgx = new Regex(pattern3, RegexOptions.IgnoreCase);
                    matches = rgx.Matches(htmlString);
                }

            }
            for (int i = 0, l = matches.Count; i < l; i++)
            {
                //var fixedString = Regex.Replace(matches[i].Value, @"<[^>]+>|&nbsp;", "").Trim();
                //var fixedString2 = Regex.Replace(fixedString, @"\s{2,}", " ");
                var fixedString2 = Regex.Replace(matches[i].Value, "<img ", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\\ class=\"", string.Empty);
                var index = fixedString2.LastIndexOf("border=", StringComparison.Ordinal);
                if (index > 0)
                    fixedString2 = fixedString2.Substring(0, index);
                fixedString2 = Regex.Replace(fixedString2, "\\ border=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "src=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\\ alt=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "/>", string.Empty);
                fixedString2 = WebUtility.HtmlDecode(fixedString2).Trim();
                images.Add(fixedString2);
            }

            return images;
        }

        public static async Task<List<RSSItem>> Reader()
        {
            var wc = new System.Net.Http.HttpClient();
            string rssContent;
            try
            {
                rssContent = await wc.GetStringAsync("http://mdan.org/feed/");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            var result = XElement.Parse(rssContent).Descendants("item").ToList();
            return result.Select(x => new RSSItem
                {
                    Title1 = x.Element("title")?.Value.TrimStart(),
                    pubDate1 = x.Element("pubDate")?.Value.Substring(0, 22),
                    Description1 = WebUtility.HtmlDecode(Regex.Replace(x.Element("description")?.Value.Replace("\r", "").Replace("\n", " "), @"<[^>]+>|&nbsp;", "").Trim()),
                    Link1 = x.Element("link")?.Value,
                    Image1 = GetImagesInHtmlString(x.Value).Count > 0
                        ? GetImagesInHtmlString(x.Value)[0]
                        : @"http://cdn.meme.am/instances/250x250/62004543.jpg"
                })
                .ToList();
        }
    }
}
