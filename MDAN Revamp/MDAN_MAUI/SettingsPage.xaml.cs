using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace MDAN_MAUI
{
    public partial class SettingsPage : ContentPage
    {
        private bool _isInitializing = true;

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
            _isInitializing = false;
        }

        private void LoadSettings()
        {
            // Load Notifications Enable Switch
            bool notificationsEnabled = Preferences.Default.Get("NotificationsEnabled", true);
            NotificationSwitch.IsToggled = notificationsEnabled;

            // Load Notification Interval
            int intervalIndex = Preferences.Default.Get("NotificationIntervalIndex", 2); // default to "1 hora" (index 2)
            IntervalPicker.SelectedIndex = intervalIndex;

            // Load Theme Preference
            int themeIndex = Preferences.Default.Get("AppThemeIndex", 0); // default to "System" (index 0)
            ThemePicker.SelectedIndex = themeIndex;
        }

        private void OnNotificationToggled(object sender, ToggledEventArgs e)
        {
            if (_isInitializing) return;

            Preferences.Default.Set("NotificationsEnabled", e.Value);

            // Here we could trigger starting or stopping background workers
            if (e.Value)
            {
                // Re-enable worker scheduling if needed
            }
            else
            {
                // Cancel worker if needed
            }
        }

        private void OnIntervalChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            int selectedIndex = IntervalPicker.SelectedIndex;
            if (selectedIndex >= 0)
            {
                Preferences.Default.Set("NotificationIntervalIndex", selectedIndex);
                
                // Save actual minutes based on index
                int minutes = selectedIndex switch
                {
                    0 => 15,
                    1 => 30,
                    2 => 60,
                    3 => 120,
                    _ => 60
                };
                Preferences.Default.Set("NotificationIntervalMinutes", minutes);
            }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            int selectedIndex = ThemePicker.SelectedIndex;
            if (selectedIndex >= 0)
            {
                Preferences.Default.Set("AppThemeIndex", selectedIndex);

                AppTheme theme = selectedIndex switch
                {
                    0 => AppTheme.Unspecified, // System Default
                    1 => AppTheme.Light,
                    2 => AppTheme.Dark,
                    _ => AppTheme.Unspecified
                };

                if (Application.Current != null)
                {
                    Application.Current.UserAppTheme = theme;
                }
            }
        }

        private async void OnTestNotificationClicked(object sender, EventArgs e)
        {
            if (!LocalNotificationCenter.Current.AreNotificationsEnabled().Result)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            var request = new NotificationRequest
            {
                NotificationId = 9999,
                Title = "MDAN Fansub Lançamentos",
                Subtitle = "Teste de Alerta",
                Description = "Seu aplicativo está configurado! Você receberá avisos de novos animes e mangás.",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}
