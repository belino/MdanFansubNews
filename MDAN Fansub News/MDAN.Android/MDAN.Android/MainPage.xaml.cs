using MDAN.Base;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MDAN.Android
{
	public partial class MainPage : ContentPage
	{
        ObservableCollection<RSSItem> mainList = new ObservableCollection<RSSItem>();
        public MainPage()
		{
            GetContent();
			InitializeComponent();
		}

      

        private async void GetContent()
        {
            try
            {
                var siteContent = new RssContentRetriever();
                var result = await siteContent.GetSiteContent();
                mainList = new ObservableCollection<RSSItem>(result);
                listRss = new ListView
                {
                    ItemsSource = mainList
                };
                Console.WriteLine(listRss);
            }
            catch (Exception ex)
            {
                var message = string.Format("Ocorreu um erro durante o acesso ao site. Tente mais tarde. Você pode reportar o erro ao Detona XD. Erro: {0}", ex.Message);
                //var dialog = new MessageDialog(message) { Title = "Erro" };
                //dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                //await dialog.ShowAsync();
            }
        }

    }
}
