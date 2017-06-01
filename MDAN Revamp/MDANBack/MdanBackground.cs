using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI.Notifications;
using NotificationsExtensions.ToastContent;
using NotificationsExtensions.TileContent;
using Windows.Storage;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;

namespace MDANBack
{
    public sealed class MdanBackground : IBackgroundTask
    {
        List<RSSItem> mainList = new List<RSSItem>();
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            if ((bool)ApplicationData.Current.LocalSettings.Values["Notifications"])
            {
                BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
                try
                {
                    await Downloader();

                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private async Task Downloader()
        {
            var wc = new HttpClient();


            var rssContent = await wc.GetStringAsync("http://mdan.org/feed/");
            //while (rssContent.Equals(string.Empty))
            //{
            //    Debug.WriteLine(cont++);
            //}
            var rssData = from rss in XElement.Parse(rssContent).Descendants("item")
                          select new RSSItem
                          {
                              Title1 = rss.Element("title")?.Value.TrimStart()
                          };
            mainList = rssData.ToList<RSSItem>();
            UpdateStatusAndTime();
            wc.Dispose();

        }

        private void UpdateStatusAndTime()
        {
            

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("LastUp"))
            {
                var data = ApplicationData.Current.LocalSettings.Values["LastUp"].ToString();
                if (!data.Equals(mainList[0].Title1))
                {
                    ApplicationData.Current.LocalSettings.Values["LastUp"] = mainList[0].Title1;
                    ITileWide310x150Text09 tileContent = TileContentFactory.CreateTileWide310x150Text09();
                    tileContent.TextHeading.Text = "Novo Lançamento!";
                    tileContent.TextBodyWrap.Text = mainList[0].Title1;

                    //Always add a 150x150 tile update also  
                    ITileSquare150x150Text04 squareContent = TileContentFactory.CreateTileSquare150x150Text04();
                    squareContent.TextBodyWrap.Text = mainList[0].Title1;
                    tileContent.Square150x150Content = squareContent;
                    TileNotification tileNotification = new TileNotification(tileContent.GetXml());
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
                    tileContent.TextBodyWrap.Text = mainList[0].Title1;


                    IToastText02 toastContent = ToastContentFactory.CreateToastText02();
                    toastContent.TextHeading.Text = "Novo Lançamento";
                    toastContent.TextBodyWrap.Text = mainList[0].Title1;

                    var toast = new ToastNotification(toastContent.GetXml());

                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }

            }



        }
    }
}
