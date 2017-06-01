using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriasRss 
    {
        private readonly User _user = User.Instance;
        private readonly List<int> _listcat = new List<int>();
        public CategoriasRss()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                Debug.WriteLine("BackRequested");
                if (!Frame.CanGoBack) return;
                Frame.GoBack();
                a.Handled = true;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_user.Username != null)
            {
                userNameBlock.Text = _user.Username;
                userAvatar.Source = new BitmapImage(new Uri(_user.Avatar));
            }
            else
            {
                Frame.Navigate(typeof(Login));
            }
            
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CheckList();
            _user.Cats = _listcat;
            _user.TrackerUriGenerator();
            ApplicationData.Current.LocalSettings.Values["Tracker"] = _user.TrackerUri;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           () => Frame.Navigate(typeof(Tracker)));
        }

        private void CheckList()
        {
            if (animeCheck.IsChecked == true)
            {
                _listcat.Add(1);
                _listcat.Add(2);
                _listcat.Add(5);
            }
            if(filmeCheck.IsChecked == true)
            {
                _listcat.Add(3);
            }
            if(ostCheck.IsChecked == true)
            {
                _listcat.Add(4);
            }if(mangaCheck.IsChecked == true)
            {
                _listcat.Add(7);
            }
            if(liveCheck.IsChecked == true)
            {
                _listcat.Add(6);
            }
        }
    }
}
