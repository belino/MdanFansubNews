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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{

    public sealed partial class Tracker
    {
        private readonly User _user = User.Instance;
        public List<RSSItem> MainList = new List<RSSItem>();

        public Tracker()
        {
            InitializeComponent();



            if (_user.TrackerUri != null)
            {
                if (_user.TrackerUri.Contains("http"))
                    RssRetriever();

            }
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
               
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };

        }


        private async void RssRetriever()
        {


            HttpWebRequest wc = (HttpWebRequest)WebRequest.Create(_user.TrackerUri);

            wc.ContentType = "text/xml;";
            wc.Accept = "text/xml";
            wc.Method = "GET";
            wc.UseDefaultCredentials = true;
            var error = false;
            try
            {
                using (WebResponse webResponse = await wc.GetResponseAsync())
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    var rssData = reader.ReadToEnd();
                  
                    TrackerReader(rssData);
                }

            }

            catch (Exception e)
            {
                var message = string.Format("Ocorreu um erro durante o acesso ao tracker. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", e.Message);
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

        public string GetString(string uri)
        {

            var httpClient = new HttpClient();

            var response = httpClient.GetStringAsync(uri).Result;
            return response;
        }


        private void TrackerReader(String rssContent)
        {

            if (rssContent.Length != 0)
            {

                var rssData = from rss in XElement.Parse(rssContent).Descendants("item")
                              select new RSSItem
                              {
                                  Title1 = rss.Element("title")?.Value.TrimStart(),
                                  //pubDate1 = rss.Element("decription").Value.Substring(0, 22),
                                  Description1 = stringTreatment(WebUtility.HtmlDecode(Regex.Replace(rss.Element("description")?.Value.Replace("\r", "").Replace("\n", " "), @"(<[^>]+>|&nbsp;)", "").Trim())),
                                  //Description1 = Regex.Replace(Description1, "([[+a-zA-Z/(?:d*\\.)?\\d+]+])", ""),
                                  Link1 = rss.Element("link")?.Value,
                                  Image1 = GetImagesInDesc(rss.Element("description")?.ToString())
                              };

                var rssItems = rssData as IList<RSSItem> ?? rssData.ToList();
                MainList = rssItems.ToList();
                listRss.ItemsSource = rssItems.ToList();
            }
            else
            {
                erroText.Visibility = Visibility.Visible;
            }
        }



        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           
                if ((_user.TrackerUri == null) || (_user.TrackerUri.Equals("")))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                             () => Frame.Navigate(typeof(CategoriasRss)));
                }
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var index = listRss.SelectedIndex;
            var uri = new Uri(MainList[index].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private String stringTreatment(string s)
        {
            //int index = s.LastIndexOf("[img]");
            //if (index > 0)
            //    s = s.Substring(0, index);

            var k = WebUtility.HtmlDecode(Regex.Replace(s, "(http)(.*?)(?=\\[\\/IMG)|(http)(.*?)(?=\\[\\/img)|(\\[i\\])|(\\[IMG+\\]|\\[/IMG])|(\\[img+\\]|\\[/img])|(\\[size=(?:\\d*\\.)?\\d+|\\[/size)\\]|(\\[b+\\]|\\[/b])|((https)(.*?)(?=\\]))|(\\[.+?\\])", ""));
            k = Regex.Replace(k, "\\[.+?\\]", "");
            if (k.Length > 150)
            {
                k = k.Remove(k.Length - 5, 5) + "[...]";
            }
            //string m = Regex.Replace(k, "(http)(.*?)(?=\\[\\/IMG)|(http)(.*?)(?=\\[\\/img)", "");
            
            return k.Trim();
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                             () => Frame.Navigate(typeof(UserPage)));
        }

        private string GetImagesInDesc(string desc)
        {
            string image = @"Images\";
            if (desc.Contains("Category: Episódios"))
            {
                image += "serie.jpg";
                return image;
            }
            if (desc.Contains("Category: Filmes"))
            {
                image += "movie.jpg";
                return image;
            }
            if (desc.Contains("Category: OST"))
            {
                image += "ost.jpg";
                return image;
            }
            if (desc.Contains("Category: Live"))
            {
                image += "live.jpg";
                return image;
            }
            if (desc.Contains("Category: Mangá"))
            {
                image += "manga.jpg";
                return image;
            }
            if (desc.Contains("Category: OVAs"))
            {
                image += "ova.jpg";
                return image;
            }

            return image;
        }

        private void SettingButton(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CategoriasRss));
        }
    }
}
