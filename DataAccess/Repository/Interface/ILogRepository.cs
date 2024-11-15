using DataAccess.Entity;

namespace DataAccess.Repository.Interface
{
    public interface ILogRepository
    {
        Task<bool> AddLog(Log log);
    }
}
