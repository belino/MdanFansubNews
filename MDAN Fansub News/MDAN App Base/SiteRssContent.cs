using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAN.Base
{
    public class SiteRssContent
    {
        public async Task<List<RSSItem>> GetSiteContent()
        {
            var downloader = new RssDownloader();
            var rssStringContent = await downloader.DownloadRssStringAsync();
            var rssReader = new RssStringReader();
            return rssReader.ReadSiteRss(rssStringContent);
        }
    }
}
