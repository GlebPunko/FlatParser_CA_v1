namespace FlatParser_CA_v1.Services.Interfaces
{
    public interface ITelegramBotClientService
    {
        public Task SendMessage(long chatId, string message);
    }
}
