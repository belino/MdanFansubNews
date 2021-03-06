﻿using System;
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
        private HTMLData HtmlContent;
        public Site()
        {
            this.InitializeComponent();
            HtmlContent = new HTMLData();
            var back = "Windows.Phone.UI.Input.HardwareButtons";
            if (!ApiInformation.IsTypePresent(back))
            {
                listRss.FlowDirection = FlowDirection.LeftToRight;
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
        {
            if (ContentGrid.Visibility == Visibility.Visible)
            {
                if (!SiteNewsContent.CanGoBack) return;
                SiteNewsContent.GoBack();
                a.Handled = true;
            }
            else
            {
                if (!Frame.CanGoBack) return;
                Frame.GoBack();
                a.Handled = true;
            }
        };

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, a) =>
                {
                    if (ContentGrid.Visibility == Visibility.Visible)
                    {
                        if (!SiteNewsContent.CanGoBack) return;
                        SiteNewsContent.GoBack();
                        a.Handled = true;
                    }
                    else
                    {
                        if (!Frame.CanGoBack) return;
                        Frame.GoBack();
                        a.Handled = true;
                    }
                };
            }
        }

        private async void WriteJSONAsync(string deb)
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
            try
            {
                var siteContent = new RssContentRetriever();
                mainList = await siteContent.GetSiteContent();
                listRss.ItemsSource = mainList;
                WriteJSONAsync(mainList.First().Title);
                ApplicationData.Current.LocalSettings.Values["LastUp"] = mainList.First().Title;
                if (ContentGrid.Visibility == Visibility.Visible)
                {
                    var htmlString = $"<style>.content {{max - width: 500px;margin: auto;}}</ style >< body >< div class='content'> {mainList[0].NewsContent}</div></body>";
                    mainList[0].NewsContent = $"<html><body>{mainList[0].NewsContent}</body></html>";
                    HtmlContent.HTML = mainList[0].NewsContent;

                    SiteNewsContent.Navigate(typeof(SiteNewsContent), mainList.First());
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("Ocorreu um erro durante o acesso ao site. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", ex.Message);
                var dialog = new MessageDialog(message) { Title = "Erro" };
                dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                await dialog.ShowAsync();
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int index = listRss.SelectedIndex;

            if (ContentGrid.Visibility == Visibility.Visible)
                SiteNewsContent.Navigate(typeof(SiteNewsContent), mainList[index]);
            else
                Frame.Navigate(typeof(SiteNewsContent), mainList[index]);
        }
    }
}

