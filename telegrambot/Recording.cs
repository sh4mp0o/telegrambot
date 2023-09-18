using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    public static class Recording
    {
        public static DateTime dateTime;
        public static string time;
        public static async void RecordingDay(ITelegramBotClient botClient,Chat chat,CancellationToken cancellationToken)
        {
            DateTime today = DateTime.Now;
            var inlineKeyboard1 = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                {
                    // Каждый новый массив - это дополнительные строки,
                    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                    new InlineKeyboardButton[] // тут создаем массив кнопок
                    {
                        InlineKeyboardButton.WithCallbackData(today.AddDays(1).Day+"."+today.Month, "button0 1"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(2).Day+"."+today.Month, "button0 2"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(3).Day+"."+today.Month, "button0 3"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(today.AddDays(4).Day+"."+today.Month, "button0 4"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(5).Day+"."+today.Month, "button0 5"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(6).Day+"."+today.Month, "button0 6"),
                        InlineKeyboardButton.WithCallbackData(today.AddDays(7).Day+"."+today.Month, "button0 7"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад","button4")
                    }
                });
            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Выберите день для маникюра!",
                replyMarkup: inlineKeyboard1,
                cancellationToken: cancellationToken);

        }
        public static async void RecordingTime(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            var inlineKeyboard2 = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                {
                    // Каждый новый массив - это дополнительные строки,
                    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                    new InlineKeyboardButton[] // тут создаем массив кнопок
                    {
                        InlineKeyboardButton.WithCallbackData("14:00-16:00", "button5 14:00-16:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("16:00-18:00", "button5 16:00-18:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("18:00-20:00", "button5 18:00-20:00"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад","button4")
                    }
                });
            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Выберите время для маникюра!",
                replyMarkup: inlineKeyboard2,
                cancellationToken: cancellationToken);
            return;
        }
    }
}
