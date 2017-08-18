using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Site
    {
        List<RSSItem> _mainList = new List<RSSItem>();

        public Site()
        {
            InitializeComponent();
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


        public void SetMainNews()
        {
            try
            {
                for (var i = 0; i <= 2; i++)
                {
                    if (i == 0)
                    {
                        newRelease.Text = _mainList[i].Title1;
                        newImage.Source = new BitmapImage(new Uri(_mainList[i].Image1));
                        newImage.Visibility = Visibility.Visible;
                        ApplicationData.Current.LocalSettings.Values["LastUp"] = _mainList[i].Title1;

                    }
                    if (i == 1)
                    {
                        newRelease1.Text = _mainList[i].Title1;
                        newImage1.Source = new BitmapImage(new Uri(_mainList[i].Image1));
                        newImage1.Visibility = Visibility.Visible;
                    }
                    if (i != 2) continue;
                    newRelease2.Text = _mainList[i].Title1;
                    newImage2.Source = new BitmapImage(new Uri(_mainList[i].Image1));
                    newImage2.Visibility = Visibility.Visible;
                }

            }
            finally
            {
                newImage.Visibility = Visibility.Visible;
            }

        }


        private async void Reader()
        {
            try
            {
                _mainList = await DataDownloader.Reader();
                SetMainNews();
                listRss.ItemsSource = _mainList;
            }
            catch (Exception ex)
            {
                var message = string.Format("Ocorreu um erro durante o acesso ao tracker. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", ex.Message);
                var dialog = new MessageDialog(message) { Title = "Erro" };
                dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                await dialog.ShowAsync();
            }
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Reader();
        }

        

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var index = listRss.SelectedIndex;
            Uri uri = new Uri(_mainList[index+3].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(_mainList[0].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(_mainList[1].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(_mainList[2].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}

