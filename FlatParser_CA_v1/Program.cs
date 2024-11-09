using FlatParser_CA_v1.Handlers;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using FlatParser_CA_v1.KufarParser.Interfaces;
using FlatParser_CA_v1.KufarParser.Services;
using FlatParser_CA_v1.Workers.Interfaces;
using FlatParser_CA_v1.Workers;
using FlatParser_CA_v1.Helper;
using Microsoft.Extensions.Hosting;

TelegramBotClient botClient = new("7886215570:AAEQXW35aZahmJ8Jl13LNPVVrnnutEsFQyY");

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

botClient.StartReceiving(Handlers.UpdateHandler, Handlers.ErrorHandler);

try
{
    services.GetRequiredService<Worker>().RunWorker();
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
            services.AddSingleton<Worker>();
        });
}