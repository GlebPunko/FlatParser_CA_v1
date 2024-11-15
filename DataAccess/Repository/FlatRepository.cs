using DataAccess.Entity;
using DataAccess.Repository.Interface;

namespace DataAccess.Repository
{
    public class FlatRepository(string connectionString) : MsSqlRepositoryBase(connectionString), IFlatRepository
    {
        public async Task<IEnumerable<FlatEntity>> GetFlats(CancellationToken cancellationToken = default)
        {
            var sql = "SELECT * FROM [dbo].parsed_flats;";

            return await Database.LoadData<FlatEntity>(sql, parameters: null, cancellationToken);
        }

        //Here need to do something with logic (future)
        public async Task<bool> CheckFlatExists(string address, string link, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM [dbo].parsed_flats WHERE Address = @address AND Link = @link) " +
                "THEN 1 ELSE 0 END;";

            return await Database.LoadDataSingle<bool>(sql, new {address, link}, cancellationToken);
        }

        public async Task<bool> AddFlat(FlatEntity flat)
        {
            var sql = "INSERT INTO [dbo].parsed_flats (Link, Address, Price, RegionId) " +
                "VALUES (@Link, @Address, @Price, @RegionId)";

            return await Database.SaveData(sql, new {flat.Link, flat.Address, flat.Price, flat.RegionId});
        }
    }
}
