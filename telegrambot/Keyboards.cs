using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    interface IKeyboards
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
                        InlineKeyboardButton.WithUrl("Мои работы 📝", "https://vk.com/your_nails_yaroslavl"),
                    },
            });
        //подтверждение записи
        public static InlineKeyboardMarkup confirmKeyboard = new InlineKeyboardMarkup(
            new List<List<InlineKeyboardButton>>()
            {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("Да, все верно ✅", "confirmButton"),
                    },
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("Назад ◀️","backConfirm") //либо альтернатива: писать для каждого уровня свою кнопку назад
                    },
            });
        public static InlineKeyboardMarkup backContacts = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("Назад ◀️", "backContacts")}
            });
        public static InlineKeyboardMarkup buzyday = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("Назад ◀️", "backTime") }
            });
        public static InlineKeyboardMarkup buzyTime = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                    new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("Назад ◀️", "backConfirm") }
            });

        //public static InlineKeyboardMarkup daysKeyboard = new InlineKeyboardMarkup(
        //    new List<List<InlineKeyboardButton>>()
        //    {
        //            new List<InlineKeyboardButton>
        //            {
        //                Check.KeyboardDay(1),
        //                Check.KeyboardDay(2),
        //                Check.KeyboardDay(3),
        //            },
        //            new List<InlineKeyboardButton>
        //            {
        //                Check.KeyboardDay(4),
        //                Check.KeyboardDay(5),
        //                Check.KeyboardDay(6),
        //                Check.KeyboardDay(7),
        //            },
        //            new List<InlineKeyboardButton>
        //            {
        //                InlineKeyboardButton.WithCallbackData("Назад ◀️","backDays")
        //            }
        //    });

        //public static InlineKeyboardMarkup timeKeyboard = new InlineKeyboardMarkup(
        //    new List<InlineKeyboardButton[]>()
        //    {
        //            new InlineKeyboardButton[]
        //            {
        //                InlineKeyboardButton.WithCallbackData("10:00", "time 10:00"),
        //            },
        //            new InlineKeyboardButton[]
        //            {
        //                InlineKeyboardButton.WithCallbackData("14:00", "time 14:00"),
        //            },
        //            new InlineKeyboardButton[]
        //            {
        //                InlineKeyboardButton.WithCallbackData("18:00", "time 18:00"),
        //            },
        //            new InlineKeyboardButton[]
        //            {
        //                InlineKeyboardButton.WithCallbackData("Назад ◀️","backTime")
        //            }
        //    });
        public static InlineKeyboardMarkup Time(long id,List<Client> clients)
        {
            return Check.KeyboardTimes(id,clients);
        }
        public static InlineKeyboardMarkup Day()
        {
            return Check.KeyboardDays();
        }
    }
}
