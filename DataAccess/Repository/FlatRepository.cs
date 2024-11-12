using DataAccess.Entity;
using DataAccess.Repository.Interface;

namespace DataAccess.Repository
{
    public class FlatRepository(string connectionString) : MsSqlRepositoryBase(connectionString), IFlatRepository
    {
        public async Task<IEnumerable<FlatEntity>> GetFlats(CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM [dbo].Flats;";

            return await Database.LoadData<FlatEntity>(sql, parameters: null, cancellationToken);
        }

        //Here need to do something with logic (future)
        public async Task<bool> CheckFlatExists(string address, string link, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT EXISTS(SELECT 1 FROM [dbo].Flats WHERE Address = @address AND Link = @link";

            return await Database.LoadDataSingle<bool>(sql, new {address, link}, cancellationToken);
        }

        public async Task<bool> AddFlat(FlatEntity flat)
        {
            var sql = "INSERT INTO [dbo].Flats (Link, Address, Price) VALUES (@Link, @Address, @Price)";

            return await Database.SaveData(sql, new {flat.Link, flat.Address, flat.Price});
        }
    }
}
