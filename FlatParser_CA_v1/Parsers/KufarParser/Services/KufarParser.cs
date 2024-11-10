using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Services;
using HtmlAgilityPack;

namespace FlatParser_CA_v1.Parsers.KufarParser.Services
{
    public class KufarParser : IKufarParser
    {
        private HashSet<FlatInfo> oldFlats = new();
        private HashSet<FlatInfo> newFindedFlats = new();

        private Config ConfigSettings { get; }
        private ITelegramBotClientService BotClientService { get; }

        public KufarParser(ITelegramBotClientService botClientService, Config configSettings)
        {
            BotClientService = botClientService;
            ConfigSettings = configSettings;
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
                            newFindedFlats.Clear();
                            continue;
                        }
                            
                        break;
                    }


                    foreach (var item in newFindedFlats)
                    {
                        Console.WriteLine($"Link: {item.Link} \nTime: {DateTime.UtcNow}");
                        await BotClientService.SendMessage(ConfigSettings.ChatId, $"Link: {item.Link}\n" +
                            $"Address: {item.Address}\nPrice: {item.Price}");
                    }

                    newFindedFlats.Clear();
                }
                catch (Exception ex)
                {
                    await BotClientService.SendMessage(ConfigSettings.ChatId, $"#unknown \nSource Name: {ex.Source} \n" +
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
                HtmlDocument doc = GetDocument();

                if (doc is null)
                    return flatLinks;

                var links = doc.DocumentNode.SelectNodes("//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Where(x => x.Contains("vi/brest/snyat/kvartiru-dolgosrochno"))
                    .ToList();

                var prices = doc.DocumentNode.SelectNodes("//span[@class='styles_price__byr__lLSfd']")
                    .Select(x => x.InnerHtml)
                    .ToList();

                var addresses = doc.DocumentNode.SelectNodes("//span[@class='styles_address__l6Qe_']")
                    .Select(x => x.InnerHtml)
                    .ToList();

                if (links is null || prices is null || addresses is null)
                    return flatLinks;

                for (int i = 0; i < links.Count; i++)
                {
                    var flatInfo = new FlatInfo()
                    {
                        Link = links[i][..links[i].IndexOf("?")],
                        Price = prices[i],
                        Address = addresses[i]
                    };

                    flatLinks.Add(flatInfo);
                }

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
            HtmlDocument doc = web.Load(ConfigSettings.KufarAddress);

            return doc;
        }
    }
}
