using System;
using Android.Content;
using AndroidX.Work;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using MDAN_MAUI.Services;
using Microsoft.Maui.Storage;

namespace MDAN_MAUI.Platforms.Android
{
    public class MdanFeedWorker : Worker
    {
        public MdanFeedWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            // Check if notifications are enabled by user
            bool enabled = Preferences.Default.Get("NotificationsEnabled", true);
            if (!enabled)
            {
                return Result.InvokeSuccess();
            }

            try
            {
                // Run background check to get feed
                var service = new RssService();
                // Pass false to avoid mock data fallback during background synchronization checks
                var items = service.FetchFeedAsync(useMockAsFallback: false).GetAwaiter().GetResult();
                
                if (items != null && items.Count > 0)
                {
                    var latestItem = items[0];
                    var lastTitle = Preferences.Default.Get("LastNotificationTitle", string.Empty);
                    
                    if (latestItem.Title != lastTitle)
                    {
                        // Save latest release title
                        Preferences.Default.Set("LastNotificationTitle", latestItem.Title);

                        // Dispatch local notification
                        var request = new NotificationRequest
                        {
                            NotificationId = 2002,
                            Title = "Novo Lançamento MDAN!",
                            Description = latestItem.Title,
                            BadgeNumber = 1,
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = DateTime.Now.AddSeconds(1)
                            }
                        };
                        LocalNotificationCenter.Current.Show(request).GetAwaiter().GetResult();
                    }
                }
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Background feed check failed: {ex.Message}");
                // Return success (or retry) to avoid crashing the scheduler worker thread
                return Result.InvokeSuccess();
            }
        }
    }
}
