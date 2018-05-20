using MDAN.Base;
using System;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPage 
    {
        User user = User.Instance;
        public UserPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (!Frame.CanGoBack) return;
                Frame.GoBack();
                a.Handled = true;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (user.Username != null)
            {
                username.Text = user.Username;
                userAvatar.Source = new BitmapImage(new Uri(user.Avatar));
                notificationSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values["Notifications"];
            }
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new MessageDialog("Tem certeza que deseja sair?");
            dialog.Title = "Mas já vai?";
            dialog.Commands.Add(new UICommand { Label = "Sim", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancelar", Id = 1 });
            var res = await dialog.ShowAsync();
            if ((int)res.Id == 0)
            {
                user.Status = false;
                user.Username = "";
                user.TrackerUri = "";
                user.Id = "";
                user.Avatar = "";
                ApplicationData.Current.LocalSettings.Values["UserID"] = user.Id;
                ApplicationData.Current.LocalSettings.Values["userName"] = user.Username;
                ApplicationData.Current.LocalSettings.Values["status"] = user.Status;
                ApplicationData.Current.LocalSettings.Values["Avatar"] = user.Avatar;
                ApplicationData.Current.LocalSettings.Values["Tracker"] = user.TrackerUri;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => Frame.Navigate(typeof(Tracker)));
            }
        }

        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
           await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => Frame.Navigate(typeof(CategoriasRss)));
        }

        private void notificationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            user.Notifications = notificationSwitch.IsOn;
            ApplicationData.Current.LocalSettings.Values["Notifications"] = user.Notifications;
        }
    }
}
