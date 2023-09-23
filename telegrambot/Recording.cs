using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    public static class Recording
    {
        public static DateTime dateTime;
        public static string time;
        public static async void RecordingDay(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken, CallbackQuery callbackQuery)
        {
            try
            {
                await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                $"Выберите дату 💅🏼",
                replyMarkup: Keyboards.daysKeyboard,
                cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static async void RecordingTime(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken, CallbackQuery callbackQuery)
        {
            try
            {
                await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                $"Выберите время💅🏼",
                replyMarkup: Keyboards.timeKeyboard,
                cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
