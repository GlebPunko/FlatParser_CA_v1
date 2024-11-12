using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Parsers.RealtParser.Interfaces
{
    public interface IRealtParser
    {
        public Task RunService();
        public HashSet<FlatInfo> GetFlats();
    }
}
