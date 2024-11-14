using ConcurrentCollections;
using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Services.Interfaces;
using HtmlAgilityPack;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FlatParser_CA_v1.Parsers.KufarParser
{
    public class KufarParser : IKufarParser
    {
        private ConcurrentHashSet<FlatInfo> newFindedFlats = new();
        private List<HtmlDocument> HtmlDocs = new();

        private StoredConfigs ConfigSettingsInfo { get; }
        private ITelegramBotClientService BotClientService { get; }
        private IFlatService FlatService { get; }

        public KufarParser(ITelegramBotClientService botClientService, IFlatService flatService, StoredConfigs configSettings)
        {
            BotClientService = botClientService;
            ConfigSettingsInfo = configSettings;
            FlatService = flatService;
        }

        public async Task RunService()
        {
            Console.WriteLine("KufarParser started.");

            while (true)
            {
                try
                {
                    while (true)
                    {
                        var flatLinks = GetFlats();

                        if (flatLinks is null || flatLinks.Count == 0)
                            continue;

                        FindNotMatchElements(flatLinks);
                        
                        if (newFindedFlats.Count == 0 || newFindedFlats is null || newFindedFlats.Count > 25)
                        {
                            if(newFindedFlats.Count > 25)
                                SaveFlats().Wait();

                            newFindedFlats.Clear();

                            continue;
                        }

                        break;
                    }

                    SaveFlats().Wait();

                    foreach (var item in newFindedFlats)
                    {
                        Console.WriteLine($"Link: {item.Link} \nTime: {DateTime.UtcNow}");
                        await BotClientService.SendMessage(ConfigSettingsInfo.Config.ChatId, $"Link: {item.Link}\n" +
                            $"Address: {item.Address}\nPrice: {item.Price}");
                    }

                    newFindedFlats.Clear();
                }
                catch (Exception ex)
                {
                    await BotClientService.SendMessage(ConfigSettingsInfo.Config.ChatId, $"#unknown \nSource Name: {ex.Source} \n" +
                        $"Message: \n{ex.Message} \nStack Trace: \n{ex.StackTrace} \n");

                    continue;
                }
            }
        }

        public HashSet<FlatInfo> GetFlats()
        {
            var flatLinks = new HashSet<FlatInfo>();

            try
            {
                GetDocument();

                for (int i = 0; i < HtmlDocs.Count; i++)
                {
                    if (HtmlDocs[i] is null)
                        return flatLinks;

                    var links = HtmlDocs[i].DocumentNode.SelectNodes("//a")
                        .Select(x => x.Attributes["href"].Value)
                        .Where(x => x.Contains("vi/brest/snyat/kvartiru-dolgosrochno"))
                        .ToList();

                    var prices = HtmlDocs[i].DocumentNode.SelectNodes("//span[@class='styles_price__byr__lLSfd']")
                        .Select(x => x.InnerHtml)
                        .ToList();

                    var addresses = HtmlDocs[i].DocumentNode.SelectNodes("//span[@class='styles_address__l6Qe_']")
                        .Select(x => x.InnerHtml)
                        .ToList();

                    if (links is null || prices is null || addresses is null)
                    {
                        HtmlDocs.Clear();

                        return flatLinks;
                    }

                    for (int j = 0; j < links.Count; j++)
                    {
                        var flatInfo = new FlatInfo()
                        {
                            Link = links[j][..links[j].IndexOf("?")],
                            Price = prices[j],
                            Address = addresses[j]
                        };

                        flatLinks.Add(flatInfo);
                    }
                }

                HtmlDocs.Clear();

                return flatLinks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.InnerException);

                return flatLinks;
            }
        }

        private async void FindNotMatchElements(HashSet<FlatInfo> bookLinks)
        {
            foreach (var item in bookLinks)
            {
                var isExists = await FlatService.CheckFlatExists(item.Address, item.Link);

                if (!isExists)
                    newFindedFlats.Add(item);
            }
        }

        private async void GetDocument()
        {
            HtmlWeb web = new();
            int page = 1;

            while (true)
            {
                var changedUrl = await AddCursorsToLink(ConfigSettingsInfo.Config.KufarAddress, page);

                var html = web.Load(changedUrl);

                var str = html.DocumentNode.SelectNodes("//span[@class='styles_link__8m3I9 styles_active__GRR1D']")
                    .Select(x => x.InnerHtml);

                if (str.Contains("1") && HtmlDocs.Count > 1)
                    break;

                HtmlDocs.Add(html);
                page++;
            }
        }

        private async Task<bool> SaveFlats()
        {
            foreach (var item in newFindedFlats)
            {
                var success = await FlatService.AddFlat(item);

                if (!success)
                    Console.WriteLine($"flat was not saved: {item.Address}");
            }

            return true;
        }

        private async Task<string> AddCursorsToLink(string url, int page)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            ConfigSettingsInfo.BrestCursor.P = page;

            var json = JsonSerializer.Serialize(ConfigSettingsInfo.BrestCursor, options);
            var jsonBytes = Encoding.ASCII.GetBytes(json);

            var cursor = Convert.ToBase64String(jsonBytes);

            string pattern = @"cursor=(.*?)(&|$)";

            string newUrl = Regex.Replace(url, pattern, $"cursor={cursor}&");

            return newUrl;
        }
    }
}
