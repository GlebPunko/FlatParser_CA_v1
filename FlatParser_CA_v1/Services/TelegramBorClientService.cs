using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Services.Interfaces;
using Telegram.Bot;

namespace FlatParser_CA_v1.Services
{
    public class TelegramBotClientService : ITelegramBotClientService
    {
        public ITelegramBotClient Bot { get; }
        private StoredConfigs ConfigSettings { get; }

        public TelegramBotClientService(StoredConfigs config)
        {
            ConfigSettings = config;
            Bot = new TelegramBotClient(ConfigSettings.Config.AccessToken);
        }

        public async Task SendMessage(long chatId, string message)
        {
            await Bot.SendMessage(chatId, message);
        }
    }
}
