using MDAN.Base;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Mdan.Background
{
    public sealed class MdanBack : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var imageDecoder = new ImageDecoder();
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();
            taskInstance.Canceled += OnCanceled;
            _deferral = taskInstance.GetDeferral();
            if (ApplicationData.Current.LocalSettings.Values["Notifications"] == null) return;
            if (!(bool)ApplicationData.Current.LocalSettings.Values["Notifications"]) return;
            try
            {
                var siteContent = new SiteRssContent();
                var result = await siteContent.GetSiteContent();
                UpdateStatusAndTime(result.First());
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            finally
            {
                _deferral.Complete();
            }
        }

        

        private static void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }

        private static void RefreshTile(string title)
        {
            var bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveText
                    {
                        Text = "Último lançamento",
                        HintStyle = AdaptiveTextStyle.Body
                    },

                    new AdaptiveText
                    {
                        Text = title,
                        HintWrap = true,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },
                }
            };
            var binding = new TileBinding
            {
                Branding = TileBranding.NameAndLogo,

                DisplayName = "MDAN Fansub",

                Content = bindingContent,

            };
            var content = new TileContent
            {
                Visual = new TileVisual
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
        }


        private static void CreateNewEntranceTile(RSSItem newsTitle)
        {
            var counter = int.Parse(ApplicationData.Current.LocalSettings.Values["Counter"].ToString()) + 1;
            TileNotificationCreator.CreateNewTileNotification(newsTitle);
            ToastNotificationCreator.CreateNewToastNotification(newsTitle);
            SetBadgeNumber(counter);
        }

        private static void UpdateStatusAndTime(RSSItem item)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("LastUp")) return;
            if (ApplicationData.Current.LocalSettings.Values["LastUp"].Equals(item.Title))
            {
                RefreshTile(item.Title);
            }
            else
            {
                
                CreateNewEntranceTile(item);
                ApplicationData.Current.LocalSettings.Values["LastUp"] = item.Title;
            }
        }

        private static void SetBadgeNumber(int num)
        {

            // Get the blank badge XML payload for a badge number
            var badgeXml =
                BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Set the value of the badge in the XML to our number
            var badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            badgeElement?.SetAttribute("value", num.ToString());

            // Create the badge notification
            var badge = new BadgeNotification(badgeXml);

            // Create the badge updater for the application
            var badgeUpdater =
                BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            // And update the badge
            badgeUpdater.Update(badge);

        }
    }
}
