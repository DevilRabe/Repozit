using System;
using System.IO;
using System.Threading.Tasks;

namespace MeetingCalendar
{
    class Program : MeetingClass
    {

         static void Main(string[] args)
        {
            Program meeting = new Program();
            meeting.PrintHello();
            Console.WriteLine();
            meeting.CheckDirectory();
            bool Exit = true;

            /*
            Ожидаем ввода от пользователя пункта меню
            */

            while (Exit)
            {
                meeting.PrintMenu();
                meeting.MeetingsCheck();

                string USERENTER = meeting.TakeTextMenu();
                switch (USERENTER)
                {
                    case "1":
                        meeting.CreateDate();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "2":
                        meeting.ReadFile();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "3":
                        meeting.PrintCurrentDate();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "4":
                        meeting.ChangeDate();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "5":
                        meeting.ChangeStartTimeMeeting();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "6":
                        meeting.ChangeNoticeTimeMeeting();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "7":
                        meeting.ImportMeetingsToFile();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "8":
                        meeting.ClearMeetings();
                        meeting.MeetingsCheck(); ;
                        PrintBreaks();
                        break;
                    case "9":
                        meeting.Close();
                        PrintBreaks();
                        Exit = false;
                        break;
                }
            }
            Console.Read();
        }
        static void PrintBreaks()
        {
            Console.WriteLine("\n\n");
        }
    }
}
