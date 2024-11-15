using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Services.Interfaces;
using HtmlAgilityPack;

namespace FlatParser_CA_v1.Parsers.RealtParser
{
    public class RealtParser : IRealtParser
    {
        private HashSet<FlatInfo> oldFlats = new();
        private HashSet<FlatInfo> newFindedFlats = new();

        private const string flatUrl = "https://realt.by";

        private StoredConfigs ConfigSettings { get; }
        private ITelegramBotClientService BotClientService { get; }

        public RealtParser(ITelegramBotClientService botClientService, StoredConfigs configSettings)
        {
            BotClientService = botClientService;
            ConfigSettings = configSettings;
        }

        public HashSet<FlatInfo> GetFlats()
        {
            var flatLinks = new HashSet<FlatInfo>();

            try
            {
                HtmlDocument doc = GetDocument();

                if (doc is null)
                    return flatLinks;

                var links = doc.DocumentNode.SelectNodes("//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Where(x => x.Contains("object"))
                    .ToList();

                var adresses = doc.DocumentNode.SelectNodes("//p[@class='text-basic w-full text-subhead md:text-body']")
                    .Select(x => x.InnerText)
                    .ToList();

                var pricesTemp = doc.DocumentNode.SelectNodes("//span[@class='text-basic text-subhead']")
                    .Select(x => x.InnerText)
                    .ToList();

                if (links is null || pricesTemp is null || adresses is null)
                    return flatLinks;

                for (int i = 0; i < links.Count; i++)
                {
                    var flatInfo = new FlatInfo()
                    {
                        Link = flatUrl + links[i],
                        Price = pricesTemp[i],
                        Address = adresses[i],
                    };

                    flatLinks.Add(flatInfo);
                }

                return flatLinks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return flatLinks;
            }
        }

        public async Task RunService()
        {
            Console.WriteLine("RealtParser started.");

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

                        if (newFindedFlats.Count == 0 || newFindedFlats is null || newFindedFlats.Count > 10)
                        {
                            newFindedFlats.Clear();
                            continue;
                        }

                        break;
                    }

                    foreach (var item in newFindedFlats)
                    {
                        Console.WriteLine($"Link: {item.Link} \nTime: {DateTime.UtcNow}");
                        await BotClientService.SendMessage(ConfigSettings.Config.ChatId, $"Link: {item.Link}\n" +
                            $"Address: {item.Address}\nPrice: {item.Price}");
                    }

                    newFindedFlats.Clear();
                }
                catch (Exception ex)
                {
                    await BotClientService.SendMessage(ConfigSettings.Config.ChatId, $"#unknown \nSource Name: {ex.Source} \n" +
                        $"Message: \n{ex.Message} \nStack Trace: \n{ex.StackTrace} \n");

                    continue;
                }
            }
        }

        private void FindNotMatchElements(HashSet<FlatInfo> bookLinks)
        {
            foreach (var item in bookLinks)
            {
                if (!oldFlats.Contains(item))
                {
                    oldFlats.Add(item);

                    newFindedFlats.Add(item);
                }
            }
        }

        private HtmlDocument GetDocument()
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(ConfigSettings.Config.RealtAddress);

            return doc;
        }
    }
}
