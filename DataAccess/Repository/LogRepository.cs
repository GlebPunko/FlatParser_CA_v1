using DataAccess.Entity;
using DataAccess.Repository.Interface;

namespace DataAccess.Repository
{
    public class LogRepository(string connectionString) : MsSqlRepositoryBase(connectionString), ILogRepository
    {
        public async Task<bool> AddLog(Log log)
        {
            var sql = "INSERT INTO (Source, Message, StackTrace, DateAndTime) " +
                "VALUES (@Source, @Message, @StackTrace, @DateAndTime)";

            return await Database.SaveData(sql, new { log.Source, log.Message, log.StackTrace, log.DateAndTime });
        }
    }
}
