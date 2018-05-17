using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAN.Base
{
    public class RssDownloader
    {
        public async Task<string> DownloadRssStringAsync()
        {
            var wc = new System.Net.Http.HttpClient();
            var rssContent = string.Empty;
            try
            {
               return  rssContent = await wc.GetStringAsync("http://mdan.org/feed/");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
