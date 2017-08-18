using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using MDAN_App_Base;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;

namespace Mdan.Background
{
    static class ToastNotificationCreator
    {
        internal static void CreateNewToastNotification(RSSItem item)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "MDAN - Novo Lançamento"
                        },

                        new AdaptiveText()
                        {
                            Text = item.Title1
                        },

                        new AdaptiveImage()
                        {
                            Source = item.Image1
                        }
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "/Images/mdanlogo1.png",
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };
            var toastContent = new ToastContent
            {
                Visual = visual
            };
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
