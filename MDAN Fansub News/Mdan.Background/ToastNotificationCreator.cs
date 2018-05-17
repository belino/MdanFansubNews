using Windows.UI.Notifications;
using MDAN.Base;
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
                            Text = item.Title
                        },

                        new AdaptiveImage()
                        {
                            Source = item.Image
                        }
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "/images/mdanlogo1.png",
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
