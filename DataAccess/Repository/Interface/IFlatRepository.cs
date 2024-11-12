using DataAccess.Entity;

namespace DataAccess.Repository.Interface
{
    public interface IFlatRepository
    {
        Task<IEnumerable<FlatEntity>> GetFlats(CancellationToken cancellationToken = default);
        Task<bool> AddFlat(FlatEntity flat);
    }
}
