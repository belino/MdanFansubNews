using MDAN.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDAN.Android
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Site : ContentPage
	{

        ObservableCollection<RSSItem> mainList = new ObservableCollection<RSSItem>();
        private const string JSONFILENAME = "data.json";

        public Site() => InitializeComponent();


        
        private void Grid_Tapped(object sender, EventArgs e)
        {
            //int index = listRss.;
            //NewsTitle.Text = mainList[index].Title;
            //NewsPubDate.Text = mainList[index].PubDate;
            //mainList[index].NewsContent = $"<html><body>{mainList[index].NewsContent}</body></html>";
            //HtmlContent.HTML = mainList[index].NewsContent;

            //NewsTitle.Text = mainList[index].Title;
            //SetNavigationPaneVisibility();
            //SetBackButtonVisibility();
            //SiteNewsContent.Navigate(typeof(SiteNewsContent), mainList[index].NewsContent);
        }



        //private void SetNavigationPaneVisibility()
        //{
        //    if (Window.Current.Bounds.Width < 1045)
        //        SiteContent.IsPaneOpen = false;
        //    else
        //    {
        //        SiteContent.IsPaneOpen = true;
        //    }
        //}

        
    }
}