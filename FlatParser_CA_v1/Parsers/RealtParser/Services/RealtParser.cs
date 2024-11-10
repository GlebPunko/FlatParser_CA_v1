using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Services;
using HtmlAgilityPack;

namespace FlatParser_CA_v1.Parsers.RealtParser.Services
{
    public class RealtParser : IRealtParser
    {
        private HashSet<FlatInfo> lastElementsList = new();
        private HashSet<FlatInfo> differenceItems = new();
        private const string flatUrl = "https://realt.by";

        private ITelegramBotClientService BotClientService { get; }

        public RealtParser(ITelegramBotClientService botClientService)
        {
            BotClientService = botClientService;
        }

        public HashSet<FlatInfo> GetFlats(string url)
        {
            var flatLinks = new HashSet<FlatInfo>();

            try
            {
                HtmlDocument doc = GetDocument(url);

                if (doc is null)
                    return flatLinks;

                var links = doc.DocumentNode.SelectNodes("//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Where(x => x.Contains("object"))
                    .ToList();

                var adresses = doc.DocumentNode.SelectNodes("//p[@class='text-basic w-full text-subhead md:text-body']")
                    .Select(x => x.InnerText)
                    .ToList();

                var pricesTemp = doc.DocumentNode.SelectNodes("//span[@class='text-title font-semibold text-info-500']")
                    .Select(x => x.InnerText)
                    .ToList();

                //class="text-basic w-full text-subhead md:text-body" address
                ///brest-region/rent-flat-for-long/object/3476081/ link
                //class="text-title font-semibold text-info-500 price

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

                return flatLinks;
            }
        }

        public async Task RunService(long chatId)
        {
            Console.WriteLine("RealtParser started.");

            while (true)
            {
                try
                {

                    while (true)
                    {
                        var flatLinks = GetFlats("https://realt.by/brest-region/rent/flat-for-long/?sortType=createdAt");

                        if (flatLinks is null || flatLinks.Count == 0)
                            continue;

                        differenceItems = FindNotMatchElements(flatLinks);

                        if (differenceItems.Count == 0 || differenceItems is null || differenceItems.Count > 10)
                            continue;

                        break;
                    }

                    Console.WriteLine("start send message: " + DateTime.Now);

                    foreach (var item in differenceItems)
                    {
                        Console.WriteLine($"Link: {item} \nTime: {DateTime.UtcNow}");
                        await BotClientService.SendMessage(chatId, $"Link: {item.Link}\nAddress: {item.Address}\nPrice: {item.Price}");
                    }

                    differenceItems.Clear();

                    Console.WriteLine("end send message: " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    await BotClientService.SendMessage(chatId, $"#unknown \nSource Name: {ex.Source} \n" +
                        $"Message: \n{ex.Message} \nStack Trace: \n{ex.StackTrace} \n");

                    continue;
                }
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

        private HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(url);

            return doc;
        }
    }
}
