using Microsoft.Extensions.DependencyInjection;
using FlatParser_CA_v1.Workers;
using Microsoft.Extensions.Hosting;
using FlatParser_CA_v1.Services;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Parsers.KufarParser.Services;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Parsers.RealtParser.Services;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    _ = services.GetRequiredService<Worker>().RunWorker(-4527978350);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

Console.WriteLine("Bot startded succesfully!");
Console.ReadKey();

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddScoped<IKufarService, KufarService>();
            services.AddScoped<IRealtParser, RealtParser>();
            services.AddSingleton<ITelegramBotClientService, TelegramBotClientService>();
            services.AddSingleton<Worker>();
        });
}