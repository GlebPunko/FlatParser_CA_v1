using Database;

namespace DataAccess.Repository
{
    public abstract class MsSqlRepositoryBase(string connectionString)
    {
        protected IDb Database { get; } = new MsSql(connectionString);
    }
}
