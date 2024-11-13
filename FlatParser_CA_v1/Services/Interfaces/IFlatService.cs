using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Services.Interfaces
{
    public interface IFlatService
    {
        public Task<IEnumerable<FlatInfo>> GetFlats(CancellationToken cancellationToken);
        public Task<bool> AddFlat(FlatInfo entity);
        Task<bool> CheckFlatExists(string address, string link, CancellationToken cancellationToken = default);
    }
}
