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
            CategoryText.Text = (_item.Category ?? "Lançamento").ToUpper();
            
            // Set image source if valid, otherwise use a placeholder
            if (!string.IsNullOrEmpty(_item.ImageUrl))
            {
                if (_item.ImageUrl.StartsWith("http://") || _item.ImageUrl.StartsWith("https://"))
                {
                    DetailImage.Source = ImageSource.FromUri(new Uri(_item.ImageUrl));
                }
                else
                {
                    // For local resources, use the filename without extension
                    var imgName = _item.ImageUrl;
                    if (imgName.EndsWith(".png") || imgName.EndsWith(".jpg") || imgName.EndsWith(".jpeg"))
                    {
                        imgName = System.IO.Path.GetFileNameWithoutExtension(imgName);
                    }
                    DetailImage.Source = ImageSource.FromFile(imgName);
                }
            }
            else
            {
                DetailImage.Source = ImageSource.FromFile("dotnet_bot"); // Fallback placeholder
            }

            // Style category badge based on category type
            var categoryLower = (_item.Category ?? "").ToLower();
            CategoryBadge.BackgroundColor = categoryLower switch
            {
                "manga" or "mangás" or "mangá" => Color.FromArgb("#10B981"),      // Emerald green
                "série" or "serie" or "séries" => Color.FromArgb("#3B82F6"),      // Blue
                "ost" or "osts" => Color.FromArgb("#8B5CF6"),        // Purple
                "filme" or "filmes" => Color.FromArgb("#EF4444"),      // Red
                "ova" or "ovas" => Color.FromArgb("#F59E0B"),        // Amber
                _ => Application.Current != null && Application.Current.Resources.TryGetValue("Primary", out var colorVal) ? (Color)colorVal : Color.FromArgb("#3482FA")
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
