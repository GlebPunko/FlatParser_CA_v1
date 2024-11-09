using FlatParser_CA_v1.Helper.Interface;
using FlatParser_CA_v1.Workers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace FlatParser_CA_v1.Helper
{
    public class BotHelper : IBotHelper
    {
        public TelegramBotClient Bot { get; }

        public BotHelper(TelegramBotClient bot)
        {
            Bot = bot;

            var receivedOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            //Worker.RunWorker();
        }
    }
}
