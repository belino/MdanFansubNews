using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MDAN_MAUI.Models;
using MDAN_MAUI.Services;

namespace MDAN_MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly RssService _rssService;
        private List<RSSItem> _allItems = new List<RSSItem>();
        private List<RSSItem> _displayedItems = new List<RSSItem>();
        private string _selectedCategory = "Todos";
        private string _searchQuery = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            
            // Resolve RssService from Dependency Injection
            _rssService = Handler?.MauiContext?.Services.GetService<RssService>() ?? new RssService();
            
            FeedRefreshView.Command = new Command(async () => await LoadFeedAsync());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Fetch the feed if it hasn't been loaded yet
            if (!_allItems.Any())
            {
                FeedRefreshView.IsRefreshing = true;
            }
        }

        private async Task LoadFeedAsync()
        {
            try
            {
                var items = await _rssService.FetchFeedAsync(useMockAsFallback: true);
                _allItems = items;

                // Bind Featured Items to Carousel (top 3)
                var featured = items.Take(3).ToList();
                FeaturedCarousel.ItemsSource = featured;
                CarouselIndicator.IndicatorSize = featured.Count;
                CarouselIndicator.BindingContext = FeaturedCarousel;

                // Apply current filters
                FilterAndDisplayFeed();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load feed in UI: {ex.Message}");
                await DisplayAlert("Erro", "Falha ao carregar novos lançamentos.", "OK");
            }
            finally
            {
                FeedRefreshView.IsRefreshing = false;
            }
        }

        private void FilterAndDisplayFeed()
        {
            // First, filter by category
            IEnumerable<RSSItem> filtered = _allItems;
            
            if (_selectedCategory != "Todos")
            {
                filtered = filtered.Where(x => string.Equals(x.Category, _selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // Then, filter by search query
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                filtered = filtered.Where(x => 
                    x.Title.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) || 
                    x.Description.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Update displayed items
            _displayedItems = filtered.ToList();
            FeedCollectionView.ItemsSource = _displayedItems;
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            FeedRefreshView.IsRefreshing = true;
        }

        private void OnFeaturedPositionChanged(object sender, PositionChangedEventArgs e)
        {
            // Indicator updates automatically via binding
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _searchQuery = e.NewTextValue ?? string.Empty;
            FilterAndDisplayFeed();
        }

        private void OnFilterChipClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string category = button.CommandParameter as string ?? "Todos";
            _selectedCategory = category;

            // Reset all chips styles
            ResetAllChips();

            // Set selected style to the clicked chip
            button.BackgroundColor = (Color)Application.Current.Resources["Primary"];
            button.TextColor = Colors.White;

            FilterAndDisplayFeed();
        }

        private void ResetAllChips()
        {
            var defaultBg = AppInfo.RequestedTheme == AppTheme.Dark 
                ? (Color)Application.Current.Resources["Gray900"] 
                : (Color)Application.Current.Resources["Gray200"];
                
            var defaultText = AppInfo.RequestedTheme == AppTheme.Dark 
                ? Colors.White 
                : Colors.Black;

            var chips = new List<Button> { ChipAll, ChipManga, ChipSerie, ChipOst, ChipFilme, ChipOva };
            foreach (var chip in chips)
            {
                chip.BackgroundColor = defaultBg;
                chip.TextColor = defaultText;
            }
        }

        private async void OnFeaturedTapped(object sender, TappedEventArgs e)
        {
            var item = e.Parameter as RSSItem;
            if (item != null)
            {
                await Navigation.PushAsync(new DetailsPage(item));
            }
        }

        private async void OnFeedItemTapped(object sender, TappedEventArgs e)
        {
            var item = e.Parameter as RSSItem;
            if (item != null)
            {
                await Navigation.PushAsync(new DetailsPage(item));
            }
        }
    }
}
