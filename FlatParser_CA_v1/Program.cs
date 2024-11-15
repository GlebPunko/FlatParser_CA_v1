using DataAccess.Repository;
using DataAccess.Repository.Interface;
using FlatParser_CA_v1.Helpers;
using FlatParser_CA_v1.Logger;
using FlatParser_CA_v1.Logger.Interface;
using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
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

            string connectionString = null;

            services.AddSingleton<StoredConfigs>(provider =>
            {
                var configReader = new ConfigsReader("config.json", "cursorBrest.json");
                
                var config =  configReader.ReadConfig();

                connectionString = config.Config.ConnectionString;

                return config;
            });
            
            services.AddScoped<IKufarParser, KufarParser>();
            services.AddScoped<IRealtParser, RealtParser>();

            services.AddScoped<IFlatRepository>(_ => new FlatRepository(connectionString));
            services.AddScoped<ILogRepository>(_ => new LogRepository(connectionString));

            services.AddSingleton<ITelegramBotClientService, TelegramBotClientService>();
            services.AddSingleton<IFlatService, FlatService>();
            services.AddSingleton<ILogger, Logger>();

            services.AddSingleton<Worker>();
        });
}