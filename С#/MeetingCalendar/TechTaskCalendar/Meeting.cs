using System;
using System.Linq;

namespace MeetingCalendar
{
    public class Meeting
    {
        //Принимает и возвращает название встречи
        public string Name { get; set; }
        //Принимает и возвращает дату встречи
        public DateTime Date { get; set; }
        //Принимает и возвращает время начала встречи
        public TimeSpan BeginTime { get; set; }
        //Принимает и возвращает время окончания встречи
        public TimeSpan EndTime { get; set; }
        //Принимает и возвращает время оповещения о встрече
        public TimeSpan NoticeTime { get; set; }
    }
}