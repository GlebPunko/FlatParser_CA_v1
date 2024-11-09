using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Parsers.RealtParser.Interfaces
{
    public interface IRealtParser
    {
        public Task RunService(long chatId);
        public HashSet<FlatInfo> GetFlats(string url);
    }
}
