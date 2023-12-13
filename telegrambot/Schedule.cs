using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegrambot
{
    public class Schedule
    {
        public Dictionary<DayOfWeek, string[]> timetable = new Dictionary<DayOfWeek, string[]>();
        public Schedule()
        {
            string[] weekdays = { "18:00" };
            string[] weekend = { "10:00", "14:00", "18:00" };
            timetable.Add(DayOfWeek.Monday, weekdays);
            timetable.Add(DayOfWeek.Tuesday, weekdays);
            timetable.Add(DayOfWeek.Wednesday, weekdays);
            timetable.Add(DayOfWeek.Thursday, weekdays);
            timetable.Add(DayOfWeek.Friday, new string[2] {"13:00","19:00" });
            timetable.Add(DayOfWeek.Saturday, weekend);
            timetable.Add(DayOfWeek.Sunday, weekend);
        }
    }
}
