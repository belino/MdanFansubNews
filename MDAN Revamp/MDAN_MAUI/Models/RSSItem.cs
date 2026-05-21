using System;

namespace MDAN_MAUI.Models
{
    public class RSSItem
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PubDate { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = "Notícia"; // Default category
    }
}
