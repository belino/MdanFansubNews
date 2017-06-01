using MDAN_App_Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MDANBack
{
    internal class Downloader
    {
        public List<RSSItem> lista { get; set; }

        public async Task DataDownloader()
        {
            var wc = new HttpClient();
            var rssContent = await wc.GetStringAsync("http://mdan.org/feed/");
            if (rssContent != null)
            {
                var rssData = from rss in XElement.Parse(rssContent).Descendants("item")
                              select new RSSItem
                              {
                                  Title1 = rss.Element("title").Value.TrimStart()
                              };
                lista = rssData.ToList();
            }


            wc.Dispose();

        }
    }
}
