using DataAccess.Repository;
using DataAccess.Repository.Interface;
using FlatParser_CA_v1.Helpers;
using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Parsers.KufarParser.Services;
using FlatParser_CA_v1.Parsers.RealtParser;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Services;
using FlatParser_CA_v1.Services.Interfaces;
using FlatParser_CA_v1.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    _ = services.GetRequiredService<Worker>().RunWorker();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

Console.WriteLine("Bot startded succesfully!");
Console.ReadKey();

static IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<StoredConfigs>(provider =>
            {
                var configReader = new ConfigsReader("config.json", "cursorBrest.json");
                
                return configReader.ReadConfig();
            });
            
            services.AddScoped<IKufarParser, KufarParser>();
            services.AddScoped<IRealtParser, RealtParser>();

            services.AddScoped<IFlatRepository>(_ => 
                new FlatRepository("Server=localhost;Database=FlatDb;Trusted_Connection=True;MultipleActiveResultSets=true"));//need to fix

            services.AddSingleton<ITelegramBotClientService, TelegramBotClientService>();
            services.AddSingleton<IFlatService, FlatService>();

            services.AddSingleton<Worker>();
        });
}