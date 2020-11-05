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

namespace Freetime_Planner
{
    class Bot
    {
        #region MainArea
        static VkApi vkapi;
        private static string _access_token = "e4df2dbee9e47576aa281af2718f2946d02ce327dd080e9d154a9f2e4b5fb95dff6dae77ab915a6c69191";
        static long userID = 0;
        public Bot()  
        {
            Init();
        }
        public static void Init()
        {
            Title = "Freetime Planner";
            WritelnColor("Bot", ConsoleColor.Yellow);
            vkapi = new VkApi();
        }
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
        private static void Start()
        {
            WritelnColor("Авторизция успешно завершена", ConsoleColor.Green);
            Console.Beep();
            WritelnColor("Включаю режим отслеживания...", ConsoleColor.White);
            Eye();
            WritelnColor("Запросов в секунду доступно: " + vkapi.RequestsPerSecond, ConsoleColor.White);
        }
        public static void Restart()
        {
            Process.Start((Process.GetCurrentProcess()).ProcessName);
            Environment.Exit(0);
        }
        public static void SendMessage(string message)
        {
            try
            {
                vkapi.Messages.Send(new MessagesSendParams
                {
                    UserId = userID,
                    Message = message,
                    RandomId = DateTime.Now.Millisecond
                });
                WritelnColor($"Успешно отправлен ответ: {message}", ConsoleColor.DarkBlue);
                Console.Beep();
            }
            catch (Exception e)
            {
                WriteLine("Ошибка! " + e.Message);
            }
        }
        static void WritelnColor(string text, ConsoleColor color)
        {
            ForegroundColor = color;
            WriteLine(text);
            ForegroundColor = ConsoleColor.Gray;
        }
        #endregion

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

        static void Eye()
        {
            LongPollServerResponse Pool = vkapi.Messages.GetLongPollServer(true);
            StartAsync(Pool.Ts, Pool.Pts);
            NewMessages += _NewMessages;
            vkapi.OnTokenExpires += _Logout;
            WritelnColor("Слежение за сообщениями успешно активировано", ConsoleColor.Green);
            Console.Beep();
        }
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
        static void _NewMessages(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type != MessageType.Sended)
                {
                    string msg = messages[i].Text;
                    var elem = messages[i];
                    VkNet.Model.User Sender = vkapi.Users.Get(new long[] { messages[i].PeerId.Value })[0];
                    WritelnColor($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {messages[i].Text}", ConsoleColor.Blue);
                    Console.Beep();
                    userID = messages[i].PeerId.Value;
                    Command(msg);
                }
            }
        }

        static void _Logout(VkApi owner)
        {
            WriteLine("Отключение от VK...");
        }

        #endregion

        #region Commands
        static void Command(string msg)
        {
            switch (msg)
            {
                default:
                    SendMessage("Bot is being developed");
                    break;
            }
        }
        #endregion
    }
}
