using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Parsers.KufarParser.Interfaces
{
    public interface IKufarService
    {
        public Task RunService(long chatId);
        public HashSet<FlatInfo> GetFlats(string url);
    }
}
