using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MeetingCalendar
{

    class MeetingClass
    {
        // Приватная переменная для хранения файла
        private static readonly string filePath = @"C:\MeetingCalendar\Meetings.xml";

        // Приватная переменная для указания пути файла в который будет происходить импорт
        private static readonly string importFilePath = @"C:\MeetingCalendar\Meetings.txt";

        // приватная переменная для хранения пути в котором будет размещаться файл с данными
        private static readonly string directoryPath = @"C:\MeetingCalendar";

        // Приватная переменная для установки настроек "культуры" в которой будет работать пользователь и оборабатываться данные времени и дат
        private CultureInfo cultureInfo = new CultureInfo("ru-RU");

        // Текущая дата в системе
        private DateTime currentDate = DateTime.Now;

        //Втреча которую мы будем записывать
        private Meeting meeting;

        // Вспомогательный лист для определения наличия окон в расписании
        private List<char> chrAvalibles = new List<char>();

        /*
         *  Метод для проверки и оповещения о преближающейся встрече
         */
        protected void MeetingsCheck()
        {
            XmlSerializer meetingSerializer;
            List<Meeting> meetingsFromFile;

            DateTime NowDate = DateTime.Now;


            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fileStream);
                }

                foreach(Meeting meet in meetingsFromFile)
                {
                    DateTime Date = meet.Date;
                    TimeSpan BeginTime = meet.BeginTime;
                    TimeSpan NoticeTime = meet.NoticeTime;

                    if(NowDate.Date == Date && NowDate.TimeOfDay == BeginTime-NoticeTime)
                    {
                        Console.WriteLine("Не забудьте о встрече! Тема встречи:{0}, Время начала:{1}", meet.Name,meet.BeginTime);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("нет запланированных мероприятий");
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }

        }
        /*
            Метод для отображения меню приложения пользователю
         */
        protected void PrintMenu()
        {
            Console.WriteLine("\t=======\n\t|МЕНЮ|\n\t=======");
            Console.WriteLine("1. Запланировать новую дату.");
            Console.WriteLine("2. Показать все запланнированные даты.");
            Console.WriteLine("3. Показать текущую дату.");
            Console.WriteLine("4. Изменить дату встречи.");
            Console.WriteLine("5. Изменить время встречи.");
            Console.WriteLine("6. Изменить время оповещения.");
            Console.WriteLine("7. Импортировать встречи в файл.");
            Console.WriteLine("8. Очистить все даты и встречи.");
            Console.WriteLine("9. Выход из приложения.");
        }
        /*
            Метод закрытия приложения
         */
        protected async void Close()
        {
            Console.WriteLine("\t===============================\n\t|Как жаль что вы уходите  T_T |\n\t===============================");
            await Task.Delay(2000);
            Environment.Exit(0);
        }
        /*
            Метод для выведения на экран приветствия
         */
        protected void PrintHello()
        {
            Console.WriteLine("Hello! \nThis is CalendarConsolApp or CCA");
            Console.WriteLine("Please, use this app smart and enjoy yourself!");
            Console.WriteLine("Please enter your date in format \" Age, Mounth, Day, Hour, Minutes, Sec \"");
        }
        /*
            Метод для проверки наличия на диске С папки в которую будет сохранять све свои данные. 
            Если дирректории нет, то метод создаст дирректорию по пути  C:\MeetingCalendar
         */
        protected void CheckDirectory()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (directoryInfo.Exists)
            {
            }
            else
            {
                Console.WriteLine("Directory not found!");
                Directory.CreateDirectory(@"C:\MeetingCalendar");
                directoryInfo.Create();
                Console.WriteLine("Directory was created!");
            }
        }
        /*
            Метод для записи новой встречи в файл. Если файл в назначенной дирректории отсутствует то файл будет создан
            и в созданный файл будет записан новый объект
         */
        protected void CreateDate()
        {
            /*Создание переменных которые необходимы для работы, сериализации и задания формата объекта встречи*/
            // Название встречи
            string meetingName;

            // Дата встречи вводимая с консоли
            DateTime parseDate;
            // Время начала встречи считываемое с консоли
            TimeSpan parseBeginTime;
            // Время окончания встречи считываемое с консоли
            TimeSpan parseEndTime;
            // Время напоминания о встрече
            TimeSpan parseNoticeTime;


            XmlSerializer meetingSerializer;
            List<Meeting> meetingsFromFile;
            List<char> chrAvalibles = new List<char>();

            // Задания форматов для распознавания ввода форматов даты и времени в консоль пользователем
            string[] dateFormats = { "d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy", "d.MM.yyyy", "dd.MM.yyyy", "d.M.yyyy" };
            string[] timeFormats = { "hh\\:mm", "h\\:mm", "hh\\-mm", "h||-mm" };

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fileStream);
            }

            // Запрашиваем у пользователя  названия для встречи
            Console.WriteLine("========================\n|Введение новой встречи|\n========================\n");
            Console.WriteLine("Введите название встречи");
            meetingName = Console.ReadLine();
            bool correktEnter = false;
            while (!correktEnter)
            {
                //Запрашиваем у пользователя дату встречи
                Console.WriteLine(@"Введите дату для записи в файл в формате (ДД.ММ.ГГГГ) или (ДД/ММ/ГГГГ)");
                string dateToFile = Console.ReadLine();
                bool isValidDate = DateTime.TryParseExact(dateToFile, dateFormats, cultureInfo, DateTimeStyles.None, out parseDate);
                while (!isValidDate)
                {
                    Console.WriteLine("Дата введена в неверном формате!");
                    Console.WriteLine(@"Введите дату для записи в файл в формате (ДД.ММ.ГГГГ) или (ДД/ММ/ГГГГ)");
                    dateToFile = Console.ReadLine();
                    isValidDate = DateTime.TryParseExact(dateToFile, dateFormats, cultureInfo, DateTimeStyles.None, out parseDate);
                    Console.WriteLine("{0}", parseDate);
                    if (isValidDate)
                    {
                        if (parseDate < currentDate)
                        {
                            Console.WriteLine("Нельзя запланировать встречи на прошлое! Введите дату еще раз.");
                            isValidDate = false;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Запрашиваем у пользователя время начала встречи
                Console.WriteLine(@"Введите время начала встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ)");
                string startTimeToFile = Console.ReadLine();
                bool isValidStartTime = TimeSpan.TryParseExact(startTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseBeginTime);
                while (!isValidStartTime)
                {
                    Console.WriteLine("Время введено в неверном формате!");
                    Console.WriteLine(@"Введите время начала встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ)");
                    startTimeToFile = Console.ReadLine();
                    isValidStartTime = TimeSpan.TryParseExact(startTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseBeginTime);
                }

                // Запрашиваем у пользователя время окончания встречи, если оно отсутствует просим ввести 00:00
                Console.WriteLine(@"Введите время окончания встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если время окончания отсутствует, то введите 00:00 или 00-00");
                string endTimeToFile = Console.ReadLine();
                bool isValidEndTime = TimeSpan.TryParseExact(endTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseEndTime);
                bool corTime = true;
                while (!isValidEndTime && corTime)
                {
                    Console.WriteLine("Время введено в неверном формате!");
                    Console.WriteLine(@"Введите время окончания встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если время окончания отсутствует, то введите 00:00 или 00-00");
                    endTimeToFile = Console.ReadLine();
                    isValidEndTime = TimeSpan.TryParseExact(endTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseEndTime);

                    if (parseEndTime < parseBeginTime)
                    {
                        corTime = false;
                        Console.WriteLine("Время окончания не может быть меньше времени начала");
                    }
                }

                // Запрашиваем у пользователя время за которое его необходимо оповестить до начала встречи
                Console.WriteLine(@"Введите время напоминания в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если напоминание не нужно то введите 00:00 или 00-00");
                string noticeTimeToFile = Console.ReadLine();
                bool isValidNoticeTime = TimeSpan.TryParseExact(noticeTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseNoticeTime);
                while (!isValidNoticeTime)
                {
                    Console.WriteLine("Время введено в неверном формате!");
                    Console.WriteLine(@"Введите время окончания встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если время окончания отсутствует, то введите 00:00 или 00-00");
                    noticeTimeToFile = Console.ReadLine();
                    isValidNoticeTime = TimeSpan.TryParseExact(noticeTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseNoticeTime);
                }

                meeting = new Meeting { Name = meetingName, Date = parseDate, BeginTime = parseBeginTime, EndTime = parseEndTime, NoticeTime = parseNoticeTime };

                // проверяем наличие "окна и пересечений"
                foreach (Meeting meet in meetingsFromFile)
                {
                    if (meet.Date > meeting.Date)
                    {
                        chrAvalibles.Add('A');
                    }
                    if (meet.Date == meeting.Date)
                    {
                        if (meet.EndTime != new TimeSpan(00, 00, 00))
                        {
                            if (meet.EndTime <= meeting.BeginTime)
                            {
                                chrAvalibles.Add('A');
                            }
                            else
                            {
                                if (meet.BeginTime <= meeting.EndTime)
                                {
                                    chrAvalibles.Add('A');
                                }
                                else
                                {
                                    chrAvalibles.Add('N');
                                }
                            }
                        }
                        else if (meet.BeginTime <= meeting.BeginTime)
                        {
                            chrAvalibles.Add('A');
                        }
                        else
                        {
                            chrAvalibles.Add('N');
                        }
                    }
                }

                // Проверяем имеется ли свободное "окошко" для вводимой даты
                if (chrAvalibles.Contains('N'))
                {
                    correktEnter = false;
                    Console.WriteLine("Обнаружено наложение даты на другие уже имеющиеся встречи. Измените вводимые параметры");
                }
                else
                {
                    correktEnter = true;
                }
            }
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    meetingsFromFile.Add(meeting);
                    fileStream.SetLength(0);
                    meetingSerializer.Serialize(fileStream, meetingsFromFile);
                    Console.WriteLine("\t=====================\n\t| Встреча записана! |\n\t=====================");
                }
            }
            catch (InvalidOperationException)
            {
                List<Meeting> meetingToFile = new List<Meeting> { meeting };
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    meetingSerializer.Serialize(fileStream, meetingToFile);
                    Console.WriteLine("\t=====================\n\t| Встреча записана! |\n\t=====================");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
            Метод для отображения всех встреч, которые имеются в файле. 
            В случае если файл пуст, то запланированные встречи отсутсвуют
         */
        protected void ReadFile()
        {
            try
            {
                using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    List<Meeting> meetings = (List<Meeting>)meetingSerializer.Deserialize(fstream);
                    int countT = 1;
                    Console.WriteLine("\t=========================\n\t|Запланированные встречи|\n\t=========================");
                    foreach (Meeting item in meetings)
                    {
                        if (item.EndTime != new TimeSpan(00, 00, 00))
                        {
                            Console.WriteLine("Номер встречи: " + countT + "\n" + "Название встречи: " + item.Name + "\n" + "Дата встречи: " + item.Date.ToString("d", cultureInfo) + "\n" + "Начало встречи: " + item.BeginTime.ToString(@"hh\:mm") + "\n" + "Конец встречи: " + item.EndTime.ToString(@"hh\:mm") + "\n" + "Напоминание: " + item.NoticeTime.ToString(@"hh\:mm"));
                        }
                        else
                        {
                            Console.WriteLine("Номер встречи: " + countT + "\n" + "Название встречи: " + item.Name + "\n" + "Дата встречи: " + item.Date.ToString("d", cultureInfo) + "\n" + "Начало встречи: " + item.BeginTime.ToString(@"hh\:mm") + "\n" + "Напоминание: " + item.NoticeTime.ToString(@"hh\:mm"));
                        }
                        Console.WriteLine();
                        countT++;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("нет запланированных мероприятий");
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         *  Метод очистки файла который содержит данные о всех встречах.
         *  В случае отсутствия файла, обрабатываем исключение FileNotFoundException и выдаем текстовое сообщение.
         */
        protected void ClearMeetings()
        {
            try
            {

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    fileStream.SetLength(0);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл отсутствует!");
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         *  Метод измнения даты встречи
         */
        protected void ChangeDate()
        {
            // Задания форматов для распознавания ввода форматов даты в консоль пользователем
            string[] dateFormats = { "d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy", "d.MM.yyyy", "dd.MM.yyyy", "d.M.yyyy" };

            // Дата встречи вводимая с консоли
            DateTime parseDate;

            try
            {
                int inputMeetingNumber = 0;
                int meetingsCount = 0;
                //Console.WriteLine("=========\n|Встречи|\n=========");
                using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    List<Meeting> meetings = (List<Meeting>)meetingSerializer.Deserialize(fstream);
                    meetingsCount = meetings.Count;
                }
                if (meetingsCount > 0)
                {
                    ReadFile();
                    Console.WriteLine("Введите номер встречи, дату которой требуется изменить:");

                    while (!Int32.TryParse(Console.ReadLine(), out inputMeetingNumber))
                    {
                        Console.WriteLine("Неверно выбрана встреча, введите номер встречи еще раз!");
                    }

                    using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                    {
                        XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                        List<Meeting> meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);

                        Meeting meetingToFile = meetingsFromFile[inputMeetingNumber - 1];

                        //  Просим от пользователя ввести новую дату встречи, до тех пор пока он не введет ее в правильном формате
                        bool correktEnter = false;
                        while (!correktEnter)
                        {
                            Console.WriteLine(@"Введите дату для записи в файл в формате (ДД.ММ.ГГГГ) или (ДД/ММ/ГГГГ)");
                            string dateToFile = Console.ReadLine();

                            bool isValidDate = DateTime.TryParseExact(dateToFile, dateFormats, cultureInfo, DateTimeStyles.None, out parseDate);
                            while (!isValidDate)
                            {
                                Console.WriteLine("Дата введена в неверном формате!");
                                Console.WriteLine(@"Введите дату для записи в файл в формате (ДД.ММ.ГГГГ) или (ДД/ММ/ГГГГ)");
                                dateToFile = Console.ReadLine();
                                isValidDate = DateTime.TryParseExact(dateToFile, dateFormats, cultureInfo, DateTimeStyles.None, out parseDate);
                                Console.WriteLine("{0}", parseDate);
                                if (isValidDate)
                                {
                                    if (parseDate < currentDate)
                                    {
                                        Console.WriteLine("Нельзя запланировать встречи на прошлое! Введите дату еще раз.");
                                        isValidDate = false;
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            // Формируем новый объект встречу и перезаписываем 
                            meetingToFile = new Meeting { Name = meetingToFile.Name, Date = parseDate, BeginTime = meetingToFile.BeginTime, EndTime = meetingToFile.EndTime, NoticeTime = meetingToFile.NoticeTime };

                            // проверяем наличие "окна и пересечений"
                            foreach (Meeting meet in meetingsFromFile)
                            {
                                if (meet.Date > meeting.Date)
                                {
                                    chrAvalibles.Add('A');
                                }
                                if (meet.Date == meeting.Date)
                                {
                                    if (meet.EndTime != new TimeSpan(00, 00, 00))
                                    {
                                        if (meet.EndTime <= meeting.BeginTime)
                                        {
                                            chrAvalibles.Add('A');
                                        }
                                        else
                                        {
                                            if (meet.BeginTime <= meeting.EndTime)
                                            {
                                                chrAvalibles.Add('A');
                                            }
                                            else
                                            {
                                                chrAvalibles.Add('N');
                                            }
                                        }
                                    }
                                    else if (meet.BeginTime <= meeting.BeginTime)
                                    {
                                        chrAvalibles.Add('A');
                                    }
                                    else
                                    {
                                        chrAvalibles.Add('N');
                                    }
                                }
                            }

                            // Проверяем имеется ли свободное "окошко" для вводимой даты
                            if (chrAvalibles.Contains('N'))
                            {
                                correktEnter = false;
                                Console.WriteLine("Обнаружено наложение даты на другие уже имеющиеся встречи. Измените вводимые параметры");
                            }
                            else
                            {
                                correktEnter = true;
                            }

                        }
                        // перезаписываем всречи с новой записанной
                        meetingsFromFile.RemoveAt(inputMeetingNumber - 1);
                        meetingsFromFile.Insert(inputMeetingNumber - 1, meetingToFile);
                        fstream.SetLength(0);
                        meetingSerializer.Serialize(fstream, meetingsFromFile);
                        Console.WriteLine("Дата встречи перезаписана!");
                    }
                }
                else
                {
                    Console.WriteLine("Встречи не обнаружены");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         *  Метод изменения начала времени встречи
         */
        protected void ChangeStartTimeMeeting()
        {
            // Задание форматов для распознавания ввода форматов времени в консоль пользователем
            string[] timeFormats = { "hh\\:mm", "h\\:mm", "hh\\-mm", "h||-mm" };

            // Дата встречи вводимая с консоли
            TimeSpan parseBeginTime;

            try
            {
                int inputMeetingNumber = 0;
                int meetingsCount = 0;
                Console.WriteLine("\t=========\n\t|Встречи|\n\t=========");

                // считаем сколько встреч в файле записано
                using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    List<Meeting> meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);
                    meetingsCount = meetingsFromFile.Count;
                }
                // если больше 0 то отображаем и просим выбрать ту, в которую нужно внести изменения
                if (meetingsCount > 0)
                {
                    ReadFile();
                    Console.WriteLine("Введите номер встречи, время начала которой требуется изменить:");

                    while (!Int32.TryParse(Console.ReadLine(), out inputMeetingNumber))
                    {
                        Console.WriteLine("Неверно выбрана встреча, введите номер встречи еще раз!");
                    }

                    // Открываем поток, считываем встречи, достаем нужную всречу
                    // После чего просим ввести новое время в нужном формате 
                    using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                    {
                        XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                        List<Meeting> meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);

                        Meeting meetingToFile = meetingsFromFile[inputMeetingNumber - 1];

                        bool correktEnter = false;
                        while (!correktEnter)
                        {
                            //  Просим от пользователя ввести новую дату встречи, до тех пор пока он не введет ее в правильном формате
                            Console.WriteLine(@"Введите время начала встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ)");
                            string startTimeToFile = Console.ReadLine();
                            bool isValidStartTime = TimeSpan.TryParseExact(startTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseBeginTime);

                            while (!isValidStartTime)
                            {
                                Console.WriteLine("Время введено в неверном формате!");
                                Console.WriteLine(@"Введите время начала встречи в формате (ЧЧ:ММ) или (ЧЧ-ММ)");
                                startTimeToFile = Console.ReadLine();
                                isValidStartTime = TimeSpan.TryParseExact(startTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseBeginTime);
                            }

                            // Формируем новый объект встречу и перезаписываем 
                            meetingToFile = new Meeting { Name = meetingToFile.Name, Date = meetingToFile.Date, BeginTime = parseBeginTime, EndTime = meetingToFile.EndTime, NoticeTime = meetingToFile.NoticeTime };

                            // проверяем наличие "окна и пересечений"
                            foreach (Meeting meet in meetingsFromFile)
                            {
                                if (meet.Date > meeting.Date)
                                {
                                    chrAvalibles.Add('A');
                                }
                                if (meet.Date == meeting.Date)
                                {
                                    if (meet.EndTime != new TimeSpan(00, 00, 00))
                                    {
                                        if (meet.EndTime <= meeting.BeginTime)
                                        {
                                            chrAvalibles.Add('A');
                                        }
                                        else
                                        {
                                            if (meet.BeginTime <= meeting.EndTime)
                                            {
                                                chrAvalibles.Add('A');
                                            }
                                            else
                                            {
                                                chrAvalibles.Add('N');
                                            }
                                        }
                                    }
                                    else if (meet.BeginTime <= meeting.BeginTime)
                                    {
                                        chrAvalibles.Add('A');
                                    }
                                    else
                                    {
                                        chrAvalibles.Add('N');
                                    }
                                }
                            }

                            // Проверяем имеется ли свободное "окошко" для вводимой даты
                            if (chrAvalibles.Contains('N'))
                            {
                                correktEnter = false;
                                Console.WriteLine("Обнаружено наложение даты на другие уже имеющиеся встречи. Измените вводимые параметры");
                            }
                            else
                            {
                                correktEnter = true;
                            }
                        }
                        //перезаписываем все всречи вновь в файл с перезаписанной
                        meetingsFromFile.RemoveAt(inputMeetingNumber - 1);
                        meetingsFromFile.Insert(inputMeetingNumber - 1, meetingToFile);
                        fstream.SetLength(0);
                        meetingSerializer.Serialize(fstream, meetingsFromFile);
                        Console.WriteLine("Время начала встречи перезаписано!");
                    }
                }
                else
                {
                    Console.WriteLine("Встречи не обнаружены");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         *  Метод для изменения времени оповещения
         */
        protected void ChangeNoticeTimeMeeting()
        {
            // Задание форматов для распознавания ввода форматов времени в консоль пользователем
            string[] timeFormats = { "hh\\:mm", "h\\:mm", "hh\\-mm", "h||-mm" };

            // Дата напоминания вводимая с консоли
            TimeSpan parseNoticeTime;
            try
            {
                int inputMeetingNumber = 0;
                int meetingsCount = 0;
                Console.WriteLine("\t=========\n\t|Встречи|\n\t=========");
                using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    List<Meeting> meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);
                    meetingsCount = meetingsFromFile.Count;
                }
                if (meetingsCount > 0)
                {
                    ReadFile();
                    Console.WriteLine("Введите номер встречи, время напоминания которой требуется изменить:");

                    while (!Int32.TryParse(Console.ReadLine(), out inputMeetingNumber))
                    {
                        Console.WriteLine("Неверно выбрана встреча, введите номер встречи еще раз!");
                    }


                    using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                    {
                        XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                        List<Meeting> meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);

                        Meeting meetingToFile = meetingsFromFile[inputMeetingNumber - 1];

                        bool correktEnter = false;
                        while (!correktEnter)
                        {

                            //  Просим от пользователя ввести новую дату встречи, до тех пор пока он не введет ее в правильном формате
                            Console.WriteLine(@"Введите новое время уведомления в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если время окончания отсутствует, то введите 00:00 или 00-00");
                            string noticeTimeToFile = Console.ReadLine();
                            bool isValidNoticeTime = TimeSpan.TryParseExact(noticeTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseNoticeTime);
                            while (!isValidNoticeTime)
                            {
                                Console.WriteLine("Время введено в неверном формате!");
                                Console.WriteLine(@"Введите новое время уведомления в формате (ЧЧ:ММ) или (ЧЧ-ММ). Если время окончания отсутствует, то введите 00:00 или 00-00");
                                noticeTimeToFile = Console.ReadLine();
                                isValidNoticeTime = TimeSpan.TryParseExact(noticeTimeToFile, timeFormats, cultureInfo, TimeSpanStyles.None, out parseNoticeTime);
                            }

                            // Формируем новый объект встречу и перезаписываем 
                            meeting = new Meeting { Name = meetingToFile.Name, Date = meetingToFile.Date, BeginTime = meetingToFile.BeginTime, EndTime = meetingToFile.EndTime, NoticeTime = parseNoticeTime };

                            // проверяем наличие "окна и пересечений"
                            foreach (Meeting meet in meetingsFromFile)
                            {
                                if (meet.Date > meeting.Date)
                                {
                                    chrAvalibles.Add('A');
                                }
                                if (meet.Date == meeting.Date)
                                {
                                    if (meet.EndTime != new TimeSpan(00, 00, 00))
                                    {
                                        if (meet.EndTime <= meeting.BeginTime)
                                        {
                                            chrAvalibles.Add('A');
                                        }
                                        else
                                        {
                                            if (meet.BeginTime <= meeting.EndTime)
                                            {
                                                chrAvalibles.Add('A');
                                            }
                                            else
                                            {
                                                chrAvalibles.Add('N');
                                            }
                                        }
                                    }
                                    else if (meet.BeginTime <= meeting.BeginTime)
                                    {
                                        chrAvalibles.Add('A');
                                    }
                                    else
                                    {
                                        chrAvalibles.Add('N');
                                    }
                                }
                            }

                            // Проверяем имеется ли свободное "окошко" для вводимой даты
                            if (chrAvalibles.Contains('N'))
                            {
                                correktEnter = false;
                                Console.WriteLine("Обнаружено наложение даты на другие уже имеющиеся встречи. Измените вводимые параметры");
                            }
                            else
                            {
                                correktEnter = true;
                            }



                            // Записываем в файл новую встречу с обновленными данными
                            meetingsFromFile.RemoveAt(inputMeetingNumber - 1);
                            meetingsFromFile.Insert(inputMeetingNumber - 1, meetingToFile);
                            fstream.SetLength(0);
                            meetingSerializer.Serialize(fstream, meetingsFromFile);
                            Console.WriteLine("Время начала встречи перезаписано!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Встречи не обнаружены");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         *  Метод импорта выбранных дат в файл
         */
        protected void ImportMeetingsToFile()
        {
            try
            {
                List<int> meetingsNumbers = new List<int>();
                int meetingsCount = 0;
                string userEntering = "";

                //Читаем из файла встречи в переменную и считаем сколько их
                using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                    List<Meeting> meetings = (List<Meeting>)meetingSerializer.Deserialize(fstream);
                    meetingsCount = meetings.Count;
                }

                //Если их больше 0 то выводим их на экран. Следом просим у пользователя выбрать какие встречи он желает импортировать в файл
                if (meetingsCount > 0)
                {
                    ReadFile();
                    Console.WriteLine("Введите номер встречи или встреч которые необходимо импортировать в файл.");
                    while (true)
                    {
                        userEntering = TakeTextMethod();
                        if (userEntering.Length > 0)
                        {
                            if (userEntering.Length == 1 && (userEntering.Contains('-') || userEntering.Contains(',') || userEntering.Contains('\n')))
                            {
                                if (userEntering.Length == 1 && userEntering.Contains('\n'))
                                {
                                    if (Console.CursorLeft == 0)
                                        return;

                                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                }
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }

                    // Распознаем в каком формате и сколько встреч выбрал пользователь. доступны 3 варианта
                    // выбрана 1 встреча, выбраны встречи в формате "15-18" или же в формате "4,8,7"
                    // при этом не имеет значения в какой последовательности введены встречи от большего к малому или от малого к большему
                    // т.е. 15-18 и 18-15 одно и тоже. Алгоритм выдаст (15,16,17 и 18) встречи в обоих случаях

                    // разбиваем строку типа "1-12" и "1,5,8" на элементы и распознаем встречи
                    string[] strNumbers;
                    if (userEntering.Contains('-'))
                    {
                        strNumbers = userEntering.Split(new char[] { '-' });

                        for (int i = 0; i < strNumbers.Length; i++)
                        {
                            meetingsNumbers.Add(Convert.ToInt32(strNumbers[i]));
                        }
                    }
                    else if (userEntering.Contains(','))
                    {
                        strNumbers = userEntering.Split(new char[] { ',' });
                        for (int i = 0; i < strNumbers.Length; i++)
                        {
                            meetingsNumbers.Add(Convert.ToInt32(strNumbers[i]));
                        }
                    }
                    else
                    {
                        meetingsNumbers.Add(Convert.ToInt32(userEntering));
                    }

                    //сортируем распознанные номера встреч
                    meetingsNumbers.Sort();

                    List<Meeting> meetingsToFile = new List<Meeting>();
                    List<Meeting> meetingsFromFile = new List<Meeting>();
                    int writingCount;

                    using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                    {

                        XmlSerializer meetingSerializer = new XmlSerializer(typeof(List<Meeting>));
                        meetingsFromFile = (List<Meeting>)meetingSerializer.Deserialize(fstream);

                    }

                    // проверяем, сколько цифр распознано. если их 2, то скорее всего пользователь ввел "1-12" а следовательно
                    // узнаем сколько именно встреч пользователь выбрал
                    if (meetingsNumbers.Count == 2)
                    {
                        writingCount = meetingsNumbers[1] - meetingsNumbers[0];
                        for (int i = 0; i < writingCount; i++)
                        {
                            meetingsToFile.Add(meetingsFromFile[meetingsNumbers[0] + i]);
                        }
                    }
                    //если счетчик = 1 значит выбрана всего 1 встреча находим ее, и пишем в лист на импорт
                    if (meetingsNumbers.Count == 1)
                    {
                        meetingsToFile.Add(meetingsFromFile[meetingsNumbers[0] - 1]);
                    }
                    //если счетчик больше 2, значит нужны отдельные встречи
                    if (meetingsNumbers.Count > 2)
                    {
                        writingCount = meetingsNumbers.Count;
                        for (int i = 0; i < writingCount; i++)
                        {
                            meetingsToFile.Add(meetingsFromFile[meetingsNumbers[i]]);
                        }
                    }

                    int fileCount = 0;
                    List<string> linesFromFile = new List<string>();
                    using (FileStream fstream = new FileStream(importFilePath, FileMode.OpenOrCreate))
                    {
                        using (StreamReader sr = new StreamReader(fstream))
                        {
                            while (!sr.EndOfStream)
                            {
                                linesFromFile.Add(sr.ReadLine());
                            }
                            fileCount = linesFromFile.Count;
                        }
                    }

                    // задаем формат записи встречи в файл и записываем встречу
                    // так же делаем проверку, имеется ли дата окончания или же нет, если нет, то не пишем ее
                    using (FileStream fileStream = new FileStream(importFilePath, FileMode.Append))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            foreach (Meeting meet in meetingsToFile)
                            {
                                fileCount++;
                                if (meet.EndTime != new TimeSpan(00, 00, 00))
                                {
                                    streamWriter.WriteLine("{0}| Название встречи:{1} Дата всречи:{2} Начало встречи:{3} Окончание встречи:{4}", fileCount, meet.Name, meet.Date.ToString("d", cultureInfo), meet.BeginTime.ToString(@"hh\:mm"), meet.EndTime.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    streamWriter.WriteLine("{0}| Название встречи:{1} Дата всречи:{2} Начало встречи:{3}", fileCount, meet.Name, meet.Date.ToString("d", cultureInfo), meet.BeginTime.ToString(@"hh\:mm"));
                                }
                            }
                            Console.WriteLine("=================================\n| Встречи импортированы в файл! |\n=================================");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Встречи не обнаружены");
                }

            }
            catch (Exception error)
            {
                Console.WriteLine("=====================\n| ПРОИЗОШЛА ОШИБКА! |\n=====================\nОШИБКА: {0}", error.Message);
            }
        }
        /*
         * Печать текущей даты
         */
        protected void PrintCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            Console.WriteLine("\t==========================\n\t|Текущая дата: {0}|\n\t==========================", currentDate.ToShortDateString());
        }
        /*
         *     ==============================
         *      \\ Вспомогательные методы  //
         *       \\         VVV           //
         *       ===========================
         */

        /*
         *  Ограничения ввода пользователя
         */
        protected string TakeTextMethod()
        {
            string takenText = "";
            bool enter = false;
            while (!enter)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case ('9'):
                        Console.Write('9');
                        takenText = string.Concat(takenText, '9');
                        break;
                    case '8':
                        Console.Write('8');
                        takenText = string.Concat(takenText, '8');
                        break;
                    case '7':
                        Console.Write('7');
                        takenText = string.Concat(takenText, '7');
                        break;
                    case '6':
                        Console.Write('6');
                        takenText = string.Concat(takenText, '6');
                        break;
                    case '5':
                        Console.Write('5');
                        takenText = string.Concat(takenText, '5');
                        break;
                    case '4':
                        Console.Write('4');
                        takenText = string.Concat(takenText, '4');
                        break;
                    case '3':
                        Console.Write('3');
                        takenText = string.Concat(takenText, '3');
                        break;
                    case '2':
                        Console.Write('2');
                        takenText = string.Concat(takenText, '2');
                        break;
                    case '1':
                        Console.Write('1');
                        takenText = string.Concat(takenText, '1');
                        break;
                    case '0':
                        Console.Write('0');
                        takenText = string.Concat(takenText, '0');
                        break;
                    case '-':
                        Console.Write('-');
                        takenText = string.Concat(takenText, '-');
                        break;
                    case ',':
                        Console.Write(',');
                        takenText = string.Concat(takenText, ',');
                        break;
                    case (char)ConsoleKey.Enter:
                        enter = true;
                        break;
                    case (char)ConsoleKey.Backspace:
                        Backspace();
                        break;
                }
            }
            return takenText;
        }
        /*
         * Метод для ввода в меню
         * ограничивает ввод пользователя цифрами
         */
        protected string TakeTextMenu()
        {
            string takenText = "";
            bool enter = false;
            while (!enter)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case ('9'):
                        Console.Write('9');
                        takenText = string.Concat(takenText, '9');
                        break;
                    case '8':
                        Console.Write('8');
                        takenText = string.Concat(takenText, '8');
                        break;
                    case '7':
                        Console.Write('7');
                        takenText = string.Concat(takenText, '7');
                        break;
                    case '6':
                        Console.Write('6');
                        takenText = string.Concat(takenText, '6');
                        break;
                    case '5':
                        Console.Write('5');
                        takenText = string.Concat(takenText, '5');
                        break;
                    case '4':
                        Console.Write('4');
                        takenText = string.Concat(takenText, '4');
                        break;
                    case '3':
                        Console.Write('3');
                        takenText = string.Concat(takenText, '3');
                        break;
                    case '2':
                        Console.Write('2');
                        takenText = string.Concat(takenText, '2');
                        break;
                    case '1':
                        Console.Write('1');
                        takenText = string.Concat(takenText, '1');
                        break;
                    case '0':
                        Console.Write('0');
                        takenText = string.Concat(takenText, '0');
                        break;
                    case (char)ConsoleKey.Enter:
                        enter = true;
                        break;
                    case (char)ConsoleKey.Backspace:
                        Backspace();
                        break;
                }
            }
            return takenText;
        }
        /*
         * стирание символов
         */
        protected void Backspace()
        {
            if (Console.CursorLeft == 0)
                return;

            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            Console.Write(" ");
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
