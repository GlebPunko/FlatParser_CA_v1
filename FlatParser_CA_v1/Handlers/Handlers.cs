using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlatParser_CA_v1.Handlers
{
    public class Handlers
    {
        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            string responseMessage = (update.Message?.Text == "/hello") ? "Hello world!" : "I don`t understand...";
            await bot.SendMessage(update.Message.Chat.Id, responseMessage, cancellationToken: cancellationToken);
        }

        public static async Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync("Error happend...");
        }
    }
}
