using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAN.Base
{
    public class RssDownloader
    {
        private readonly User _user = User.Instance;
        public async Task<string> DownloadRssSiteStringAsync()
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

        public async Task<string> DownloadRssTrackerStringAsync()
        {
            //HttpWebRequest wc = (HttpWebRequest)WebRequest.Create(_user.TrackerUri);

            //wc.ContentType = "text/xml;";
            //wc.Accept = "text/xml";
            //wc.Method = "GET";
            //wc.UseDefaultCredentials = true;
            //var error = false;
            var wc = new System.Net.Http.HttpClient();
            var rssContent = string.Empty;
            try
            {
                return rssContent = await wc.GetStringAsync(_user.TrackerUri);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
