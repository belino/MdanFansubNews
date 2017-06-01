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
            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent(back))
            {
                listRss.FlowDirection = FlowDirection.LeftToRight;
            }
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                Debug.WriteLine("BackRequested");
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, a) =>
                {
                    Debug.WriteLine("BackPressed");
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                        a.Handled = true;
                    }
                };
            }
        }

        private async void writeJSONAsync(string deb)
        {

            // Notice that the write is ALMOST identical ... except for the serializer.
            var serializer = new DataContractJsonSerializer(typeof(string));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                          JSONFILENAME,
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, deb);
            }

        }

        private async void Reader()
        {
            // In this event we need to create the web client who's going to download the informations from our link  
            var wc = new System.Net.Http.HttpClient();
            var rssContent = string.Empty;
            var error = false;
            try
            {
                 rssContent = await wc.GetStringAsync("http://mdan.org/feed/");
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog(string.Format("Aconteceu algo estranho. Erro: {0}", ex.Message)); await dialog.ShowAsync();
                error = true;
            }
            finally
            {
                if(error)
                Frame.Navigate(typeof(Error));
            }

            var result = XElement.Parse(rssContent).Descendants("item").ToList();
            foreach(var x in result)
            {
                var noticia = new RSSItem
                {
                    Title1 = x.Element("title").Value.TrimStart(),
                    pubDate1 = x.Element("pubDate").Value.Substring(0, 22),
                    Description1 =
                        WebUtility.HtmlDecode(
                            Regex.Replace(x.Element("description").Value.Replace("\r", "").Replace("\n", " "),
                                @"<[^>]+>|&nbsp;", "").Trim()),
                    Link1 = x.Element("link").Value
                };
                noticia.Image1 = GetImagesInHTMLString(x.Value).Count > 0 ? GetImagesInHTMLString(x.Value)[0] : @"http://cdn.meme.am/instances/250x250/62004543.jpg";
                mainList.Add(noticia);
            }
            Debug.WriteLine(mainList.Count);
            var rssData = from rss in XElement.Parse(rssContent).Descendants("item")
                          select new RSSItem
                          {
                              Title1 = rss.Element("title").Value.TrimStart(),
                              
                              pubDate1 = rss.Element("pubDate").Value.Substring(0,22),
                              Description1 = WebUtility.HtmlDecode(Regex.Replace(rss.Element("description").Value.Replace("\r", "").Replace("\n", " "), @"<[^>]+>|&nbsp;", "").Trim()),
                              Link1 = rss.Element("link").Value,
                              Image1 = GetImagesInHTMLString(rss.Value)[0]
                          };
            //mainList = RssData.ToList();
            

            try {
                for(var i =0; i <= 2; i++)
                {
                    if( i == 0) {
                        newRelease.Text = mainList[i].Title1;
                        newImage.Source = new BitmapImage(new Uri(mainList[i].Image1));
                        newImage.Visibility = Visibility.Visible;
                        writeJSONAsync(mainList[i].Title1);
                        ApplicationData.Current.LocalSettings.Values["LastUp"] = mainList[i].Title1;
                        
                    }
                    if(i == 1)
                    {
                        newRelease1.Text = mainList[i].Title1;
                        newImage1.Source = new BitmapImage(new Uri(mainList[i].Image1));
                        newImage1.Visibility = Visibility.Visible;
                    }
                    if (i == 2)
                    {
                        newRelease2.Text = mainList[i].Title1;
                        newImage2.Source = new BitmapImage(new Uri(mainList[i].Image1));
                        newImage2.Visibility = Visibility.Visible;
                    }
                    
                }
                
            }
            catch(Exception e)
            {
                var dialog = new MessageDialog(string.Format("Aconteceu algo estranho. Erro: {0}", e.Message)); await dialog.ShowAsync();
               Frame.Navigate(typeof(Error));
            }
            finally
            {
                newImage.Visibility = Visibility.Visible;
            }
            
            listRss.ItemsSource = mainList;
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Reader();
        }

        private List<string> GetImagesInHTMLString(string htmlString)
        {
            var images = new List<string>();
            
            const string pattern = @"http://i.imgur\b[^>]*>";
            const string pattern2 = @"http://imgur\b[^>]*>";
            const string pattern3= @"https://s^(([0-9]*)|(([0-9]*)\.([0-9]*)))$\b[^>]*>";
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = rgx.Matches(htmlString);
            if(matches.Count == 0)
            {
                rgx = new Regex(pattern2, RegexOptions.IgnoreCase);
                matches = rgx.Matches(htmlString);
                if (matches.Count == 0)
                {
                    rgx = new Regex(pattern3, RegexOptions.IgnoreCase);
                    matches = rgx.Matches(htmlString);
                }

            }
            for (int i = 0, l = matches.Count; i < l; i++)
            {
                //var fixedString = Regex.Replace(matches[i].Value, @"<[^>]+>|&nbsp;", "").Trim();
                //var fixedString2 = Regex.Replace(fixedString, @"\s{2,}", " ");
                var fixedString2 = Regex.Replace(matches[i].Value, "<img ", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\\ class=\"", string.Empty);
                var index = fixedString2.LastIndexOf("border=", StringComparison.Ordinal);
                if (index > 0)
                    fixedString2 = fixedString2.Substring(0, index);
                fixedString2 = Regex.Replace(fixedString2, "\\ border=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "src=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\\ alt=\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "\"", string.Empty);
                fixedString2 = Regex.Replace(fixedString2, "/>", string.Empty);
                fixedString2 = WebUtility.HtmlDecode(fixedString2).Trim();
                images.Add(fixedString2);
            }

            return images;
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int index = listRss.SelectedIndex;
            Uri uri = new Uri(mainList[index+3].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[0].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[1].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Grid_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(mainList[2].Link1);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}

