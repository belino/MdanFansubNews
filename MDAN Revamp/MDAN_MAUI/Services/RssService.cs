using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MDAN_MAUI.Models;

namespace MDAN_MAUI.Services
{
    public class RssService
    {
        private readonly HttpClient _httpClient;
        private const string RssUrl = "https://mdan.org/feed/";

        public RssService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }

        public async Task<List<RSSItem>> FetchFeedAsync(bool useMockAsFallback = true)
        {
            try
            {
                var content = await _httpClient.GetStringAsync(RssUrl);
                if (string.IsNullOrWhiteSpace(content) || content.Contains("Erro ao estabelecer uma conexão com o banco de dados"))
                {
                    if (useMockAsFallback)
                    {
                        return GetMockItems();
                    }
                    throw new Exception("Feed contains database connection error.");
                }

                var doc = XDocument.Parse(content);
                var items = doc.Descendants("item").Select(item =>
                {
                    var title = item.Element("title")?.Value.Trim() ?? "Sem Título";
                    
                    // Parse description and clean HTML tags
                    var rawDescription = item.Element("description")?.Value ?? string.Empty;
                    var cleanDescription = System.Net.WebUtility.HtmlDecode(
                        Regex.Replace(rawDescription, @"<[^>]+>|&nbsp;", "").Trim()
                    );
                    
                    // Clean carriage returns and extra spaces
                    cleanDescription = Regex.Replace(cleanDescription.Replace("\r", "").Replace("\n", " "), @"\s+", " ");

                    var link = item.Element("link")?.Value ?? string.Empty;
                    var pubDate = item.Element("pubDate")?.Value ?? string.Empty;
                    if (pubDate.Length > 22)
                    {
                        pubDate = pubDate.Substring(0, 22);
                    }

                    // Extract first image from description or content:encoded
                    var xmlContent = item.ToString();
                    var imageUrls = GetImagesInHtmlString(xmlContent);
                    var imageUrl = imageUrls.FirstOrDefault() ?? "https://picsum.photos/id/1025/600/400"; // Fallback image

                    // Category extraction
                    var category = item.Elements("category").FirstOrDefault()?.Value ?? DetectCategory(title);

                    return new RSSItem
                    {
                        Title = title,
                        Link = link,
                        Description = cleanDescription,
                        PubDate = pubDate,
                        ImageUrl = imageUrl,
                        Category = category
                    };
                }).ToList();

                if (!items.Any() && useMockAsFallback)
                {
                    return GetMockItems();
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching RSS: {ex.Message}");
                if (useMockAsFallback)
                {
                    return GetMockItems();
                }
                return new List<RSSItem>();
            }
        }

        private static string DetectCategory(string title)
        {
            title = title.ToLower();
            if (title.Contains("[manga]") || title.Contains("manga") || title.Contains("mangá")) return "Manga";
            if (title.Contains("[série]") || title.Contains("série") || title.Contains("serie")) return "Série";
            if (title.Contains("[ost]") || title.Contains("ost") || title.Contains("soundtrack")) return "OST";
            if (title.Contains("[filme]") || title.Contains("filme") || title.Contains("movie")) return "Filme";
            if (title.Contains("[ova]") || title.Contains("ova")) return "OVA";
            return "Lançamento";
        }

        private static List<string> GetImagesInHtmlString(string htmlString)
        {
            var images = new List<string>();
            if (string.IsNullOrEmpty(htmlString)) return images;

            // Search for standard img tags
            var imgRegex = new Regex(@"<img[^>]+src=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            var matches = imgRegex.Matches(htmlString);
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    images.Add(match.Groups[1].Value);
                }
            }

            // Search for links ending in image extensions
            var urlRegex = new Regex(@"https?://\S+\.(?:png|jpg|jpeg|gif|webp)", RegexOptions.IgnoreCase);
            var urlMatches = urlRegex.Matches(htmlString);
            foreach (Match match in urlMatches)
            {
                images.Add(match.Value);
            }

            return images;
        }

        public List<RSSItem> GetMockItems()
        {
            return new List<RSSItem>
            {
                new RSSItem
                {
                    Title = "[Manga] Solo Leveling - Capítulo 180 (Completo)",
                    Link = "https://mdan.org/solo-leveling-capitulo-180/",
                    Description = "O capítulo final do aclamado Webtoon Solo Leveling finalmente chegou. Acompanhe a conclusão da jornada épica de Sung Jin-Woo em sua batalha final contra os Monarcas para salvar a humanidade.",
                    PubDate = "Qui, 21 Mai 2026 18:00:00",
                    ImageUrl = "https://picsum.photos/id/1025/800/500",
                    Category = "Manga"
                },
                new RSSItem
                {
                    Title = "[Série] Fairy Tail: 100 Years Quest - Episódio 05",
                    Link = "https://mdan.org/fairy-tail-100-years-quest-episodio-05/",
                    Description = "Natsu, Lucy e toda a guilda continuam na lendária missão de 100 anos. O mistério se aprofunda após a revelação do primeiro dragão divino e o poder de Touka é revelado na guilda.",
                    PubDate = "Qui, 21 Mai 2026 17:30:00",
                    ImageUrl = "https://picsum.photos/id/1035/800/500",
                    Category = "Série"
                },
                new RSSItem
                {
                    Title = "[OST] Bleach: Thousand-Year Blood War OST Vol. 2",
                    Link = "https://mdan.org/bleach-tybw-ost-vol-2/",
                    Description = "A imperdível trilha sonora oficial da segunda parte da Guerra de Sangue dos Mil Anos de Bleach. Contém faixas icônicas compostas pelo renomado compositor Shiro Sagisu.",
                    PubDate = "Qui, 21 Mai 2026 16:45:00",
                    ImageUrl = "https://picsum.photos/id/1043/800/500",
                    Category = "OST"
                },
                new RSSItem
                {
                    Title = "[Filme] Demon Slayer: Mugen Train (BluRay 1080p)",
                    Link = "https://mdan.org/demon-slayer-mugen-train-bluray-1080p/",
                    Description = "O longa-metragem recordista de bilheteria que adapta o arco do Trem Infinito. Tanjiro e Nezuko se juntam a Rengoku, o Hashira das Chamas, para combater demônios em um trem em movimento.",
                    PubDate = "Qua, 20 Mai 2026 12:00:00",
                    ImageUrl = "https://picsum.photos/id/1062/800/500",
                    Category = "Filme"
                },
                new RSSItem
                {
                    Title = "[OVA] Attack on Titan: Lost Girls (Coleção Completa)",
                    Link = "https://mdan.org/attack-on-titan-lost-girls-ova-completo/",
                    Description = "Episódios especiais focados no passado misterioso de Annie Leonhart e no mundo idealizado de Mikasa Ackerman. Excelente animação pelo Wit Studio.",
                    PubDate = "Ter, 19 Mai 2026 10:15:00",
                    ImageUrl = "https://picsum.photos/id/1084/800/500",
                    Category = "OVA"
                }
            };
        }
    }
}
