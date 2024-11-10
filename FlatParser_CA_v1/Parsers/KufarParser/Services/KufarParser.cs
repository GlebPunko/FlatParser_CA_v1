using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Services;
using HtmlAgilityPack;

namespace FlatParser_CA_v1.Parsers.KufarParser.Services
{
    public class KufarParser : IKufarParser
    {
        private HashSet<FlatInfo> lastElementsList = new();
        private HashSet<FlatInfo> differenceItems = new();

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

                        differenceItems = FindNotMatchElements(flatLinks);

                        if (differenceItems.Count == 0 || differenceItems is null || differenceItems.Count > 25)
                            continue;

                        break;
                    }


                    foreach (var item in differenceItems)
                    {
                        Console.WriteLine($"Link: {item} \nTime: {DateTime.UtcNow}");
                        await BotClientService.SendMessage(ConfigSettings.ChatId, $"Link: {item.Link}\n" +
                            $"Address: {item.Address}\nPrice: {item.Price}");
                    }

                    differenceItems.Clear();
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

                HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//section/a");
                HtmlNodeCollection pricesTemp = doc.DocumentNode.SelectNodes("//section/a/div/div/div/span");
                HtmlNodeCollection addresses = doc.DocumentNode.SelectNodes("//section/a/div/div/div/div/span");

                HtmlNodeCollection prices = new(null);

                if (links is null || pricesTemp is null || addresses is null)
                    return flatLinks;

                for (int i = 0; i < pricesTemp.Count; i++)
                {
                    if (pricesTemp[i].InnerHtml.Contains("р.") || pricesTemp[i].InnerHtml.Contains("Договорная"))
                        prices.Add(pricesTemp[i]);
                    else
                        continue;
                }

                for (int i = 0; i < links.Count; i++)
                {
                    string href = links[i].Attributes["href"].Value;
                    int lastIndex = href.LastIndexOf('?');

                    var res = href.Remove(lastIndex);

                    var flatInfo = new FlatInfo()
                    {
                        Link = res,
                        Price = prices[i].InnerHtml,
                        Address = addresses[i].InnerHtml
                    };

                    flatLinks.Add(flatInfo);
                }

                return flatLinks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return flatLinks;
            }
        }

        private HashSet<FlatInfo> FindNotMatchElements(HashSet<FlatInfo> bookLinks)
        {
            HashSet<FlatInfo> temp = new();

            foreach (var item in bookLinks)
            {
                if (!lastElementsList.Contains(item))
                {
                    lastElementsList.Add(item);

                    temp.Add(item);
                }
            }

            return temp;
        }

        private HtmlDocument GetDocument()
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(ConfigSettings.KufarAddress);

            return doc;
        }
    }
}
