using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Work;
using System;
using MDAN_MAUI.Platforms.Android;
using Microsoft.Maui.Storage;

namespace MDAN_MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ScheduleBackgroundSync();
    }

    private void ScheduleBackgroundSync()
    {
        try
        {
            var enabled = Preferences.Default.Get("NotificationsEnabled", true);
            if (enabled)
            {
                var minutes = Preferences.Default.Get("NotificationIntervalMinutes", 60);
                var syncWorkRequest = PeriodicWorkRequest.Builder.From<MdanFeedWorker>(TimeSpan.FromMinutes(minutes))
                    .Build();

                WorkManager.GetInstance(Android.App.Application.Context).EnqueueUniquePeriodicWork(
                    "MdanFeedSync",
                    ExistingPeriodicWorkPolicy.Keep,
                    syncWorkRequest);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to schedule WorkManager: {ex.Message}");
        }
    }
}
