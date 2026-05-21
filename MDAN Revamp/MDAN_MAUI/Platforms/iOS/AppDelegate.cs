using Foundation;
using UIKit;
using System;
using Plugin.LocalNotification;
using Microsoft.Maui.Storage;

namespace MDAN_MAUI;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		// Set background fetch interval (minimum background fetch interval supported by iOS)
		application.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
		
		return base.FinishedLaunching(application, launchOptions);
	}

	[Export("application:performFetchWithCompletionHandler:")]
	public async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
	{
		try
		{
			var enabled = Preferences.Default.Get("NotificationsEnabled", true);
			if (!enabled)
			{
				completionHandler(UIBackgroundFetchResult.NoData);
				return;
			}

			var service = new Services.RssService();
			var items = await service.FetchFeedAsync(useMockAsFallback: false);
			
			if (items != null && items.Count > 0)
			{
				var latestItem = items[0];
				var lastTitle = Preferences.Default.Get("LastNotificationTitle", string.Empty);
				
				if (latestItem.Title != lastTitle)
				{
					Preferences.Default.Set("LastNotificationTitle", latestItem.Title);

					var request = new NotificationRequest
					{
						NotificationId = 3003,
						Title = "Novo Lançamento MDAN!",
						Description = latestItem.Title,
						BadgeNumber = 1
					};
					
					await LocalNotificationCenter.Current.Show(request);
					completionHandler(UIBackgroundFetchResult.NewData);
					return;
				}
			}
			completionHandler(UIBackgroundFetchResult.NoData);
		}
		catch (Exception)
		{
			completionHandler(UIBackgroundFetchResult.Failed);
		}
	}
}
