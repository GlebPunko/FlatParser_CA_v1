using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Parsers.KufarParser.Interfaces
{
    public interface IKufarParser
    {
        public Task RunService();
        public HashSet<FlatInfo> GetFlats();
    }
}
