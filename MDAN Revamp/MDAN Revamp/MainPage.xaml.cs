using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MDAN_Revamp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<RSSItem> mainList = new List<RSSItem>();
        private const string JSONFILENAME = "data.json";

        public MainPage()
        {
            this.InitializeComponent();
            this.RegisterBackGroundTask();
            this.NavigationCacheMode = NavigationCacheMode.Required;
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
            catch (Exception ex)
            {
                var dialog = new MessageDialog(string.Format("Aconteceu algo estranho. Erro: {0}", ex.Message)); await dialog.ShowAsync();
                error = true;
            }
           
            var result = XElement.Parse(rssContent).Descendants("item").ToList();
            foreach (var x in result)
            {
                var noticia = new RSSItem
                {
                    Title1 = x.Element("title")?.Value.TrimStart(),
                    pubDate1 = x.Element("pubDate")?.Value.Substring(0, 22),
                    Description1 =
                        WebUtility.HtmlDecode(
                            Regex.Replace(x.Element("description")?.Value.Replace("\r", "").Replace("\n", " "),
                                @"<[^>]+>|&nbsp;", "").Trim()),
                    Link1 = x.Element("link")?.Value,
                    Image1 = GetImagesInHTMLString(x.Value).Count > 0
                        ? GetImagesInHTMLString(x.Value)[0]
                        : @"http://cdn.meme.am/instances/250x250/62004543.jpg"
                };
                mainList.Add(noticia);
            }
            var rssData = from rss in XElement.Parse(rssContent).Descendants("item")
                select new RSSItem
                {
                    Title1 = rss.Element("title").Value.TrimStart(),

                    pubDate1 = rss.Element("pubDate").Value.Substring(0, 22),
                    Description1 = WebUtility.HtmlDecode(Regex.Replace(rss.Element("description").Value.Replace("\r", "").Replace("\n", " "), @"<[^>]+>|&nbsp;", "").Trim()),
                    Link1 = rss.Element("link").Value,
                    Image1 = GetImagesInHTMLString(rss.Value)[0]
                };


            try
            {
                for (var i = 0; i <= 2; i++)
                {
                    if (i == 0)
                    {
                        newRelease.Text = mainList[i].Title1;
                        newImage.Source = new BitmapImage(new Uri(mainList[i].Image1));
                        newImage.Visibility = Visibility.Visible;
                        writeJSONAsync(mainList[i].Title1);
                        ApplicationData.Current.LocalSettings.Values["LastUp"] = mainList[i].Title1;

                    }
                    if (i == 1)
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
            catch (Exception e)
            {
                var dialog = new MessageDialog(string.Format("Aconteceu algo estranho. Erro: {0}", e.Message)); await dialog.ShowAsync();
            }
            finally
            {
                newImage.Visibility = Visibility.Visible;
            }

            listRss.ItemsSource = mainList;
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


        async void RegisterBackGroundTask()
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.Denied)
            {
                ApplicationData.Current.LocalSettings.Values["Notifications"] = false;
            }
            else
            {
                if (access == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    try
                    {
                        var builder = new BackgroundTaskBuilder
                        {
                            Name = "MDANBack",
                            TaskEntryPoint = typeof(MDANBack.MdanBackground).FullName
                        };
                        IBackgroundTrigger trigger = new TimeTrigger(30, false);
                        builder.SetTrigger(trigger);
                        IBackgroundCondition condition = new SystemCondition(SystemConditionType.InternetAvailable);
                        builder.AddCondition(condition);
                        builder.Register();
                        
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

      
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Reader();
            
            
        }

        private List<string> GetImagesInHTMLString(string htmlString)
        {
            var images = new List<string>();

            const string pattern = @"http://i.imgur\b[^>]*>";
            const string pattern2 = @"http://imgur\b[^>]*>";
            const string pattern3 = @"https://s^(([0-9]*)|(([0-9]*)\.([0-9]*)))$\b[^>]*>";
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = rgx.Matches(htmlString);
            if (matches.Count == 0)
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
            Uri uri = new Uri(mainList[index].Link1);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
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

     

        private void SettingsPage_Tapped(object sender, RoutedEventArgs e)
        {
            var content = Window.Current.Content;
            var frame = content as Frame;
            try { 
            if (frame != null)
            {
                frame.Navigate(typeof(PageConfig));
            }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Window.Current.Activate();
            
        }
    }
}
