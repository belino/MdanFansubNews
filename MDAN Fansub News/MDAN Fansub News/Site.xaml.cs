using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.UI.Popups;
using MDAN.Base;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Site : Page
    {
        List<RSSItem> mainList = new List<RSSItem>();
        private const string JSONFILENAME = "data.json";
        public Site()
        {
            this.InitializeComponent();

            var back = "Windows.Phone.UI.Input.HardwareButtons";
            if (!ApiInformation.IsTypePresent(back))
            {
                listRss.FlowDirection = FlowDirection.LeftToRight;
            }
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (!Frame.CanGoBack) return;
                Frame.GoBack();
                a.Handled = true;
            };

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, a) =>
                {
                    if (!Frame.CanGoBack) return;
                    Frame.GoBack();
                    a.Handled = true;
                };
            }
        }

        private async void writeJSONAsync(string deb)
        {

            var serializer = new DataContractJsonSerializer(typeof(string));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                          JSONFILENAME,
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, deb);
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GetContent();
        }

        private async void GetContent()
        {
            var siteContent = new SiteRssContent();
            mainList = await siteContent.GetSiteContent();
            listRss.ItemsSource = mainList;
            for (var i = 0; i <= 2; i++)
            {
                if (i == 0)
                {
                    newRelease.Text = mainList[i].Title;
                    newImage.Source = new BitmapImage(new Uri(mainList[i].Image));
                    newImage.Visibility = Visibility.Visible;
                    writeJSONAsync(mainList[i].Title);
                    ApplicationData.Current.LocalSettings.Values["LastUp"] = mainList[i].Title;

                }
                if (i == 1)
                {
                    newRelease1.Text = mainList[i].Title;
                    newImage1.Source = new BitmapImage(new Uri(mainList[i].Image));
                    newImage1.Visibility = Visibility.Visible;
                }
                if (i == 2)
                {
                    newRelease2.Text = mainList[i].Title;
                    newImage2.Source = new BitmapImage(new Uri(mainList[i].Image));
                    newImage2.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int index = listRss.SelectedIndex;
            Uri uri = new Uri(mainList[index].Link);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[0].Link);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[1].Link);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[2].Link);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}

