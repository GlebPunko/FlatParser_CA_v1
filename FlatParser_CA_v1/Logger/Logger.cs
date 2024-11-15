using DataAccess.Entity;
using DataAccess.Repository.Interface;
using FlatParser_CA_v1.Logger.Interface;

namespace FlatParser_CA_v1.Logger
{
    public class Logger : ILogger
    {
        private ILogRepository LogRepository { get; }

        public Logger(ILogRepository logRepository)
        {
            LogRepository = logRepository;
        }

        public async Task Log(Exception ex)
        {
            var log = new Log
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                DateAndTime = DateTime.Now.ToString()
            };

            await LogRepository.AddLog(log);
        }
    }
}
