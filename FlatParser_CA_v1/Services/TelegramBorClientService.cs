using Telegram.Bot;
using FlatParser_CA_v1.Handlers;

namespace FlatParser_CA_v1.Services
{
    public class TelegramBotClientService : ITelegramBotClientService
    {
        public ITelegramBotClient Bot { get; }

        public TelegramBotClientService()
        {
            Bot = new TelegramBotClient("7886215570:AAEQXW35aZahmJ8Jl13LNPVVrnnutEsFQyY");
        }

        public async Task SendMessage(long chatId, string message)
        {
            Console.WriteLine("Send message method.");
            await Bot.SendMessage(chatId, message);
        }
    }
}
