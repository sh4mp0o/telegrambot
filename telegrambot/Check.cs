using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
#pragma warning disable CS8602
#pragma warning disable CS8600


namespace telegrambot
{
    static internal class Check
    {
        private static List<Client>? _client;
        private static SerializationOfClient serializationOfClient = new SerializationOfClient(new JSONSerialization());
        static public string KeyboardDay(int i)
        {
            _client = serializationOfClient.Deserialization();
            DateTime _date = new DateTime();
            _date = DateTime.Today.AddDays(i);
            int count = _client.FindAll(x => x.DateTime.Day == DateTime.Today.AddDays(i).Day).Count;
            if (_date.DayOfWeek == DayOfWeek.Sunday || _date.DayOfWeek == DayOfWeek.Saturday)
            {
                if (count < 3)
                {
                    return $"true {DateTime.Today.AddDays(i).Day}.{DateTime.Today.Month}";
                }
                return "false";
            }
            else
            {
                if (count <= 0)
                {
                    return $"true  {DateTime.Today.AddDays(i).Day}.{DateTime.Today.Month}";
                }
                return "false";
            }
        }
        static public InlineKeyboardMarkup KeyboardDays()
        {
            List<List<InlineKeyboardButton>> list = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
            };
            for (int i = 1; i < 4; i++)
            {
                if (KeyboardDay(i).Split().First() == "true")
                {
                    list[0].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardDay(i).Split().Last()}", $"day {i}"));
                }
            }
            list.Add(new List<InlineKeyboardButton>());
            for (int i = 4; i < 8; i++)
            {
                if (KeyboardDay(i).Split().First() == "true")
                {
                    list[1].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardDay(i).Split().Last()}", $"day {i}"));
                }
            }
            list.Add(new List<InlineKeyboardButton>());
            list[2].Add(InlineKeyboardButton.WithCallbackData("Назад ◀️", "backDays"));
            return new InlineKeyboardMarkup(list);
        }
        static public InlineKeyboardMarkup KeyboardTimes(long id, List<Client> clients)
        {
            DateTime _date = new DateTime();
            DateTime _dateTime = new DateTime(2023,10,24,10,0,0);
            _client = serializationOfClient.Deserialization();
            _date = clients.Find(x => x.Id == id).DateTime;
            List<List<InlineKeyboardButton>> list = new List<List<InlineKeyboardButton>>();
            if (_date.DayOfWeek == DayOfWeek.Sunday || _date.DayOfWeek == DayOfWeek.Saturday)
            {
                for (int i = 0; i < 3; i++)
                {
                    list.Add(new List<InlineKeyboardButton>());
                    if (KeyboardTime(_dateTime.AddHours(i*4).Hour.ToString(), _date).Split().First() == "true")
                    {
                        list[i].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardTime(_dateTime.AddHours(i * 4).Hour.ToString(), _date).Split().Last()}:00", $"time {KeyboardTime(_dateTime.AddHours(i * 4).Hour.ToString(), _date).Split().Last()}:00"));
                    }
                }
                list.Add(new List<InlineKeyboardButton>());
                list[3].Add(InlineKeyboardButton.WithCallbackData("Назад ◀️", "backTime"));
                return new InlineKeyboardMarkup(list);
            }
            else
            {
                list.Add(new List<InlineKeyboardButton>());
                if (KeyboardTime("18:00", _date).Split().First() == "true")
                {
                    list[0].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardTime("18:00", _date).Split().Last()}", $"time {KeyboardTime("18:00", _date).Split().Last()}"));
                }
                list.Add(new List<InlineKeyboardButton>());
                list[1].Add(InlineKeyboardButton.WithCallbackData("Назад ◀️", "backTime"));
                return new InlineKeyboardMarkup(list);
            }

        }
        static public string KeyboardTime(string time,DateTime date)
        {
            _client = serializationOfClient.Deserialization();
            if (_client.Exists(x => x.DateTime.Day == date.Day && x.Time == time+":00"))
            {
                return "false";
            }
            else
            {
                return $"true {time}";
            }
        }
    }
}