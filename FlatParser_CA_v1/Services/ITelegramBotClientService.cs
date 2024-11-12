namespace FlatParser_CA_v1.Services
{
    public interface ITelegramBotClientService
    {
        public Task SendMessage(long chatId, string message);
    }
}
