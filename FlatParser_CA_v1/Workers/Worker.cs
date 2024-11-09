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

        public async Task RunWorker()
        {
            try
            {
                await RealtParser.RunService(12);
                //await KufarService.RunService(12);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
                throw;
            }
        }

    }
}
