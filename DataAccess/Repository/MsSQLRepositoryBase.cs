using Database;

namespace DataAccess.Repository
{
    public abstract class MsSqlRepositoryBase
    {
        protected IDb Database { get; }

        public MsSqlRepositoryBase(string connectionString)
        {
            Database = new Database.MsSql(connectionString);
        }
    }
}
