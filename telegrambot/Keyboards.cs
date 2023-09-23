using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    static class Keyboards
    {
        public static DateTime today = DateTime.Now;

        // Главное меню
        public static InlineKeyboardMarkup mainMenu = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Записаться 🗓", "recButton"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Контакты 📱", "contactButton"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithUrl("Отзывы 📝", "https://vk.com/your_nails_yaroslavl"),
                    },
            });
        //подтверждение записи
        public static InlineKeyboardMarkup confirmKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да, все верно ✅", "confirmButton"),
                    },
                    new InlineKeyboardButton[]
                    { 
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backConfirm") //либо альтернатива: писать для каждого уровня свою кнопку назад
                    },
            });
        //кнопка "назад" из уровня контактов
        public static InlineKeyboardMarkup backContacts = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("Назад ◀️", "backContacts")}
            });

        public static InlineKeyboardMarkup daysKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]
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
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backDays")
                    }
            });

        public static InlineKeyboardMarkup timeKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]
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
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backTime")
                    }
            });
    }
}
