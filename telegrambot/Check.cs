
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
        static public string KeyboardDay(int j)
        {
            _client = serializationOfClient.Deserialization();
            Schedule schedule = new Schedule();
            DateTime _date = new DateTime();
            _date = DateTime.Today.AddDays(j);
            int count = _client.FindAll(x => x.DateTime.Day == DateTime.Today.AddDays(j).Day).Count;
            if (count < schedule.timetable[_date.DayOfWeek].Length)
            {
                return $"true {DateTime.Today.AddDays(j).Day}.{DateTime.Today.Month}";
            }
            return "false";
        }
        static public InlineKeyboardMarkup KeyboardDays()
        {
            List<List<InlineKeyboardButton>> list = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
            };
            int rows = 0;
            for (int i = 1; i < 4; i++)
            {
                if (KeyboardDay(i).Split().First() == "true")
                {
                    list[rows].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardDay(i).Split().Last()}", $"day {i}"));
                }
            }
            rows++;
            list.Add(new List<InlineKeyboardButton>());
            for (int i = 4; i < 8; i++)
            {
                if (KeyboardDay(i).Split().First() == "true")
                {
                    list[rows].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardDay(i).Split().Last()}", $"day {i}"));
                }
            }
            rows++;
            list.Add(new List<InlineKeyboardButton>());
            list[rows].Add(InlineKeyboardButton.WithCallbackData("Назад ◀️", "backDays"));
            return new InlineKeyboardMarkup(list);
        }
        static public InlineKeyboardMarkup KeyboardTimes(long id, List<Client> clients)
        {
            DateTime _date = new DateTime();
            Schedule schedule = new Schedule();
            DateTime _dateTime = new DateTime(2023,10,24,10,0,0);
            _client = serializationOfClient.Deserialization();
            _date = clients.Find(x => x.Id == id).DateTime;
            int count = schedule.timetable[_date.DayOfWeek].Length;
            List<List<InlineKeyboardButton>> list = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new List<InlineKeyboardButton>());
                if (KeyboardTime(schedule.timetable[_date.DayOfWeek][i], _date).Split().First() == "true")
                {
                    list[i].Add(InlineKeyboardButton.WithCallbackData($"{KeyboardTime(schedule.timetable[_date.DayOfWeek][i], _date).Split().Last()}", $"time {KeyboardTime(schedule.timetable[_date.DayOfWeek][i], _date).Split().Last()}"));
                }
            }
            list.Add(new List<InlineKeyboardButton>());
            list[count].Add(InlineKeyboardButton.WithCallbackData("Назад ◀️", "backTime"));
            return new InlineKeyboardMarkup(list);
        }
        static public string KeyboardTime(string time,DateTime date)
        {
            _client = serializationOfClient.Deserialization();
            if (_client.Exists(x => x.DateTime.Day == date.Day && x.Time == time))
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