using MDAN.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


namespace MDAN_App_Base
{

    public sealed partial class Tracker
    {
        private readonly User _user = User.Instance;
        public List<RSSItem> MainList = new List<RSSItem>();

        public Tracker()
        {
            InitializeComponent();



            
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {

                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };

        }


        //private async void RssRetriever()
        //{


        //    HttpWebRequest wc = (HttpWebRequest)WebRequest.Create(_user.TrackerUri);

        //    wc.ContentType = "text/xml;";
        //    wc.Accept = "text/xml";
        //    wc.Method = "GET";
        //    wc.UseDefaultCredentials = true;
        //    var error = false;
        //    try
        //    {
        //        using (WebResponse webResponse = await wc.GetResponseAsync())
        //        {
        //            var reader = new StreamReader(webResponse.GetResponseStream());
        //            var rssData = reader.ReadToEnd();

        //            TrackerReader(rssData);
        //        }

        //    }

        //    catch (Exception e)
        //    {
        //        var message = string.Format("Ocorreu um erro durante o acesso ao tracker. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", e.Message);
        //        var dialog = new MessageDialog(message) { Title = "Erro" };
        //        dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
        //        await dialog.ShowAsync();
        //        error = true;
        //    }
        //    finally
        //    {
        //        if (error)
        //            Frame.Navigate(typeof(Site));
        //    }
        //}

        public string GetString(string uri)
        {

            var httpClient = new HttpClient();

            var response = httpClient.GetStringAsync(uri).Result;
            return response;
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var error = false;
            if ((_user.TrackerUri == null) || (_user.TrackerUri.Equals("")))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                         () => Frame.Navigate(typeof(CategoriasRss)));
            }
            try
            {
                GetContent();
            }
            catch (Exception ex)
            {
                var message = string.Format("Ocorreu um erro durante o acesso ao tracker. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", ex.Message);
                var dialog = new MessageDialog(message) { Title = "Erro" };
                dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                await dialog.ShowAsync();
                error = true;
            }
            finally
            {
                if (error)
                    Frame.Navigate(typeof(Site));
            }
            
        }

        private async void GetContent()
        {
            if (_user.TrackerUri != null)
            {
                if (_user.TrackerUri.Contains("http"))
                {
                    var retriever = new RssContentRetriever();
                    var result = await retriever.GeTrackerContent();
                    listRss.ItemsSource = result;
                }
            }
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var index = listRss.SelectedIndex;
            var uri = new Uri(MainList[index].Link);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }



        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                             () => Frame.Navigate(typeof(UserPage)));
        }



        private void SettingButton(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CategoriasRss));
        }
    }
}
