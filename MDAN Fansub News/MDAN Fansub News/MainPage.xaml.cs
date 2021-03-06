﻿using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Mdan.Background;
using Windows.UI.Notifications;
using System.Diagnostics;
using MDAN.Base;
using Windows.UI.Xaml;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        private readonly User _user = User.Instance;
        //public event EventHandler<BackRequestedEventArgs> BackRequested;
        public MainPage()
        {
            InitializeComponent();
            ApplicationData.Current.LocalSettings.Values["Notifications"] = _user.Notifications;
            ApplicationData.Current.LocalSettings.Values["Counter"] = 0;
            //_user.TrackerUri = ApplicationData.Current.LocalSettings.Values["Tracker"]?.ToString();
            //_user.CatsRetriever();
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();
            
        }

        private static async void NotiRequesterPhone()
        {
            await BackgroundExecutionManager.RequestAccessAsync();

        }

        private void HamburgerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            menu.IsPaneOpen = !menu.IsPaneOpen;
        }


        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == "MDANBack")
                    {
                        task.Value.Unregister(true);
                    }
                }

                var taskBuilder = new BackgroundTaskBuilder
                {
                    Name = "MDANBack",
                    TaskEntryPoint = typeof(MdanBack).ToString(),
                };
                taskBuilder.SetTrigger(new TimeTrigger(20, false));
                var registration = taskBuilder.Register();
            }
        }


    protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RestoreUser(_user);
            while (Frame.CanGoBack)
            {
                Frame.BackStack.Clear();
            }
            RegisterBackgroundTask();
            ((Frame) menu.Content)?.Navigate(typeof(Site));
        }

        private static void RestoreUser(User user)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("UserID")) return;
            user.Id = ApplicationData.Current.LocalSettings.Values["UserID"].ToString();
            user.Username = ApplicationData.Current.LocalSettings.Values["userName"].ToString();
            user.Status = (bool)ApplicationData.Current.LocalSettings.Values["status"];
            user.Avatar = ApplicationData.Current.LocalSettings.Values["Avatar"].ToString();
            if(ApplicationData.Current.LocalSettings.Values["Notifications"]!=null)
                user.Notifications = (bool)ApplicationData.Current.LocalSettings.Values["Notifications"];
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Tracker"))
                user.TrackerUri = ApplicationData.Current.LocalSettings.Values["Tracker"].ToString();
        }

        private void Home_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((Frame)menu.Content)?.Navigate(typeof(Site));
            if(menu.IsPaneOpen)
            menu.IsPaneOpen = !menu.IsPaneOpen;
        }

        private void Chat_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((Frame)menu.Content)?.Navigate(typeof(Chat));
            if (menu.IsPaneOpen)
                menu.IsPaneOpen = !menu.IsPaneOpen;
        }

        private void Tracker_Tapped(object sender, TappedRoutedEventArgs e)
        {

            ((Frame)menu.Content)?.Navigate(typeof(Tracker));
            if (menu.IsPaneOpen)
                menu.IsPaneOpen = !menu.IsPaneOpen;
        }

        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((Frame)menu.Content)?.Navigate(typeof(Settings));
            if (menu.IsPaneOpen)
                menu.IsPaneOpen = !menu.IsPaneOpen;
        }

        private void About_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((Frame)menu.Content)?.Navigate(typeof(About));
            if (menu.IsPaneOpen)
                menu.IsPaneOpen = !menu.IsPaneOpen;
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((Frame)menu.Content)?.Navigate(typeof(Login));
            if (menu.IsPaneOpen)
                menu.IsPaneOpen = !menu.IsPaneOpen;
        }
    }
}
