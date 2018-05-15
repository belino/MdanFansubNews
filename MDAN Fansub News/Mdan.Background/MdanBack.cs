﻿using MDAN_App_Base;
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
           var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();
            taskInstance.Canceled += OnCanceled;
            _deferral = taskInstance.GetDeferral();
            if (ApplicationData.Current.LocalSettings.Values["Notifications"] == null) return;
            if (!(bool)ApplicationData.Current.LocalSettings.Values["Notifications"]) return;
            try
            {
                var down = new Downloader();
                var res = await down.DataDownloader();
                var rssData = from rss in XElement.Parse(res).Descendants("item")
                                  select new RSSItem
                                  {
                                      Title = rss.Element("title")?.Value.TrimStart()
                                  };
                var firstImage = GetImagesInHTMLString(res).FirstOrDefault();
                var rssItems = rssData as IList<RSSItem> ?? rssData.ToList();
                rssItems.ToList().FirstOrDefault().Image = firstImage;
                    var lista = rssItems.ToList();

                
                var first = lista.FirstOrDefault();
                first.Image = firstImage;
                UpdateStatusAndTime(first);
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

        private List<string> GetImagesInHTMLString(string htmlString)
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
