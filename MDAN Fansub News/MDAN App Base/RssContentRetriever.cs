using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAN.Base
{
    public class RssContentRetriever
    {
        public async Task<List<RSSItem>> GetSiteContent()
        {
            var downloader = new RssDownloader();
            var rssStringContent = await downloader.DownloadRssSiteStringAsync();
            var rssReader = new RssStringReader();
            return rssReader.ReadSiteRss(rssStringContent);
        }

        public async Task<List<RSSItem>> GeTrackerContent()
        {
            var downloader = new RssDownloader();
            var rssStringContent = await downloader.DownloadRssTrackerStringAsync();
            var rssReader = new RssStringReader();
            return rssReader.ReadTrackerRss(rssStringContent);
        }
    }
}
