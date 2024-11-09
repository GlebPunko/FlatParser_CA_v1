using FlatParser_CA_v1.KufarParser.Interfaces;
using FlatParser_CA_v1.Workers.Interfaces;
using Telegram.Bot;

namespace FlatParser_CA_v1.Workers
{
    public class Worker : IWorker
    {
        private IKufarService KufarService { get; set; }

        public Worker(IKufarService kufarService)
        {
            KufarService = kufarService;
        }

        public async Task RunWorker()
        {
            try
            {
                await KufarService.RunService(12);
                await Console.Out.WriteLineAsync("ww");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
        }

    }
}
