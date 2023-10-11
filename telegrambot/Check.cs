using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    static internal class Check
    {
        private static List<Client> _client;
        private static DateTime _date;
        static public InlineKeyboardButton KeyboardDay(int i)
        {
            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _client = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception ex) { _client = new(); }
            DateTime _date = new DateTime();
            _date = DateTime.Today.AddDays(i);
            int count = _client.FindAll(x => x.DateTime.Day == DateTime.Today.AddDays(i).Day).Count;
            if (_date.DayOfWeek == DayOfWeek.Sunday || _date.DayOfWeek == DayOfWeek.Saturday)
            {
                if (count < 3)
                {
                    return InlineKeyboardButton.WithCallbackData(DateTime.Today.AddDays(i).Day + "." + DateTime.Today.Month, $"day {i}");
                }
                return InlineKeyboardButton.WithCallbackData(DateTime.Today.AddDays(i).Day + "." + DateTime.Today.Month, $"buzyday");
            }
            else
            {
                if (count <= 0)
                {
                    return InlineKeyboardButton.WithCallbackData(DateTime.Today.AddDays(i).Day + "." + DateTime.Today.Month, $"day {i}");
                }
                return InlineKeyboardButton.WithCallbackData(DateTime.Today.AddDays(i).Day + "." + DateTime.Today.Month, $"buzyday");

            }
        }

        static public InlineKeyboardMarkup KeyboardDayAndTime(long id, List<Client> clients)
        {
            DateTime _date = new DateTime();
            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _client = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception ex) { _client = new(); }
            _date = clients.Find(x => x.Id == id).DateTime;
            if (_date.DayOfWeek == DayOfWeek.Sunday || _date.DayOfWeek == DayOfWeek.Saturday)
            {
                return new InlineKeyboardMarkup(
                   new List<InlineKeyboardButton[]>()
                   {
                        new InlineKeyboardButton[]
                        {
                            KeyboardTime("10:00",_date),
                            //InlineKeyboardButton.WithCallbackData("10:00", "time 10:00"),
                        },
                        new InlineKeyboardButton[]
                        {
                            KeyboardTime("14:00",_date),
                            //InlineKeyboardButton.WithCallbackData("14:00", "time 14:00"),
                        },
                        new InlineKeyboardButton[]
                        {
                            KeyboardTime("18:00",_date),
                            //InlineKeyboardButton.WithCallbackData("18:00", "time 18:00"),
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.WithCallbackData("Назад ◀️","backTime")
                        }
                   });
            }
            else
            {
                return new InlineKeyboardMarkup(
                   new List<InlineKeyboardButton[]>()
                   {
                        new InlineKeyboardButton[]
                        {
                            KeyboardTime("18:00",_date),
                            //InlineKeyboardButton.WithCallbackData("18:00", "time 18:00"),
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.WithCallbackData("Назад ◀️","backTime")
                        }
                   }) ;

            }
        }
        static public InlineKeyboardButton KeyboardTime(string time,DateTime date)
        {
            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _client = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception ex) { _client = new(); }
            if (_client.Exists(x => x.DateTime.Day == date.Day && x.Time == time))
            {
                return InlineKeyboardButton.WithCallbackData(time, $"buzytime");
            }
            else
            {
                return InlineKeyboardButton.WithCallbackData(time, $"time {time}");
            }
        }
    }
}