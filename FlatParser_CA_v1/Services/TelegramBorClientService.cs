using FlatParser_CA_v1.Models;
using Telegram.Bot;

namespace FlatParser_CA_v1.Services
{
    public class TelegramBotClientService : ITelegramBotClientService
    {
        public ITelegramBotClient Bot { get; }
        private Config ConfigSettings { get; }

        public TelegramBotClientService(Config config)
        {
            ConfigSettings = config;
            Bot = new TelegramBotClient(ConfigSettings.AccessToken);
        }

        public async Task SendMessage(long chatId, string message)
        {
            await Bot.SendMessage(chatId == default ? ConfigSettings.ChatId : chatId, message);
        }
    }
}
