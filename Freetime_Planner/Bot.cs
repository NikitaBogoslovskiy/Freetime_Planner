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
using static Freetime_Planner.Modes.Mode;
using VkNet.Model.Keyboard;
using static Freetime_Planner.Modes;
using VkNet.Model.Template;
using VkNet.Enums.SafetyEnums;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Yandex.Music.Api;
using RestSharp;

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
        /// Объект для работы с сообществом через VkApi
        /// </summary>
        public static VkApi vkapi_service;
        public static VkApi vkapi_main;

        /// <summary>
        /// Объект для работы с пользовательским аккаунтом через VkApi
        /// </summary>
        public static VkApi private_vkapi;

        /// <summary>
        /// Объект для работы с Яндекс.Музыка
        /// </summary>
        public static YandexMusicApi yandex_api;

        /// <summary>
        /// Поле, хранящее токен авторизации бота
        /// </summary>
        public static string _access_token_main;
        public static string _access_token_service;

        /// <summary>
        /// Поле, хранящее пользовательский токен
        /// </summary>
        public static string _private_access_token;

        /// <summary>
        /// Поле, хранящее логин к аккаунту в Яндексе
        /// </summary>
        public static string _yandex_login;

        /// <summary>
        /// Поле, хранящее пароль к аккаунту в Яндексе
        /// </summary>
        public static string _yandex_password;

        /// <summary>
        /// Поле, хранящее логин к аккаунту в ВКонтакте
        /// </summary>
        public static string _vk_login;

        /// <summary>
        /// Поле, хранящее пароль к аккаунту в ВКонтакте
        /// </summary>
        public static string _vk_password;

        /// <summary>
        /// Поле, хранящее ключ доступа к англоязычной базе данных с фильмами
        /// </summary>
        public static string _mdb_key;

        /// <summary>
        /// Поле, хранящее ключ доступа к Кинопоиску
        /// </summary>
        public static string _kp_key;

        /// <summary>
        /// Поле, хранящее ключ доступа к API Youtube
        /// </summary>
        public static string _youtube_key;

        /// <summary>
        /// Поле, хранящее ключ доступа к API Google
        /// </summary>
        public static string _google_key;

        /// <summary>
        /// Поле, хранящее id поисковой системы Google
        /// </summary>
        public static string _google_sid;

        public static string _google_sid_series;

        public static string _spotify_client_id;

        public static string _spotify_client_secret;

        public static string _spotify_token;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены служебные изображения
        /// </summary>
        //public static long album_id = 277695979;
        public static long album_id = 273234101;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены изображения актеров
        /// </summary>
        //public static long album_id_actors = 280879039;
        //public static long album_id_actors = 273234101;
        public static long album_id_actors = 279214092;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры популярных фильмов
        /// </summary>
        //public static long album_id_popular = 278759103;
        //public static long album_id_popular = 273234101;
        public static long album_id_popular = 279214123;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры популярных сериалов
        /// </summary>
        //public static long album_id_popular_tv = 278837885;
        //public static long album_id_popular_tv = 273234101;
        public static long album_id_popular_tv = 279214108;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры рекомендованных фильмов
        /// </summary>
        //public static long album_id_recommended = 278816822;
        //public static long album_id_recommended = 273234101;
        public static long album_id_recommended = 279214120;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры рекомендованных сериалов
        /// </summary>
        //public static long album_id_recommended_tv = 278839233;
        //public static long album_id_recommended_tv = 273234101;
        public static long album_id_recommended_tv = 279214104;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры фильмов из результатов поисковой выдачи
        /// </summary>
        //public static long album_id_results = 278816830;
        //public static long album_id_results = 273234101;
        public static long album_id_results = 279214116;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры сериалов из результатов поисковой выдачи
        /// </summary>
        //public static long album_id_results_tv = 278840696;
        //public static long album_id_results_tv = 273234101;
        public static long album_id_results_tv = 279214099;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры фильмов из выдачи случайных фильмов
        /// </summary>
        //public static long album_id_random = 278816835;
        //public static long album_id_random = 273234101;
        //public static long album_id_random = 279214111;
        public static long album_id_random = 279214100;
        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры сериалов из выдачи случайных сериалов
        /// </summary>
        //public static long album_id_random_tv = 278840440;
        //public static long album_id_random_tv = 273234101;
        public static long album_id_random_tv = 279214111;

        public static long album_id_mailing = 279215287;

        public static string defaultPosterID = "-196898018_457258902";
        public static Photo defaultPoster;
        /// <summary>
        /// ID группы ВКонтакте
        /// </summary>
        public static long group_id_main = 199604726;
        //public static long group_id = 196898018;
        public static long group_id_service = 196898018;
        //public static long group_id_main = 204471838;
        /// <summary>
        /// Поле, хранящее пользователя, с которым бот ведет диалог в данный момент времени
        /// </summary>
        //public static User user;

        /// <summary>
        /// Поле, хранящее актуальное сообщение текущего пользователя (его данные хранятся в поле User)
        /// </summary>
        //public static Message message;

        /// <summary>
        /// Поле, хранящее клавиатуру для отправки в диалог с текущим пользователем
        /// </summary>
        //public static MessageKeyboard keyboard;

        /// <summary>
        /// Поле, хранящее карусель для отправки в диалог с текущим пользователем
        /// </summary>
        //public static MessageTemplate template;

        /// <summary>
        /// Поле, хранящее вложения для отправки в диалог с текущим пользователем
        /// </summary>
        //public static List<MediaAttachment> attachments;

        /// <summary>
        /// Текст капчи, который ввел пользователь
        /// </summary>
        public static string captcha_key;

        /// <summary>
        /// ID капчи, которая была отправлена пользователю
        /// </summary>
        public static long? captcha_sid;


        //public static bool? IsMobileVersion;

        public static Stopwatch timer = new Stopwatch();

        public static string directory;

        public static Random GlobalRandom = new Random();



        //Функции региона MainArea

        /// <summary>
        /// Конструктор класса Bot, внутри которого выполняются все подготовительные действия
        /// </summary>
        public Bot()
        {
            Init();
            AccessTokens.Upload();
            WritelnColor("Токены и ключи доступа загружены", ConsoleColor.Green);
            Users.Upload();
            WritelnColor("Пользователи загружены", ConsoleColor.Green);
            Keyboards.Init();
            WritelnColor("Клавиатуры инициализированы", ConsoleColor.Green);
            ServiceClass.UploadServiceData();
            WritelnColor("Сервисные данные загружены", ConsoleColor.Green);
            Food.UploadGenreFood();
            WritelnColor("Файл с жанрами и едой загружен", ConsoleColor.Green);
            timer.Start();
        }

        /// <summary>
        /// Инициализация объекта класса VkApi
        /// </summary>
        public static void Init()
        {
            Title = "Freetime Planner";
            WritelnColor("Bot", ConsoleColor.Yellow);
            var service = new ServiceCollection();
            service.AddAudioBypass();
            vkapi_main = new VkApi(service);
            vkapi_service = new VkApi();
            private_vkapi = new VkApi(service);
            yandex_api = new YandexMusicApi();
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
                vkapi_main.Authorize(new ApiAuthParams { AccessToken = _access_token_main });
                vkapi_service.Authorize(new ApiAuthParams { AccessToken = _access_token_service });
                private_vkapi.Authorize(new ApiAuthParams
                {
                    Login = _vk_login,
                    Password = _vk_password
                });
                yandex_api.Authorize(_yandex_login, _yandex_password);
                defaultPoster = private_vkapi.Photo.GetById(new string[] { Bot.defaultPosterID })[0];

                WritelnColor("Загрузка популярных фильмов...", ConsoleColor.White);
                Film.UploadPopularFilms();
                WritelnColor("Список популярных фильмов загружен", ConsoleColor.Green);
                WritelnColor("Загрузка популярных сериалов...", ConsoleColor.White);
                TV.UploadPopularTV();
                WritelnColor("Список популярных сериалов загружен", ConsoleColor.Green);
                WritelnColor("Загрузка случайных фильмов...", ConsoleColor.White);
                Film.UploadRandomFilms();
                WritelnColor("Список случайных фильмов загружен", ConsoleColor.Green);
                WritelnColor("Загрузка случайных сериалов...", ConsoleColor.White);
                TV.UploadRandomTV();
                WritelnColor("Список случайных сериалов загружен", ConsoleColor.Green);
                UpdateSpotifyToken();
                WritelnColor("Загрузка рассылки по фильмам...", ConsoleColor.White);
                Film.UploadPopularFilmsQueue();
                WritelnColor("Рассылка по фильмам загружена", ConsoleColor.Green);
                WritelnColor("Загрузка фильмов по жанрам...", ConsoleColor.White);
                Film.UploadGenreFilms();
                WritelnColor("Фильмы по жанрам успешно загружены", ConsoleColor.Green);
                WritelnColor("Загрузка сериалов по жанрам...", ConsoleColor.White);
                TV.UploadGenreTV();
                WritelnColor("Сериалы по жанрам успешно загружены", ConsoleColor.Green);
                InitTimers();
                WritelnColor("Таймеры запущены", ConsoleColor.Green);

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
            var response = vkapi_main.Groups.GetLongPollServer((ulong)Bot.group_id_main);
            Key = response.Key;
            Ts_g = response.Ts;
            Server = response.Server;
            var m_response = vkapi_main.Messages.GetLongPollServer(true);
            //Ts_m = m_response.Ts;
            Pts = m_response.Pts;
            //Eye();
            EyeAsync();
            WritelnColor("Запросов в секунду доступно: " + vkapi_main.RequestsPerSecond, ConsoleColor.White);
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
        public static void SendMessage(User user, string message, MessageKeyboard keyboard = null, MessageTemplate template = null, IEnumerable<MediaAttachment> attachments = null)
        {
            try
            {
                vkapi_main.Messages.Send(new MessagesSendParams
                {
                    UserId = user.ID,
                    Message = message,
                    RandomId = GlobalRandom.Next(),
                    Keyboard = keyboard,
                    Template = template,
                    Attachments = attachments
                });
                WriteLine($"Успешно отправлен ответ: {message}\nВремя между сообщениями = {timer.ElapsedMilliseconds / 1000.0}");
                timer.Restart();
                Console.Beep();
            }
            catch (Exception e)
            {
                WriteLine("Ошибка! " + e.Message);
                WriteLine("Путь к ошибке: " + e.StackTrace);
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
        static string Key;
        static string Ts_g;
        //static string Ts_m;
        static string Server;
        static ulong? Pts;
        /*static bool IsActive;
        static Timer WatchTimer = null;
        static byte MaxSleepSteps = 3;
        static int StepSleepTime = 333;
        static byte CurrentSleepSteps = 1;
        delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<VkNet.Model.Message> messages);
        static event MessagesRecievedDelegate NewMessages;*/

        static async void EyeAsync()
        {
            await Task.Run(() => Eye());
        }

        static void Eye()
        {
            while (true)
            {
                try
                {
                    //var response = vkapi_main.Groups.GetLongPollServer((ulong)Bot.group_id_main);
                    var history = vkapi_main.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                    {
                        Key = Key,
                        Ts = Ts_g,
                        Server = Server,
                        Wait = 0
                    });
                    if (history == null || history.Updates.Count() == 0)
                        continue;
                    foreach (var m in history.Updates.Where(u => u.Type == GroupUpdateType.MessageNew))
                    {
                        var m1 = m.MessageNew.Message;
                        if (m1.Attachments.Count == 1 && m1.Attachments[0].Instance is AudioMessage)
                        {
                            bool f = true;
                            while (f)
                            {
                                //var m_response = vkapi_main.Messages.GetLongPollServer(true);
                                var m_history = vkapi_main.Messages.GetLongPollHistory(new MessagesGetLongPollHistoryParams
                                {
                                    Ts = ulong.Parse(Ts_g),
                                    Pts = Pts
                                });
                                m1 = m_history.Messages.Last();
                                f = (m1.Attachments[0].Instance as AudioMessage).TranscriptState == TranscriptStates.InProgress;
                            }
                        }
                        if (m1.Type == MessageType.Received)
                        {
                            GetMessageAsync(m1, m.MessageNew.ClientInfo);
                        }
                    }
                    Ts_g = history.Ts;
                    Thread.Sleep(340);
                }
                catch (Exception)
                {
                    var response = vkapi_main.Groups.GetLongPollServer((ulong)Bot.group_id_main);
                    Key = response.Key;
                    Ts_g = response.Ts;
                    Server = response.Server;
                    var m_response = vkapi_main.Messages.GetLongPollServer(true);
                    //Ts_m = m_response.Ts;
                    Pts = m_response.Pts;
                }
            }
        }

        static async void GetMessageAsync(Message message, ClientInfo info)
        {
            await Task.Run(() => GetMessage(message, info));
        }

        static void GetMessage(Message message, ClientInfo info)
        {
            if (message.Type != MessageType.Sended)
            {
                VkNet.Model.User Sender = vkapi_main.Users.Get(new long[] { message.PeerId.Value }, ProfileFields.Online)[0];
                bool b = info.InlineKeyboard;
                bool? IsMobileVersion = b;
                //bool? IsMobileVersion = false;
                var user = Users.GetUser(Sender, out bool IsOld);
                //SendMessage(user, "Answer");
                if (message.Attachments.Count != 0)
                {
                    if (message.Attachments[0].Instance is AudioMessage am)
                    {
                        if (am.TranscriptState == TranscriptStates.InProgress)
                        {
                            //SendMessage("Обрабатываю голосовое сообщение...");
                            return;
                        }
                        if (user.CurrentLevel() == Mode.Film || user.CurrentLevel() == Mode.TV || user.CurrentLevel() == Mode.Search || user.CurrentLevel() == Mode.SearchGenre)
                        {
                            message.Text = am.Transcript;
                            if (user.CurrentLevel() == Mode.SearchGenre)
                            {
                                user.RemoveLevel();
                                if (user.CurrentLevel() == Mode.Film)
                                    SendMessage(user, "Возврат к меню фильмов...", Keyboards.FilmKeyboard);
                                else
                                    SendMessage(user, "Возврат к меню сериалов...", Keyboards.TVKeyboard);
                            }
                            if (user.CurrentLevel() != Mode.Search)
                                user.AddLevel(Mode.Search);
                        }
                        else
                        {
                            SendMessage(user, "Голосовые сообщения я понимаю только в вкладках 'Фильмы' и 'Сериалы': с помощью голосовых ты можешь искать фильмы и сериалы по названию");
                            return;
                        }
                    }
                    else if (IsOld)
                    {
                        SendMessage(user, "Мне тяжело понимать любые медивложения (фото, видео, аудиозаписи, стикеры), кроме голосовых сообщений: их ты можешь использовать во вкладках" +
                            " 'Фильмы' и 'Сериалы' для поиска фильмов и сериалов по названию");
                        return;
                    }
                }

                WriteLine($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {message.Text}");
                if (!IsOld)
                {
                    SendMessage(user, $"Привет, {user.Name}! Я Freetime Planner - чат-бот Вконтакте, помогающий подобрать фильм, сериал или еду для просмотра. Ниже тебе уже доступны кнопки, " +
                        $"с помоью которых и будет проходить наше общение 🙃. Одна из них - 'Помощь', нажав на которую ты узнаешь обо мне немного больше. Приятного досуга!", Keyboards.MainKeyboard);
                    return;
                }
                //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                CommandCentre(user, message, IsMobileVersion);
            }
        }

        /// <summary>
        /// Функция, отслеживающая входящие сообщения
        /// </summary>
        /*static void Eye()
        {
            LongPollServerResponse Pool = vkapi_main.Messages.GetLongPollServer(true);
            StartAsync(Pool.Ts, Pool.Pts);
            NewMessages += _NewMessages;
            vkapi_main.OnTokenExpires += _Logout;
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
            LongPollServerResponse response = vkapi_main.Messages.GetLongPollServer(lastPts == null);
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
                NewMessages?.Invoke(vkapi_main, history.Messages);
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
                    history = vkapi_main.Messages.GetLongPollHistory(rp);
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
                    m.FromId = m.Type == MessageType.Sended ? vkapi_main.UserId : m.UserId;
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
                    var message = messages[i];
                    VkNet.Model.User Sender = vkapi_main.Users.Get(new long[] { message.PeerId.Value }, ProfileFields.Online)[0];
                    bool? IsMobileVersion = false;
                    var user = Users.GetUser(Sender, out bool IsOld);
                    if (message.Attachments.Count != 0)
                    {
                        if (message.Attachments[0].Instance is AudioMessage am)
                        {
                            if (am.TranscriptState == TranscriptStates.InProgress)
                            {
                                //SendMessage("Обрабатываю голосовое сообщение...");
                                continue;
                            }
                            if (user.CurrentLevel() == Mode.Film || user.CurrentLevel() == Mode.TV || user.CurrentLevel() == Mode.Search)
                            {
                                message.Text = am.Transcript;
                                if (user.CurrentLevel() != Mode.Search)
                                    user.AddLevel(Mode.Search);
                            }
                            else
                            {
                                SendMessage(user, "Голосовые сообщения я понимаю только в вкладках 'Фильмы' и 'Сериалы': с помощью голосовых ты можешь искать фильмы и сериалы по названию");
                                continue;
                            }
                        }
                        else if (IsOld)
                        {
                            SendMessage(user, "Мне тяжело понимать любые медивложения (фото, видео, аудиозаписи, стикеры), кроме голосовых сообщений: их ты можешь использовать во вкладках" +
                                " 'Фильмы' и 'Сериалы' для поиска фильмов и сериалов по названию");
                            continue;
                        }
                    }

                    WriteLine($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {messages[i].Text}");
                    if (!IsOld)
                    {
                        SendMessage(user, $"Привет, {user.Name}! Я Freetime Planner - чат-бот Вконтакте, помогающий подобрать фильм, сериал или еду для просмотра. Ниже тебе уже доступны кнопки, " +
                            $"с помоью которых и будет проходить наше общение 🙃. Одна из них - 'Помощь', нажав на которую ты узнаешь обо мне немного больше. Приятного досуга!", Keyboards.MainKeyboard);
                        continue;
                    }
                    CommandCentre(user, message, IsMobileVersion);
                }
            }
        }*/

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

        public static string MainHelp;

        public static string FilmHelp;

        public static string TVHelp;

        public static string FoodHelp;

        public static string OptionsHelp;

        /// <summary>
        /// Командный центр, определяющий уровень пользователя и реакцию на его сообщение
        /// </summary>
        static void CommandCentre(User user, Message message, bool? IsMobileVersion)
        {
            var p = new Payload(message.Payload);
            string payload = p.text;
            switch (user.Level.Count)
            {
                case 1:
                    try
                    {
                        /*if (payload != null && payload != "Command")
                        {
                            if (p.type == "f")
                                SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь в основном меню. Перейди во вкладку" +
                                " 'Фильмы' и повтори нажатие кнопки");
                            else if (p.type == "t")
                                SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь в основном меню. Перейди во вкладку" +
                                " 'Сериалы' и повтори нажатие кнопки");
                            else if (p.type == "o")
                                SendMessage("Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                            break;
                        }*/
                        var level = MainMenu(message.Text);
                        if (payload == null)
                        {
                            SendMessage(user, "Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки", Keyboards.MainKeyboard);
                            break;
                        }
                        user.AddLevel(level);
                    }
                    catch (ArgumentException e)
                    {
                        //keyboard = Keyboards.MainKeyboard;
                        SendMessage(user, e.Message, Keyboards.MainKeyboard);
                        break;
                    }

                    if (user.CurrentLevel() == Mode.Film)
                    {
                        //keyboard = Keyboards.FilmKeyboard;
                        SendMessage(user, "Выбери режим обзора фильмов", Keyboards.FilmKeyboard);
                    }
                    else if (user.CurrentLevel() == Mode.TV)
                    {
                        //keyboard = Keyboards.TVKeyboard;
                        SendMessage(user, "Выбери режим обзора сериалов", Keyboards.TVKeyboard);
                    }
                    else if (user.CurrentLevel() == Mode.Food)
                    {
                        //keyboard = Keyboards.FoodKeyboard;
                        SendMessage(user, "Выбери тип еды под просмотр", Keyboards.FoodKeyboard);
                    }
                    else if (user.CurrentLevel() == Mode.Help)
                    {
                        SendMessage(user, MainHelp);
                        user.RemoveLevel();
                    }
                    else if (user.CurrentLevel() == Mode.Options)
                    {
                        //keyboard = Keyboards.Options();
                        SendMessage(user, "Выбери настройку", Keyboards.Options());
                    }
                    break;

                case 2:
                    Mode previous_level = user.CurrentLevel();
                    try
                    {
                        var level = SecondMenu(message.Text);
                        if (payload == null)
                        {
                            SendMessage(user, "Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки", LevelKeyboard(user.CurrentLevel()));
                            break;
                        }
                        else
                            user.AddLevel(level);
                    }
                    catch (ArgumentException e)
                    {
                        SendMessage(user, e.Message, LevelKeyboard(user.CurrentLevel()));
                        break;
                    }

                    if (previous_level == Mode.Film)
                    {
                        if (payload != "Command")
                        {
                            if (p.type != "f")
                            {
                                if (p.type == "t")
                                    SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Фильмы'. Перейди во вкладку" +
                                    " 'Сериалы' и повтори нажатие кнопки");
                                else if (p.type == "o")
                                    SendMessage(user, "Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                                user.RemoveLevel();
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                //"Посмотрел"
                                case Watched:
                                    user.HideFilm(int.Parse(p.filmId));
                                    user.RemovePlannedFilm(p.filmId);
                                    //keyboard = Keyboards.FilmWatched(p.nameEn, p.filmId);
                                    SendMessage(user, "Понравился фильм?", Keyboards.FilmWatched(p.nameEn, p.filmId));
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Хочу посмотреть"
                                case WantToWatch:
                                    if (user.AddPlannedFilm(p.nameRu, p.nameEn, p.date, p.filmId))
                                    {
                                        if (user.MailFunction && User.StringToDate(p.date).CompareTo(DateTime.Now) < 0)
                                            user.AddMailObjectAsync(p.filmId, true, p.nameRu, p.nameEn, p.date).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                                        SendMessage(user, "Добавлено в список планируемых фильмов");
                                    }
                                    else
                                        SendMessage(user, "Фильм уже есть в списке планируемых, я о нем помню 😉");   
                                    user.RemoveLevel();
                                    break;


                                //"Актеры"
                                case Actors:
                                    SendMessage(user, "Формирую список актеров...");                                       
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));

                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                    {
                                        MessageTemplate filmmetod = null;
                                        if (!user.GetFilmActors(p.filmId, ref filmmetod))
                                            SendMessage(user, "К сожалению, для этого сериала я не смог ничего найти... 😔");
                                        else
                                            SendMessage(user, "Результаты поиска", null, filmmetod);
                                    }
                                    else {
                                        user.GetFilmActors(p.filmId);
                                    }
                                    user.RemoveLevel();
                                    break;

                                //"Узнать больше"
                                case MoreAboutActor:
                                    SendMessage(user,"Ищу информацию по этому актеру...");
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    var m = Film.Methods.ActorDescriptionMessage(user, p.filmId, out var Actora);
                                    SendMessage(user,m,null,null,Actora);
                                    user.RemoveLevel();
                                    break;

                               

                                //"Саундтрек"
                                case Soundtrack:
                                    SendMessage(user, "Собираю трек-лист...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    List<Audio> audios = new List<Audio>();
                                    string name, addition;
                                    if (p.nameEn != string.Empty)
                                    {
                                        name = p.nameEn;
                                        addition = "ost";
                                    }
                                    else
                                    {
                                        name = p.nameRu;
                                        addition = "саундтрек";
                                    }
                                    //addition = p.date.Substring(0, 4);
                                    if (!user.FilmSoundtrack(name, addition, ref audios))
                                    {
                                        SendMessage(user, "К сожалению, для этого фильма я не смог ничего найти... 😔");
                                        //break;
                                    }
                                    else
                                    //var attachments = audios.Select(a => a as MediaAttachment).ToList();
                                        SendMessage(user, "", null, null, audios.Select(a => a as MediaAttachment));
                                    //attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Еда"
                                case GenreFood:
                                    SendMessage(user, "Подбираю блюдо для данного фильма...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    var video = Film.Methods.Food(p.genres.Split('*'), user);
                                    if (video != null)
                                    {
                                        //attachments = new List<MediaAttachment> { video as MediaAttachment };
                                        SendMessage(user, "", null, null, new List<MediaAttachment> { video as MediaAttachment });
                                    }
                                    else
                                        SendMessage(user, "При загрузке видео-рецепта что-то произошло... 😔 Попробуй повторно выполнить запрос");
                                    //attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Не показывать"
                                case BlackList:
                                    user.HideFilm(int.Parse(p.filmId));
                                    user.RemovePlannedFilm(p.filmId);
                                    SendMessage(user, "Добавлено в список нежелаемых фильмов");
                                    user.RemoveLevel();
                                    break;

                                //"Подробнее"
                                case More:
                                    SendMessage(user, "Готовлю детали по фильму...");
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));

                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        user.AddFilmActorsAsync(p.filmId).ContinueWith(t => Console.WriteLine(t.Exception),TaskContinuationOptions.OnlyOnFaulted);
                                    else
                                        user.MessageAddFilmActorsAsync(p.filmId).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted); ;

                                    if (user.FilmRecommendations.TryGetValue(int.Parse(p.filmId), out Film.FilmObject film))
                                    {
                                        if (film.data.nameEn != null && film.data.nameEn != string.Empty)
                                            user.AddFilmSoundtrackAsync(film.data.nameEn, "ost").ContinueWith(t => Console.WriteLine(t.Exception),TaskContinuationOptions.OnlyOnFaulted);
                                        else
                                            user.AddFilmSoundtrackAsync(film.data.nameRu, "саундтрек").ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                                        //attachments = new List<MediaAttachment> { Attachments.PosterObject(film.data.posterUrl, film.data.filmId.ToString()) };
                                        //keyboard = Keyboards.FilmSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), film.data.premiereRu ?? film.data.premiereWorld ?? film.data.year, string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereDigital ?? film.data.premiereDvd);
                                        SendMessage(user, 
                                            Film.Methods.FullInfo(film),
                                            Keyboards.FilmSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), film.data.premiereRu ?? film.data.premiereWorld ?? film.data.year, string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereDigital ?? film.data.premiereDvd),
                                            null,
                                            private_vkapi.Photo.GetById(new string[] { film.data.VKPhotoID_2 }));
                                    }
                                    else
                                    {
                                        var answer = Film.Methods.FullInfo(user, int.Parse(p.filmId), out var k, out var a);
                                        SendMessage(user, answer, k, null, a);
                                        //attachments и keyboard присваиваются внутри функции FullInfo
                                    }
                                    WriteLine($"Конец вызова Подробнее: {timer.ElapsedMilliseconds}");
                                    user.RemoveLevel();
                                    break;

                                //"Уже посмотрел"
                                case AlreadyWatched:
                                    SendMessage(user, "Введи порядковый номер просмотренного кинофильма");
                                    break;

                                //"Да" (посмотрел, понравилось)
                                case Yes:
                                    user.LikeFilm(p.nameEn);
                                    if (user.MailFunction)
                                        user.AddMailObjectAsync(p.filmId, false).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted); ;
                                    SendMessage(user, "Круто! Буду советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                //"Нет" (посмотрел, не понравилось)
                                case No:
                                    SendMessage(user, "Жаль... Буду стараться предлагать более интересные фильмы");
                                    user.RemoveLevel();
                                    break;

                                //"Где посмотреть"
                                case WhereToWatch:
                                    if (ServiceClass.service_data.google_requests < 100)
                                    {
                                        SendMessage(user, "Ищу места для просмотра фильма...");
                                        var keyboard = Film.Methods.ServiceLinks(p.nameRu, p.date);
                                        if (keyboard == null)
                                            SendMessage(user, "К сожалению, я не смог найти места для просмотра... 😔");
                                        else
                                        {
                                            SendMessage(user, "Жми одну из кнопок и смотри!", keyboard);
                                            //keyboard = null;
                                        }
                                    }
                                    else
                                        SendMessage(user, "К сожалению, я не смог найти места для просмотра... 😔");
                                    user.RemoveLevel();
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //кнопки меню
                            switch (user.CurrentLevel())
                            {
                                //"Поиск по названию"
                                case Search:
                                    //keyboard = null;
                                    SendMessage(user, "Введи название фильма");
                                    break;
                                
                                //Поиск по жанрам
                                case SearchGenre:
                                    SendMessage(user,"Выбери жанр", Keyboards.GenresKeyboard, null,null);
                                    break;

                                //"Мои рекомендации"-------------------------------------------
                                case Recommendations:
                                    SendMessage(user, "Составляю список рекомендаций...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetFilmRecommendations(), null);
                                    else
                                        user.GetFilmRecommendationsMessage(user); //отправка сообщения внутри
                                    user.RemoveLevel();
                                        break;
                                   
                                //"Планирую посмотреть"
                                case PlanToWatch:
                                    //keyboard = Keyboards.FilmPlanToWatch();
                                    SendMessage(user, user.GetPlannedFilms(), Keyboards.FilmPlanToWatch());
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Рандомный фильм"  ------------------------
                                case Modes.Mode.Random:
                                    SendMessage(user, "Ищу случайные фильмы...");
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.RandomFilms());
                                    else
                                        user.RandomFilmsMessage(user); //отправка сообщения внутри
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    //keyboard = Keyboards.MainKeyboard;
                                    SendMessage(user, "Выбери один из режимов", Keyboards.MainKeyboard);
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(user, FilmHelp);
                                    user.RemoveLevel();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if (previous_level == Mode.TV)
                    {
                        if (payload != "Command")
                        {
                            if (p.type != "t")
                            {
                                if (p.type == "f")
                                    SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Сериалы'. Перейди во вкладку" +
                                    " 'Фильмы' и повтори нажатие кнопки");
                                else if (p.type == "o")
                                    SendMessage(user, "Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                                user.RemoveLevel();
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                //"Посмотрел"
                                case Watched:
                                    user.HideTV(int.Parse(p.filmId));
                                    user.RemovePlannedTV(p.filmId);
                                    //keyboard = Keyboards.TVWatched(p.nameEn, p.filmId);
                                    SendMessage(user, "Понравился сериал?", Keyboards.TVWatched(p.nameEn, p.filmId));
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Хочу посмотреть"
                                case WantToWatch:
                                    if (user.AddPlannedTV(p.nameRu, p.nameEn, p.filmId))
                                        SendMessage(user, "Добавлено в список планируемых сериалов");
                                    else
                                        SendMessage(user, "Сериал уже есть в списке планируемых, я о нем помню 😉");
                                    user.RemoveLevel();
                                    break;

                                //"Актеры"
                                case Actors:
                                    SendMessage(user, "Формирую список актеров...");
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    MessageTemplate tvmetod = null;
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                    {
                                        if (!user.GetTVActors(p.filmId, ref tvmetod))
                                            SendMessage(user, "К сожалению, для этого сериала я не смог ничего найти... 😔");
                                        else
                                            SendMessage(user, "Результаты поиска", null, tvmetod);
                                    }
                                    else
                                    {
                                        user.GetTVActors(p.filmId);
                                    }
                                    user.RemoveLevel();
                                    break;

                              
                                //"Узнать больше"
                                case MoreAboutActor:
                                    SendMessage(user, "Ищу информацию по этому актеру...");
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    var m = TV.Methods.ActorDescriptionMessageTV(user, p.filmId, out var Actora);
                                    SendMessage(user, m, null, null, Actora);
                                    user.RemoveLevel();
                                    break;

                                //"Саундтрек"
                                case Soundtrack:
                                    SendMessage(user, "Собираю трек-лист...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    List<Audio> audios = new List<Audio>();
                                    string name, addition;
                                    if (p.nameEn != string.Empty)
                                    {
                                        name = p.nameEn;
                                        addition = "ost";
                                    }
                                    else
                                    {
                                        name = p.nameRu;
                                        addition = "саундтрек";
                                    }
                                    //addition = p.date.Substring(0, 4);
                                    if (!user.TVSoundtrack(name, addition, ref audios))
                                    {
                                        SendMessage(user, "К сожалению, для этого сериала я не смог ничего найти... 😔");
                                        //break;
                                    }
                                    else
                                    //attachments = audios.Select(a => a as MediaAttachment).ToList();
                                        SendMessage(user, "", null, null, audios.Select(a => a as MediaAttachment));
                                    //attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Еда"
                                case GenreFood:
                                    SendMessage(user, "Подбираю блюдо для данного сериала...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    var video = TV.Methods.Food(p.genres.Split('*'), user);
                                    if (video != null)
                                    {
                                        //attachments = new List<MediaAttachment> { video as MediaAttachment };
                                        SendMessage(user, "", null, null, new List<MediaAttachment> { video as MediaAttachment });
                                    }
                                    else
                                        SendMessage(user, "При загрузке видео-рецепта что-то произошло... 😔 Попробуй повторно выполнить запрос");
                                    //attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Не показывать"
                                case BlackList:
                                    user.HideTV(int.Parse(p.filmId));
                                    user.RemovePlannedTV(p.filmId);
                                    SendMessage(user, "Добавлено в список нежелаемых сериалов");
                                    user.RemoveLevel();
                                    break;

                                //"Подробнее"
                                case More:
                                    SendMessage(user, "Готовлю детали по сериалу...");
                                    //vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));

                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        user.AddTVActorsAsync(p.filmId).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                                    else
                                        user.MessageAddTVActorsAsync(p.filmId).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

                                    if (user.TVRecommendations.TryGetValue(int.Parse(p.filmId), out TV.TVObject tv))
                                    {
                                        if (tv.data.nameEn != null && tv.data.nameEn != string.Empty)
                                            user.AddTVSoundtrackAsync(tv.data.nameEn, "ost").ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                                        else
                                            user.AddTVSoundtrackAsync(tv.data.nameRu, "саундтрек").ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                                        //attachments = new List<MediaAttachment> { Attachments.PosterObject(tv.data.posterUrl, tv.data.filmId.ToString()) };
                                        //keyboard = Keyboards.TVSearch(tv.data.nameRu, tv.data.nameEn, tv.data.filmId.ToString(), string.Join("*", tv.data.genres.Select(g => g.genre)), tv.data.premiereRu);
                                        SendMessage(user, 
                                            TV.Methods.FullInfo(tv),
                                            Keyboards.TVSearch(tv.data.nameRu, tv.data.nameEn, tv.data.filmId.ToString(), string.Join("*", tv.data.genres.Select(g => g.genre)), tv.data.premiereRu),
                                            null,
                                            private_vkapi.Photo.GetById(new string[] { tv.data.VKPhotoID_2 }));
                                    }
                                    else
                                    {
                                        var answer = TV.Methods.FullInfo(user, int.Parse(p.filmId), out var k, out var a);
                                        SendMessage(user, answer, k, null, a);
                                        //attachments и keyboard присваиваются внутри функции FullInfo
                                    }
                                    //keyboard = null;
                                    //attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Уже посмотрел"
                                case AlreadyWatched:
                                    SendMessage(user, "Введи порядковый номер просмотренного сериала");
                                    break;

                                //"Да" (посмотрел, понравилось)
                                case Yes:
                                    user.LikeTV(p.nameEn);
                                    if (user.MailFunction)
                                        user.AddMailObjectAsync(p.filmId, false).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted); ;
                                    SendMessage(user, "Круто! Буду советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                //"Нет" (посмотрел, не понравилось)
                                case No:
                                    SendMessage(user, "Жаль... Буду стараться предлагать более интересные сериалы");
                                    user.RemoveLevel();
                                    break;

                                //"Где посмотреть"
                                case WhereToWatch:
                                    if (ServiceClass.service_data.google_requests < 100)
                                    {
                                        SendMessage(user, "Ищу места для просмотра сериала...");
                                        var keyboard = TV.Methods.ServiceLinks(p.nameRu, p.date);
                                        if (keyboard == null)
                                            SendMessage(user, "К сожалению, я не смог найти места для просмотра... 😔");
                                        else
                                        {
                                            SendMessage(user, "Жми одну из кнопок кнопку и смотри!", keyboard);
                                            //keyboard = null;
                                        }
                                    }
                                    else
                                        SendMessage(user, "К сожалению, я не смог найти места для просмотра... 😔");
                                    user.RemoveLevel();
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //кнопки меню
                            switch (user.CurrentLevel())
                            {
                                //"Поиск по названию"
                                case Search:
                                    //keyboard = null;
                                    SendMessage(user, "Введи название сериала");
                                    break;


                                //Поиск по жанрам
                                case SearchGenre:
                                    SendMessage(user, "Выбери жанр", Keyboards.SearchGenreList(), null, null);
                                    break;


                                //"Мои рекомендации"
                                case Recommendations:
                                    SendMessage(user, "Составляю список рекомендаций...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetTVRecommendations());
                                    else
                                        //SendMessage(user, "Рекомендуемые сериалы");
                                        user.GetTVRecommendationsMessage(user); //отправка сообщения внутри
                                    user.RemoveLevel();
                                    break;

                                //"Планирую посмотреть"
                                case PlanToWatch:
                                    SendMessage(user, user.GetPlannedTV(), Keyboards.TVPlanToWatch());
                                    user.RemoveLevel();
                                    break;

                                //"Рандомный сериал"
                                case Modes.Mode.Random:
                                   SendMessage(user, "Ищу случайные сериалы...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.RandomTV());
                                    else
                                        user.RandomTVMessage(user); //отправка сообщения внутри
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    //keyboard = Keyboards.MainKeyboard;
                                    SendMessage(user, "Выбери один из режимов", Keyboards.MainKeyboard);
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(user, TVHelp);
                                    user.RemoveLevel();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if (previous_level == Mode.Food)
                    {
                        if (payload != "Command")
                        {
                            if (p.type == "f")
                                SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Еда под просмотр'. Перейди во вкладку" +
                                " 'Фильмы' и повтори нажатие кнопки");
                            else if (p.type == "t")
                                SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Еда под просмотр'. Перейди во вкладку" +
                                " 'Сериалы' и повтори нажатие кнопки");
                            else if (p.type == "o")
                                SendMessage(user, "Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                            user.RemoveLevel();
                            break;
                        }
                        switch (user.CurrentLevel())
                        {
                            //"Закуски"
                            case Snack:
                                SendMessage(user, "Подбираю закуску...");
                                vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                var video1 = Food.Snack(user);
                                if (video1 != null)
                                {
                                    //attachments = new List<MediaAttachment> { video1 as MediaAttachment };
                                    SendMessage(user, "", null, null, new List<MediaAttachment> { video1 as MediaAttachment });
                                }
                                else
                                    SendMessage(user, "При загрузке видео-рецепта что-то произошло... 😔 Попробуй повторно выполнить запрос");
                                //attachments = null;
                                break;

                            //"Сладкое"
                            case Dessert:
                                SendMessage(user, "Подбираю десерт...");
                                vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                var video2 = Food.Dessert(user);
                                if (video2 != null)
                                {
                                    //attachments = new List<MediaAttachment> { video2 as MediaAttachment };
                                    SendMessage(user, "", null, null, new List<MediaAttachment> { video2 as MediaAttachment });
                                }
                                else
                                    SendMessage(user, "При загрузке видео-рецепта что-то произошло... 😔 Попробуй повторно выполнить запрос");
                                //attachments = null;
                                break;

                            //"Коктейли"
                            case Cocktails:
                                SendMessage(user, "Подбираю коктейль...");
                                vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                var video3 = Food.Cocktail(user);
                                if (video3 != null)
                                {
                                    //attachments = new List<MediaAttachment> { video3 as MediaAttachment };
                                    SendMessage(user, "", null, null, new List<MediaAttachment> { video3 as MediaAttachment });
                                }
                                else
                                    SendMessage(user, "При загрузке видео-рецепта что-то произошло... 😔 Попробуй повторно выполнить запрос");
                                //attachments = null;
                                break;

                            //"Назад"
                            case Back:
                                //keyboard = Keyboards.MainKeyboard;
                                SendMessage(user, "Выбери один из режимов", Keyboards.MainKeyboard);
                                //keyboard = null;
                                user.RemoveLevel();
                                break;

                            //"Помощь"
                            case Help:
                                SendMessage(user, FoodHelp);
                                break;

                        }
                        user.RemoveLevel();
                    }
                    else if (previous_level == Mode.Options)
                    {
                        if (payload != "Command")
                        {
                            if (p.type != "o")
                            {
                                if (p.type == "f")
                                    SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Настройки'. Перейди во вкладку" +
                                    " 'Фильмы' и повтори нажатие кнопки");
                                else if (p.type == "t")
                                    SendMessage(user, "Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Настройки'. Перейди во вкладку" +
                                " 'Сериалы' и повтори нажатие кнопки");
                                user.RemoveLevel();
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                //"Ежедневно"
                                case Everyday:
                                    user.MailFunction = true;
                                    user.DaysGap = 1;
                                    SendMessage(user, "Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в три дня"
                                case ThreeDays:
                                    user.MailFunction = true;
                                    user.DaysGap = 3;
                                    SendMessage(user, "Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в пять дней"
                                case FiveDays:
                                    user.MailFunction = true;
                                    user.DaysGap = 5;
                                    SendMessage(user, "Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в неделю"
                                case EveryWeek:
                                    user.MailFunction = true;
                                    user.DaysGap = 7;
                                    SendMessage(user, "Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Без рассылки"
                                case NoMail:
                                    user.MailFunction = false;
                                    SendMessage(user, "Рассылка успешно выключена");
                                    user.RemoveLevel();
                                    break;

                                //"Без ограничений"
                                case NoLimit:
                                    user.OnlyHealthyFood = false;
                                    SendMessage(user, "Режим питания без ограничений успешно включён");
                                    user.RemoveLevel();
                                    break;

                                //"Здоровое питание"
                                case HealthyFood:
                                    user.OnlyHealthyFood = true;
                                    SendMessage(user, "Режим здорового питания успешно включён");
                                    user.RemoveLevel();
                                    break;

                            }
                        }
                        else
                        {
                            switch (user.CurrentLevel())
                            {
                                //"Частота рассылки"
                                case MailFrequency:
                                    //keyboard = Keyboards.MailFrequency();
                                    SendMessage(user, "Выбери комфортную для себя частоту рассылки:", Keyboards.MailFrequency());
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Режим диеты"
                                case DietMode:
                                    SendMessage(user, "Выбери наиболее предпочтительный тип питания", Keyboards.DietMode());
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(user, OptionsHelp);
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    //keyboard = Keyboards.MainKeyboard;
                                    SendMessage(user, "Выбери один из режимов", Keyboards.MainKeyboard);
                                    //keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;
                            }
                        }
                    }
                    break;

                case 3:
                    previous_level = user.PreviousLevel();
                    if (previous_level == Mode.Film)
                    {
                        if (!(payload == null && (user.CurrentLevel() == Search || user.CurrentLevel() == AlreadyWatched) || (payload == "Command" && user.CurrentLevel() == SearchGenre)))
                        {
                            if (user.CurrentLevel() != Search && user.CurrentLevel() != AlreadyWatched)
                                SendMessage(user, "Возврат к меню фильмов...", Keyboards.FilmKeyboard);
                            user.RemoveLevel();
                            CommandCentre(user, message, IsMobileVersion);
                            return;
                        }

                        if (payload != "Command")
                        {
                            switch (user.CurrentLevel())
                            {
                                //<название фильма> (после кнопки "Поиск по названию")
                                case Search:
                                    //keyboard = null;
                                    SendMessage(user, "Ищу фильмы по введенному названию...");
                                    vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                    {
                                        var template = Film.Methods.Search(user, message.Text);
                                        if (template == null)
                                            SendMessage(user, "К сожалению, я не смог найти такой фильм... 😔");
                                        else
                                        {
                                            SendMessage(user, "Результаты поиска", null, template);
                                            template = null;
                                        }
                                    }
                                    //not mobile
                                    else
                                    {
                                        Film.Methods.Search_inMessage(user, message.Text); //отправка сообщения внутри
                                                                                           //keyboard = null;
                                                                                           //attachments = null;
                                    }
                                    break;

                                //<порядковый номер фильма> (после кнопки "Уже посмотрел")
                                case AlreadyWatched:
                                    try
                                    {
                                        int index = int.Parse(message.Text) - 1;
                                        Film.FilmObject film = null;
                                        string eng_name = null;
                                        int id = 0;
                                        if (user.PlannedFilms[0].Count > index)
                                        {
                                            film = user.PlannedFilms[0][index];
                                            eng_name = film.data.nameEn;
                                            id = film.data.filmId;
                                            user.PlannedFilms[0].RemoveAt(index);
                                        }
                                        else
                                        {
                                            index -= user.PlannedFilms[0].Count;
                                            film = user.PlannedFilms[1][index];
                                            eng_name = film.data.nameEn;
                                            id = film.data.filmId;
                                            user.PlannedFilms[1].RemoveAt(index);
                                        }
                                        user.HideFilm(id);
                                        SendMessage(user, "Фильм перенесен в список просмотренных");
                                        //keyboard = Keyboards.FilmWatched(eng_name, id.ToString());
                                        SendMessage(user, "Понравился фильм?", Keyboards.FilmWatched(eng_name, id.ToString()));
                                        //keyboard = null;
                                    }
                                    catch (FormatException)
                                    {
                                        SendMessage(user, "По-моему, ты ввел не порядковый номер, а что-то другое. Нужно ввести число, стоящее слева от просмотренного фильма");
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        SendMessage(user, "К сожалению, я не смог найти фильм с таким номером. Попробуй вводить только те числа, которые указаны слева от фильмов");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                var level = ThirdMenu(message.Text);
                                if (payload == null)
                                {
                                    SendMessage(user, "Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки", Keyboards.GenresKeyboard);
                                    break;
                                }
                                else
                                    user.AddLevel(level);
                            }
                            catch (ArgumentException e)
                            {
                                SendMessage(user, e.Message, Keyboards.GenresKeyboard);
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                case GenreFiction:
                                    SendMessage(user, "Подбираю фантастические фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Фантастика"));
                                    else
                                        user.SendGenreFilm("Фантастика");
                                    break;
                                case GenreDetective:
                                    SendMessage(user, "Подбираю детективные фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Детектив"));
                                    else
                                        user.SendGenreFilm("Детектив");
                                    break;
                                case GenreBoevik:
                                    SendMessage(user, "Подбираю фильмы в жанре боевик...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Боевик"));
                                    else
                                        user.SendGenreFilm("Боевик");
                                    break;
                                case GenreComedy:
                                    SendMessage(user, "Подбираю комедийные фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Комедия"));
                                    else
                                        user.SendGenreFilm("Комедия");
                                    break;
                                case GenreAnime:
                                    SendMessage(user, "Подбираю аниме-фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Аниме"));
                                    else
                                        user.SendGenreFilm("Аниме");
                                    break;
                                case GenreFantasy:
                                    SendMessage(user, "Подбираю фэнтези-фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Фэнтези"));
                                    else
                                        user.SendGenreFilm("Фэнтези");
                                    break;
                                case GenreDrama:
                                    SendMessage(user, "Подбираю драматические фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Драма"));
                                    else
                                        user.SendGenreFilm("Драма");
                                    break;
                                case GenreMilitary:
                                    SendMessage(user, "Подбираю военные фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Военный"));
                                    else
                                        user.SendGenreFilm("Военный");
                                    break;
                                case GenreThriller:
                                    SendMessage(user, "Подбираю триллеры...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Триллер"));
                                    else
                                        user.SendGenreFilm("Триллер");
                                    break;
                                case GenreCriminal:
                                    SendMessage(user, "Подбираю криминальные фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Криминал"));
                                    else
                                        user.SendGenreFilm("Криминал");
                                    break;
                                case GenreFamily:
                                    SendMessage(user, "Подбираю семейные фильмы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Семейный"));
                                    else
                                        user.SendGenreFilm("Семейный");
                                    break;
                                case GenreHoror:
                                    SendMessage(user, "Подбираю фильмы ужасов...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreFilms("Ужасы"));
                                    else
                                        user.SendGenreFilm("Ужасы");
                                    break;
                                case Back:
                                    SendMessage(user, "Выбери режим обзора фильмов", Keyboards.FilmKeyboard);
                                    user.RemoveLevel();
                                    break;
                            }
                        }
                        user.RemoveLevel();
                    }
                    else if (previous_level == Mode.TV)
                    {
                        if (!(payload == null && (user.CurrentLevel() == Search || user.CurrentLevel() == AlreadyWatched) || (payload == "Command" && user.CurrentLevel() == SearchGenre)))
                        {
                            if (user.CurrentLevel() != Search && user.CurrentLevel() != AlreadyWatched)
                                SendMessage(user, "Возврат к меню сериалов...", Keyboards.TVKeyboard);
                            user.RemoveLevel();
                            CommandCentre(user, message, IsMobileVersion);
                            return;
                        }

                        if (payload != "Command")
                        {
                            switch (user.CurrentLevel())
                            {
                                //<название сериала> (после кнопки "Поиск по названию")
                                case Search:
                                    //keyboard = null;
                                    SendMessage(user, "Ищу сериалы по введенному названию...");
                                    vkapi_main.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id_main.ToString()));
                                    if (IsMobileVersion.HasValue && IsMobileVersion.Value)
                                    {
                                        var template = TV.Methods.Search(user, message.Text);
                                        if (template == null)
                                            SendMessage(user, "К сожалению, я не смог найти такой сериал... 😔");
                                        else
                                        {
                                            SendMessage(user, "Результаты поиска", null, template);
                                            //template = null;
                                        }
                                    }//not mobile
                                    else
                                    {
                                        TV.Methods.Search_inMessage(user, message.Text); //отправка сообщения внутри
                                                                                         //keyboard = null;
                                                                                         //attachments = null;
                                    }
                                    break;

                                //<порядковый номер сериала> (после кнопки "Уже посмотрел")
                                case AlreadyWatched:
                                    try
                                    {
                                        int index = int.Parse(message.Text) - 1;
                                        TV.TVObject film = null;
                                        string eng_name = null;
                                        int id = 0;
                                        film = user.PlannedTV[index];
                                        eng_name = film.data.nameEn;
                                        id = film.data.filmId;
                                        user.PlannedTV.RemoveAt(index);
                                        user.HideTV(id);
                                        SendMessage(user, "Сериал перенесен в список просмотренных");
                                        //keyboard = Keyboards.TVWatched(eng_name, id.ToString());
                                        SendMessage(user, "Понравился сериал?", Keyboards.TVWatched(eng_name, id.ToString()));
                                        //keyboard = null;
                                    }
                                    catch (FormatException)
                                    {
                                        SendMessage(user, "По-моему, ты ввел не порядковый номер, а что-то другое. Попробуй ввести число, стоящее слева от просмотренного сериала");
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        SendMessage(user, "К сожалению, я не смог найти сериал с таким номером. Попробуй вводить только те числа, которые указаны слева от сериалов");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                var level = ThirdMenu(message.Text);
                                if (payload == null)
                                {
                                    SendMessage(user, "Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки", Keyboards.GenresKeyboard);
                                    break;
                                }
                                else
                                    user.AddLevel(level);
                            }
                            catch (ArgumentException e)
                            {
                                SendMessage(user, e.Message, Keyboards.GenresKeyboard);
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                case GenreFiction:
                                    SendMessage(user, "Подбираю фантастические сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Фантастика"));
                                    else
                                        user.SendGenreTV("Фантастика");
                                    break;
                                case GenreDetective:
                                    SendMessage(user, "Подбираю детективные сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Детектив"));
                                    else
                                        user.SendGenreTV("Детектив");
                                    break;
                                case GenreBoevik:
                                    SendMessage(user, "Подбираю сериалы в жанре боевик...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Боевик"));
                                    else
                                        user.SendGenreTV("Боевик");
                                    break;
                                case GenreComedy:
                                    SendMessage(user, "Подбираю комедийные сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Комедия"));
                                    else
                                        user.SendGenreTV("Комедия");
                                    break;
                                case GenreAnime:
                                    SendMessage(user, "Подбираю аниме-сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Аниме"));
                                    else
                                        user.SendGenreTV("Аниме");
                                    break;
                                case GenreFantasy:
                                    SendMessage(user, "Подбираю фэнтези-сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Фэнтези"));
                                    else
                                        user.SendGenreTV("Фэнтези");
                                    break;
                                case GenreDrama:
                                    SendMessage(user, "Подбираю драматические сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Драма"));
                                    else
                                        user.SendGenreTV("Драма");
                                    break;
                                case GenreMilitary:
                                    SendMessage(user, "Подбираю военные сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Военный"));
                                    else
                                        user.SendGenreTV("Военный");
                                    break;
                                case GenreThriller:
                                    SendMessage(user, "Подбираю триллеры...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Триллер"));
                                    else
                                        user.SendGenreTV("Триллер");
                                    break;
                                case GenreCriminal:
                                    SendMessage(user, "Подбираю криминальные сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Криминал"));
                                    else
                                        user.SendGenreTV("Криминал");
                                    break;
                                case GenreFamily:
                                    SendMessage(user, "Подбираю семейные сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Семейный"));
                                    else
                                        user.SendGenreTV("Семейный");
                                    break;
                                case GenreHoror:
                                    SendMessage(user, "Подбираю хоррор-сериалы...");
                                    if (IsMobileVersion != null && IsMobileVersion.Value)
                                        SendMessage(user, "Результаты поиска", null, user.GetGenreTV("Ужасы"));
                                    else
                                        user.SendGenreTV("Ужасы");
                                    break;
                                case Back:
                                    SendMessage(user, "Выбери режим обзора фильмов", Keyboards.FilmKeyboard);
                                    user.RemoveLevel();
                                    break;
                            }
                        }
                        user.RemoveLevel();
                    }
                    break;

                default:
                    break;  //доделать дефолт

            }
        }
        #endregion

        //Регион, где создаются таймеры
        #region Timers

        public static Timer ResetTimer;
        public static int RTinterval = 180000; //3 минуты - интервал проверки бездействия пользователя
        public static long reset_time = 600000; //10 минут - критическое время бездействия
        public static object synclock = new object();

        public static Timer PopularFilmsTimer;
        public static int PFTinterval = 30; //24 часа - интервал проверки 
        public static int update_time = 7; //7 дней - срок, после которого обновляются популярные фильмы и сериалы
        public static object PFTsynclock = new object();

        public static Timer OneHourTimer;
        public static int PlFTinterval = 30000; //1 час - интервал проверки
      
        public static int day_time = 0; //0 - час в сутках, в который обновляются планируемые фильмы (т.е. в диапозоне 0:00-0:59)
        public static object PlFTsynclock = new object();

        public static Timer TokenTimer;
        public static int TTinterval = 3300000; //55 минут - интервал проверки 
        public static object TTsynclock = new object();

        /// <summary>
        /// Инициализация таймеров
        /// </summary>
        public static void InitTimers()
        {
            ResetTimer = new Timer(new TimerCallback(Reset), null, 0, RTinterval); //таймер, вызывающий каждый интервал времени RTinterval функцию Reset
            PopularFilmsTimer = new Timer(new TimerCallback(RegularPopularFilmsUpdating), null, 0, PFTinterval); //таймер, вызывающий каждый интервал времени PFTinterval функцию RegularPopularFilmsUpdating
            OneHourTimer = new Timer(new TimerCallback(OneHourChecker), null, 0, PlFTinterval); //таймер, вызывающий каждый интервал времени PlFTinterval функцию DailyPlannedFilmsUpdating
            TokenTimer = new Timer(new TimerCallback(TokenUpdator), null, 0, TTinterval);
        }

        /// <summary>
        /// Функция, вызывающая функцию ResetLevel в случае, если истекло время из поля reset_time
        /// </summary>
        /// <param name="obj"></param>
        public static void Reset(object obj)
        {
            lock (synclock)
            {
                foreach (var u in Users.Users_Dict.Values.ToList())
                {
                    if (TimeIsUp(u, reset_time) && u.CurrentLevel() != Mode.Default)
                        u.ResetLevel();
                    foreach (var elem in u.FilmTracks)
                        if (DateTime.Now.CompareTo(elem.Value.DownloadTime.AddMinutes(10)) > 0)
                            u.FilmTracks.Remove(elem.Key);
                    foreach (var elem in u.TVTracks)
                        if (DateTime.Now.CompareTo(elem.Value.DownloadTime.AddMinutes(10)) > 0)
                            u.TVTracks.Remove(elem.Key);
                    foreach (var elem in u.FilmActors)
                        if (DateTime.Now.CompareTo(elem.Value.DownloadTime.AddMinutes(10)) > 0)
                            u.FilmActors.Remove(elem.Key);
                    foreach (var elem in u.TVActors)
                        if (DateTime.Now.CompareTo(elem.Value.DownloadTime.AddMinutes(10)) > 0)
                            u.FilmActors.Remove(elem.Key);
                    Users.Unload();
                }
            }
        }

        /// <summary>
        /// Функция, обновляющая списки популярных фильмов и сериалов каждую неделю
        /// </summary>
        /// <param name="obj"></param>
        public static void RegularPopularFilmsUpdating(object obj)
        {
            lock (PFTsynclock)
            {
                if (DateTime.Now.CompareTo(Film.LastPopularFilmsUpdate.AddDays(update_time)) != -1)
                {
                    Film.UpdatePopularFilms();
                    Film.LastPopularFilmsUpdate = DateTime.Now;
                    Film.UnloadPopularFilms();
                    Film.UpdatePopularFilmsQueue();
                    Film.UnloadPopularFilmsQueue();
                    Film.UpdateGenreFilms();
                    Film.UnloadGenreFilms();
                }
                if (DateTime.Now.CompareTo(TV.LastPopularTVUpdate.AddDays(update_time)) != -1)
                {
                    TV.UpdatePopularTV();
                    TV.LastPopularTVUpdate = DateTime.Now;
                    TV.UnloadPopularTV();
                    TV.UpdateGenreTV();
                    TV.UnloadGenreTV();
                }
                if (DateTime.Now.CompareTo(Film.LastRandomFilmsUpdate.AddDays(1)) != -1)
                {
                    Film.UpdateRandomFilms();
                    Film.LastRandomFilmsUpdate = DateTime.Now;
                    Film.UnloadRandomFilms();
                }
                if (DateTime.Now.CompareTo(TV.LastRandomTVUpdate.AddDays(1)) != -1)
                {
                    TV.UpdateRandomTV();
                    TV.LastRandomTVUpdate = DateTime.Now;
                    TV.UnloadRandomTV();
                }
                foreach (var u in Users.Users_Dict.Values)
                    foreach (var t in u.Posters)
                        if (DateTime.Now.CompareTo(t.Value.Item2) > 0 && t.Value.Item1 != null)
                        {
                            var ids = t.Value.Item1.Split('_');
                            private_vkapi.Photo.Delete(ulong.Parse(ids[1]), long.Parse(ids[0]));
                            u.Posters.Remove(t.Key, out var value);
                        }
                Users.Unload();
            }
        }

        /// <summary>
        /// Функция, которая вызывается таймером каждый час
        /// </summary>
        /// <param name="obj"></param>
        public static void OneHourChecker(object obj)
        {
            lock (PlFTsynclock)
            {
                //Обновление списка планируемых фильмов и проверка наличия новых трейлеров, если после последнего обновления прошло более суток
                foreach (var user in Users.Users_Dict.Values.ToList())
                {
                    if (user.LastPlannedFilmsUpdate.AddDays(1).CompareTo(DateTime.Now) <= 0)
                    {
                        user.UpdatePlannedFilms();
                        foreach (var film in user.PlannedFilms[1])
                            film.UpdateTrailer();
                    }
                }
                Users.Unload();

                //Рассылка вышедших трейлеров, если сейчас 11:00-11:59
                if (DateTime.Now.Hour == 11)
                {
                    foreach (var user in Users.Users_Dict.Values.ToList())
                    {
                        foreach(var film in user.PlannedFilms[1].Where(f => f.Trailer.IsNew))
                        {
                            //attachments = new List<MediaAttachment> { film.Trailer.Trailer };
                            //var previous_user = Bot.user;
                            //Bot.user = user;
                            SendMessage(user, $"🔥 Новый трейлер! 🔥\n\n{film.data.nameRu ?? film.data.nameEn} ({film.data.premiereRu.Substring(0, 4)})", null, null, new List<MediaAttachment> { film.Trailer.Trailer });
                            //Bot.user = previous_user;
                            film.Trailer.IsNew = false;
                        }
                    }
                    //attachments = null;
                    Users.Unload();
                }

                //Напоминание пойти в кино за две недели, если сейчас 19:00-19:59
                if (DateTime.Now.Hour == 19)
                {
                    foreach (var u in Users.Users_Dict.Values.ToList())
                    {
                        string message = "Напоминаю, что следующие фильмы уже совсем скоро выходят в кино:\n\n";
                        var en = u.PlannedFilms[1].Where(f => f.data.premiereRu.Length != 4).Where(f => !f.TwoWeeksNotification).Where(f => DateTime.Now.AddDays(14).CompareTo(User.StringToDate(f.data.premiereRu)) >= 0);
                        if (en.Count() == 0)
                            continue;
                        message += string.Join("\n", en.Select(f => $"{f.data.nameRu ?? f.data.nameEn} ({Film.Methods.ChangeDateType(f.data.premiereRu)})"));
                        //var previous_user = user;
                        //user = u;
                        SendMessage(u, message);
                        //user = previous_user;
                        foreach (var film in en)
                            film.TwoWeeksNotification = true;
                    }
                    Users.Unload();
                }

                //Напоминание пойти в кино за день, если сейчас 20:00-20:59
                if (DateTime.Now.Hour == 20)
                {
                    foreach (var u in Users.Users_Dict.Values.ToList())
                    {
                        string message = "Напоминаю, что следующие фильмы выходят в кино уже завтра:\n\n";
                        var en = u.PlannedFilms[1].Where(f => f.data.premiereRu.Length != 4).Where(f => !f.PremiereNotification).Where(f => DateTime.Now.AddDays(1).CompareTo(User.StringToDate(f.data.premiereRu)) >= 0);
                        if (en.Count() == 0)
                            continue;
                        message += string.Join("\n", en.Select(f => $"{f.data.nameRu ?? f.data.nameEn} ({Film.Methods.ChangeDateType(f.data.premiereRu)})"));
                        //var previous_user = user;
                        //user = u;
                        SendMessage(u, message);
                        //user = previous_user;
                        foreach (var film in en)
                            film.PremiereNotification = true;
                    }
                    Users.Unload();
                }

                //ежесуточный сброс числа запросов к гуглу
                if (DateTime.Now.CompareTo(ServiceClass.service_data.last_update.AddHours(24)) >= 0)
                    ServiceClass.service_data.ResetGoogleRequests();

                //рассылка кадров, фактов и саундтрека
                if (3 <= DateTime.Now.Hour && DateTime.Now.Hour <= 23)
                {
                    var r = new Random();
                    foreach (var p in Users.Users_Dict.Values.Where(u => u.MailFunction).ToList())
                    {
                        while (DateTime.Now.CompareTo(p.NextMail) >= 0)
                        {
                            Mailing.MailObject mail;
                            bool IsPopular = false;
                            if (p.MailObjects.Count == 0)
                            {
                                mail = Film.PopularFilmsQueue.Peek();
                                IsPopular = true;
                            }
                            else
                                mail = p.MailObjects.Dequeue();
                            string message = $"{mail.Name} ({mail.Year})";
                            List<MediaAttachment> attachments = null;
                            if (mail.IsTrailer)
                            {
                                if (!IsPopular && !p.PlannedFilms[0].Select(f => f.data.filmId.ToString()).Contains(mail.id))
                                    continue;
                                attachments = new List<MediaAttachment> { mail.Trailer };
                            }
                            else
                            {
                                if (IsPopular && p.PlannedFilms[0].Select(f => f.data.filmId.ToString()).Contains(mail.id))
                                {
                                    var f = Film.PopularFilmsQueue.Dequeue();
                                    Film.PopularFilmsQueue.Enqueue(f);
                                    Film.UnloadPopularFilmsQueue();
                                    continue;
                                }
                                attachments = new List<MediaAttachment>();
                                if (IsPopular)
                                    attachments.AddRange(private_vkapi.Photo.GetById(mail.PostersIds));
                                else
                                    attachments.AddRange(mail.Posters.Select(p => p as MediaAttachment));
                                if (mail.SoundTrack != null && mail.SoundTrack.Count != 0)
                                    attachments.AddRange(mail.SoundTrack.Select(s => s as MediaAttachment));
                                if (mail.Facts != null)
                                {
                                    message += "\n\n";
                                    message += string.Join("\n", mail.Facts.Select(f => $"✅ {f}"));
                                }
                            }
                            //var previous_user = user;
                            //user = p;
                            SendMessage(p, message, null, null, attachments);
                            //user = previous_user;
                            //attachments = null;
                            var next = DateTime.Now.AddDays(p.DaysGap);
                            p.NextMail = new DateTime(next.Year, next.Month, next.Day, r.Next(12, 21), 0, 0);
                        }
                    }
                    if (Film.PopularFilmsQueue.Count <= 1)
                        Film.UpdatePopularFilmsQueue();
                    else
                        Film.PopularFilmsQueue.Dequeue();
                    Film.UnloadPopularFilmsQueue();
                    Users.Unload();
                }
            }
        }

        public static void TokenUpdator(object obj)
        {
            lock(TTsynclock)
            {
                UpdateSpotifyToken();
            }
        }

        #endregion

        /// <summary>
        /// Функция, возвращающая true, если время истекло
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool TimeIsUp(User user, long critical_time) => DateTime.Now.CompareTo(user.LastTime.AddMilliseconds(critical_time)) != -1;

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static void UpdateSpotifyToken()
        {
            var client = new RestSharp.RestClient("https://accounts.spotify.com/api/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", $"Basic {Base64Encode($"{_spotify_client_id}:{_spotify_client_secret}")}");
            request.AddQueryParameter("grant_type", "client_credentials");
            var result = JsonConvert.DeserializeObject<SpotifyResponse>(client.Execute(request).Content);
            _spotify_token = result.access_token;
            TTinterval = (result.expires_in - 300) * 1000;
        }

    }
}
