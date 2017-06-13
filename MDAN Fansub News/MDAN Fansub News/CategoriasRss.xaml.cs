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
                
                if (!Frame.CanGoBack) return;
                Frame.GoBack();
                a.Handled = true;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetCheckList();
            base.OnNavigatedTo(e);
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

        private void SetCheckList()
        {
            foreach (var category in _user.Cats)
            {
                if (category == 1 || category == 2 || category == 5)
                    animeCheck.IsChecked = true;
                if (category == 3)
                    filmeCheck.IsChecked = true;
                if (category == 4)
                    ostCheck.IsChecked = true;
                if (category == 7)
                    mangaCheck.IsChecked = true;
                if (category == 6)
                    liveCheck.IsChecked = true;
            }
            
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
