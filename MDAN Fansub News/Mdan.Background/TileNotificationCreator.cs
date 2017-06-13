using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;
using MDAN_App_Base;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;

namespace Mdan.Background
{
    internal static class TileNotificationCreator
    {
        internal static void CreateNewTileNotification(RSSItem item)
        {
            
            var bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new AdaptiveText
                    {
                        Text = "Novo lançamento!",
                        HintStyle = AdaptiveTextStyle.Body
                    },

                    new AdaptiveText
                    {
                        Text = item.Title1,
                        HintWrap = true,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },

                    new AdaptiveImage
                    {
                        Source = item.Image1,
                        HintCrop = AdaptiveImageCrop.Default
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
            var tileNotification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
