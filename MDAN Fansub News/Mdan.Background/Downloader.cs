using MDAN_App_Base;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Mdan.Background
{
    public sealed class Downloader
    {


        internal async Task<string> DataDownloader()
        {
            var wc = new HttpClient();
            var result = await wc.GetStringAsync("http://mdan.org/feed/");
            return result;
        }
    }
}
