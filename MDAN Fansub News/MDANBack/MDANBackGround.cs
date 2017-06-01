using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using System.Net.Http;
using System.Xml.Linq;
using Windows.Storage;
using System.Diagnostics;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;
using NotificationsExtensions.Tiles;
using MDAN_App_Base;


namespace MDANBack
{
    public sealed class MdanBackGround : IBackgroundTask
    {
        private List<RSSItem> _mainList = new List<RSSItem>();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            if ((bool)ApplicationData.Current.LocalSettings.Values["Notifications"]) { 
                try {
                    Downloader down = new Downloader();
                    down.DataDownloader();
                var res = down.lista;
                }
                catch(Exception ex)
                {
                    Debug.Write(ex.Message);
                }
                finally
                { 
                    deferral.Complete();
                }
            }
        }

        

       

        private void UpdateStatusAndTime()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("LastUp")) return;
            if (ApplicationData.Current.LocalSettings.Values["LastUp"].Equals(_mainList[0].Title1)) return;
            if (_mainList[0] == null) return; 
            ApplicationData.Current.LocalSettings.Values["LastUp"] = _mainList[0].Title1;
                    
            TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive()
            {


                Children =
                {
                    new TileText()
                    {
                        Text = "Novo lançamento!",
                        Style = TileTextStyle.Body
                    },

                    new TileText()
                    {
                        Text = _mainList[0].Title1,
                        Wrap = true,
                        Style = TileTextStyle.CaptionSubtle
                    }
                        
                }
            };
            var binding = new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,

                DisplayName = "MDAN Fansub",

                Content = bindingContent
            };
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = binding,
                    TileWide = binding,
                    TileLarge = binding
                }
            };
            var doc = content.GetXml();

            var notification = new TileNotification(doc);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Update(notification);


            var visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = "MDAN - Novo Lançamento"

                },

                BodyTextLine1 = new ToastText()
                {
                    Text = _mainList[0].Title1,
                },
            };
            var toastContent = new ToastContent()
            {
                Visual = visual
            };
            var tileNotification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
