using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Services;
using FlatParser_CA_v1.Workers.Interfaces;

namespace FlatParser_CA_v1.Workers
{
    public class Worker : IWorker
    {
        private IKufarService KufarService { get; set; }
        private IRealtParser RealtParser { get; set; }

        public Worker(IKufarService kufarService, IRealtParser realtParser)
        {
            KufarService = kufarService;
            RealtParser = realtParser;
        }

        public async Task RunWorker(long chatId)
        {
            try
            {
                Task realtTask = Task.Run(() => RealtParser.RunService(chatId));
                Task kufarTask = Task.Run(() => KufarService.RunService(chatId));

                await Task.WhenAll(realtTask, kufarTask);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error in Worker: {ex.Message}");
                throw;
            }
        }

    }
}
