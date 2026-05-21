using System;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using MDAN_MAUI.Models;

namespace MDAN_MAUI
{
    public partial class DetailsPage : ContentPage
    {
        private readonly RSSItem _item;

        public DetailsPage(RSSItem item)
        {
            InitializeComponent();
            _item = item;
            PopulateData();
        }

        private void PopulateData()
        {
            TitleText.Text = _item.Title;
            DateText.Text = _item.PubDate;
            DescriptionText.Text = _item.Description;
            CategoryText.Text = _item.Category.ToUpper();
            
            // Set image source if valid, otherwise use a placeholder
            if (!string.IsNullOrEmpty(_item.ImageUrl))
            {
                DetailImage.Source = ImageSource.FromUri(new Uri(_item.ImageUrl));
            }
            else
            {
                DetailImage.Source = "dotnet_bot.png"; // Fallback placeholder
            }

            // Style category badge based on category type
            CategoryBadge.BackgroundColor = _item.Category.ToLower() switch
            {
                "manga" => Color.FromArgb("#10B981"),      // Emerald green
                "série" => Color.FromArgb("#3B82F6"),      // Blue
                "ost" => Color.FromArgb("#8B5CF6"),        // Purple
                "filme" => Color.FromArgb("#EF4444"),      // Red
                "ova" => Color.FromArgb("#F59E0B"),        // Amber
                _ => (Color)Application.Current.Resources["Primary"]
            };
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnOpenBrowserClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_item.Link)) return;

            try
            {
                var uri = new Uri(_item.Link);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open browser: {ex.Message}");
                await DisplayAlert("Erro", "Não foi possível abrir o link.", "OK");
            }
        }
    }
}
