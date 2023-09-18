using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    public static class Recording
    {
        public static DateTime dateTime;
        public static string time;
        public static async void RecordingDay(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            DateTime today = DateTime.Now;
            var daysKeyboard = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                {
                    new InlineKeyboardButton[] // тут создаем массив кнопок
                    {
                        InlineKeyboardButton.WithCallbackData(today.AddDays(1).Day+"."+today.Month, "day 1"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(2).Day+"."+today.Month, "day 2"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(3).Day+"."+today.Month, "day 3"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(today.AddDays(4).Day+"."+today.Month, "day 4"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(5).Day+"."+today.Month, "day 5"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(6).Day+"."+today.Month, "day 6"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(7).Day+"."+today.Month, "day 7"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backButton") //либо альтернатива: писать для каждого уровня свою кнопку назад
                    }
                });

            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Выберите дату 💅🏼",
                replyMarkup: daysKeyboard,
                cancellationToken: cancellationToken);

        }
        public static async void RecordingTime(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            var timeKeyboard = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                {
                    new InlineKeyboardButton[] // тут создаем массив кнопок
                    {
                        InlineKeyboardButton.WithCallbackData("14:00-16:00", "time 14:00-16:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("16:00-18:00", "time 16:00-18:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("18:00-20:00", "time 18:00-20:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backButton") //либо альтернатива: писать для каждого уровня свою кнопку назад
                    }
                });

            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Выберите время💅🏼",
                replyMarkup: timeKeyboard,
                cancellationToken: cancellationToken);

            return;
        }
    }
}
