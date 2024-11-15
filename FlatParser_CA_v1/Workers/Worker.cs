using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Parsers.KufarParser;
using FlatParser_CA_v1.Parsers.KufarParser.Interfaces;
using FlatParser_CA_v1.Parsers.RealtParser.Interfaces;
using FlatParser_CA_v1.Workers.Interfaces;

namespace FlatParser_CA_v1.Workers
{
    public class Worker : IWorker
    {
        private IKufarParser KufarService { get; }
        private IRealtParser RealtParser { get; }

        public Worker(IKufarParser kufarService, IRealtParser realtParser)
        {
            KufarService = kufarService;
            RealtParser = realtParser;
        }

        public async Task RunWorker()
        {
            try
            {
                Task kufarTask = Task.Run(() => KufarService.RunService());

                await Task.WhenAll(kufarTask);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error in Worker: {ex.Message}");
                throw;
            }
        }

    }
}
