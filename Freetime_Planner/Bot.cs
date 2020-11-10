using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Model.Attachments;
using static System.Console;
using VkNet.Utils;
using Newtonsoft.Json;
using VkNet.Model.Keyboard;
using static Freetime_Planner.Modes;

namespace Freetime_Planner
{
    class Bot
    {
        /*Регион, в котором проходит:
         1. Авторизация бота
         2. Включение режима отслеживания сообщений
         3. Отправка сообщений
        */
        #region MainArea

        //Поля региона MainArea

        /// <summary>
        /// Поле, представляющее объект класса VkApi
        /// </summary>
        public static VkApi vkapi;

        /// <summary>
        /// Поле, хранящее токен авторизации бота
        /// </summary>
        private static string _access_token = "e4df2dbee9e47576aa281af2718f2946d02ce327dd080e9d154a9f2e4b5fb95dff6dae77ab915a6c69191";

        /// <summary>
        /// Поле, хранящее пользователя, с которым бот ведет диалог в данный момент времени
        /// </summary>
        public static User user;

        /// <summary>
        /// Поле, хранящее актуальное сообщение текущего пользователя (его данные хранятся в поле User)
        /// </summary>
        public static string message;

        /// <summary>
        /// Поле, хранящее клавиатуру для отправки в диалог с текущим пользователем
        /// </summary>
        public static MessageKeyboard keyboard;

        //Функции региона MainArea

        /// <summary>
        /// Конструктор класса Bot, внутри которого выполняются все подготовительные действия
        /// </summary>
        public Bot()  
        {
            Init();
            Users.Upload();
            InitResetTimer();
        }

        /// <summary>
        /// Инициализация объекта класса VkApi
        /// </summary>
        public static void Init()
        {
            Title = "Freetime Planner";
            WritelnColor("Bot", ConsoleColor.Yellow);
            vkapi = new VkApi();
        }

        /// <summary>
        /// Авторизация бота на сервере Вконтакте
        /// </summary>
        /// <returns></returns>
        public static bool Authorize()
        {
            Bot VK = new Bot();
            try
            {
                WritelnColor("Попытка авторизации...", ConsoleColor.White);
                vkapi.Authorize(new ApiAuthParams { AccessToken = _access_token });
                Start();
                return true;
            }
            catch (Exception e)
            {
                Console.Beep();
                WritelnColor("Авторизция не удалась", ConsoleColor.Red);
                WriteLine($"Сообщение об ошибке: {e.Message}");
                WriteLine($"Путь к ошибке: {e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Запуск режима отслеживания сообщений
        /// </summary>
        private static void Start()
        {
            WritelnColor("Авторизция успешно завершена", ConsoleColor.Green);
            Console.Beep();
            WritelnColor("Включаю режим отслеживания...", ConsoleColor.White);
            Eye();
            WritelnColor("Запросов в секунду доступно: " + vkapi.RequestsPerSecond, ConsoleColor.White);
        }

        /// <summary>
        /// Функция перезагрузки бота
        /// </summary>
        public static void Restart()
        {
            ExitActions();
            Process.Start((Process.GetCurrentProcess()).ProcessName);
            Environment.Exit(0);
        }

        /// <summary>
        /// Функция, отправляющая сообщение
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage(string message)
        {
            try
            {
                vkapi.Messages.Send(new MessagesSendParams
                {
                    UserId = user.ID,
                    Message = message,
                    RandomId = DateTime.Now.Millisecond,
                    Keyboard = keyboard
                });
                WritelnColor($"Успешно отправлен ответ: {message}", ConsoleColor.DarkBlue);
                Console.Beep();
            }
            catch (Exception e)
            {
                WriteLine("Ошибка! " + e.Message);
            }
        }

        /// <summary>
        /// Вспомогательная функция, выводящая текст на консоль в определенном цвете
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        static void WritelnColor(string text, ConsoleColor color)
        {
            ForegroundColor = color;
            WriteLine(text);
            ForegroundColor = ConsoleColor.Gray;
        }
        #endregion

        /*Регион, в котором:
         1. Отслеживаются входящие сообщения
         2. Идентифицируется текущий пользователь
         3. Передается сообщение пользователя в командный центр
        */
        #region Watcher
        static string Ts;
        static ulong? Pts;
        static bool IsActive;
        static Timer WatchTimer = null;
        static byte MaxSleepSteps = 3;
        static int StepSleepTime = 333;
        static byte CurrentSleepSteps = 1;
        delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<VkNet.Model.Message> messages);
        static event MessagesRecievedDelegate NewMessages;

        /// <summary>
        /// Функция, отслеживающая входящие сообщения
        /// </summary>
        static void Eye()
        {
            LongPollServerResponse Pool = vkapi.Messages.GetLongPollServer(true);
            StartAsync(Pool.Ts, Pool.Pts);
            NewMessages += _NewMessages;
            vkapi.OnTokenExpires += _Logout;
            WritelnColor("Слежение за сообщениями успешно активировано", ConsoleColor.Green);
            Console.Beep();
        }

        //Субрегион, в котором творятся СТРАШНЫЕ вещи. Не лезь - убьет!
        #region MessagesCatching
        static async void StartAsync(string lastTs = null, ulong? lastPts = null)
        {
            if (IsActive) WriteLine("Messages for {0} already watching");
            IsActive = true;
            await GetLongPoolServerAsync(lastPts);
            WatchTimer = new Timer(new TimerCallback(WatchAsync), null, 0, Timeout.Infinite);
        }
        static Task<LongPollServerResponse> GetLongPoolServerAsync(ulong? lastPts = null)
        {
            return Task.Run(() =>
            {
                return GetLongPoolServer(lastPts);
            });
        }
        static LongPollServerResponse GetLongPoolServer(ulong? lastPts = null)
        {
            LongPollServerResponse response = vkapi.Messages.GetLongPollServer(lastPts == null);
            Ts = response.Ts;
            Pts = Pts == null ? response.Pts : lastPts;
            return response;
        }
        static async void WatchAsync(object state)
        {
            LongPollHistoryResponse history = await GetLongPoolHistoryAsync();
            if (history.Messages.Count > 0)
            {
                CurrentSleepSteps = 1;
                NewMessages?.Invoke(vkapi, history.Messages);
            }
            else if (CurrentSleepSteps < MaxSleepSteps) CurrentSleepSteps++;
            WatchTimer.Change(CurrentSleepSteps * StepSleepTime, Timeout.Infinite);
        }
        static Task<LongPollHistoryResponse> GetLongPoolHistoryAsync()
        {
            return Task.Run(() => { return GetLongPoolHistory(); });
        }
        static LongPollHistoryResponse GetLongPoolHistory()
        {
            if (Ts == null) GetLongPoolServer(null);
            MessagesGetLongPollHistoryParams rp = new MessagesGetLongPollHistoryParams();
            rp.Ts = ulong.Parse(Ts);
            rp.Pts = Pts;
            int i = 0;
            LongPollHistoryResponse history = null;
            string errorLog = "";

            while (i < 5 && history == null)
            {
                i++;
                try
                {
                    history = vkapi.Messages.GetLongPollHistory(rp);
                }
                catch (TooManyRequestsException)
                {
                    Thread.Sleep(150);
                    i--;
                }
                catch (Exception ex)
                {
                    errorLog += string.Format("{0} - {1}{2}", i, ex.Message, Environment.NewLine);
                }
            }

            if (history != null)
            {
                Pts = history.NewPts;
                foreach (var m in history.Messages)
                {
                    m.FromId = m.Type == MessageType.Sended ? vkapi.UserId : m.UserId;
                }
            }
            else WriteLine(errorLog);
            return history;
        }
        #endregion

        /// <summary>
        /// Функция, определяющая пользователя, который прислал сообщение, и передающая его сообщение в CommandCentre
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messages"></param>
        static void _NewMessages(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type != MessageType.Sended)
                {
                    message = messages[i].Text;
                    VkNet.Model.User Sender = vkapi.Users.Get(new long[] { messages[i].PeerId.Value })[0];
                    user = Users.GetUser(Sender);
                    WritelnColor($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {messages[i].Text}", ConsoleColor.Blue);
                    Console.Beep();
                    CommandCentre();
                }
            }
        }

        /// <summary>
        /// Действия, выполняемые перед окончанием программы
        /// </summary>
        public static void ExitActions()
        {
            Users.Unload();
        }

        /// <summary>
        /// Функция, выводящая сообщение об отключении от сервера Вконтакте
        /// </summary>
        /// <param name="owner"></param>
        static void _Logout(VkApi owner)
        {
            WriteLine("Отключение от VK...");
        }

        #endregion

        /*Регион, в котором вырабатывается реакция бота 
        на входящее сообщение от текущего пользователя (командный центр)*/
        #region Commands

        /// <summary>
        /// Командный центр, определяющий уровень пользователя и реакцию на его сообщение
        /// </summary>
        static void CommandCentre()
        {
            //Эта функция находится в разработке, в ней куча заглушек
            switch (user.Level.Count)
            {
                case 1:
                    user.AddLevel(ConvertIntoMode(message));
                    if (user.CurrentLevel() == Mode.Film)
                        SendMessage("<клавиатура с кнопками 'Поиск по названию', 'Жанры', 'Мои рекомендации', 'Планирую посмотреть', 'Рандомный фильм', 'Саундтрек фильма', 'Назад'>");
                    if (user.CurrentLevel() == Mode.TV)
                        SendMessage("<клавиатура с кнопками 'Поиск по названию', 'Жанры', 'Мои рекомендации', 'Планирую посмотреть', 'Рандомный сериал', 'Саундтрек сериала', 'Назад'>");
                    if (user.CurrentLevel() == Mode.Food)
                        SendMessage("<информация о рандомном блюде>");
                    break;
                case 2:
                    var previous_level = user.CurrentLevel();
                    var current_level = ConvertIntoMode(message);
                    if (previous_level == Mode.Film)
                    {
                        if (current_level == Mode.Random)
                        SendMessage("<информация о рандомном фильме>");
                    }
                    if (previous_level == Mode.TV)
                    {
                        if (current_level == Mode.Random)
                            SendMessage("<информация о рандомном сериале>");
                    }
                    if (current_level == Mode.Back)
                        user.RemoveLevel();
                    break;
                default:
                    SendMessage("Bot is being developed");
                    break;
            }
        }
        #endregion

        /*В этом регионе создается таймер, который переводит пользователя в состояние по умолчанию, 
          если он бездействует больше, чем определенное время*/
        #region ResetTimer

        public static Timer ResetTimer;
        public int interval = 500; //0.5 секунды - интервал проверки бездействия пользователя
        public long reset_time = 180000; //3 минуты - критическое время бездействия
        public static object synclock = new object();

        /// <summary>
        /// Инициализация таймера, который каждый интервал времени из поля interval выполняет функцию Reset
        /// </summary>
        public void InitResetTimer()
        {
            ResetTimer = new Timer(new TimerCallback(Reset), null, 0, interval);
        }

        /// <summary>
        /// Функция, вызывающая функцию ResetLevel в случае, если истекло время из поля reset_time
        /// </summary>
        /// <param name="obj"></param>
        public void Reset(object obj)
        {
            lock(synclock)
            {
                Parallel.ForEach(Users.Users_Dict, (pair) =>
                {
                    if (TimeIsUp(pair.Value))
                        pair.Value.ResetLevel();
                });
            }
        }

        /// <summary>
        /// Функция, возвращающая true, если время истекло
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool TimeIsUp(User user) => DateTime.Now.CompareTo(user.LastTime.AddMilliseconds(reset_time)) != -1;

        #endregion
    }
}
