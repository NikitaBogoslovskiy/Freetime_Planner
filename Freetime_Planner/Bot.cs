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
        public static VkApi vkapi;

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
        public static string _access_token;

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
        public static long album_id = 277695979;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры популярных фильмов
        /// </summary>
        public static long album_id_popular = 278759103;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры популярных сериалов
        /// </summary>
        public static long album_id_popular_tv = 278837885;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры рекомендованных фильмов
        /// </summary>
        public static long album_id_recommended = 278816822;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры рекомендованных сериалов
        /// </summary>
        public static long album_id_recommended_tv = 278839233;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры фильмов из результатов поисковой выдачи
        /// </summary>
        public static long album_id_results = 278816830;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры сериалов из результатов поисковой выдачи
        /// </summary>
        public static long album_id_results_tv = 278840696;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры фильмов из выдачи случайных фильмов
        /// </summary>
        public static long album_id_random = 278816835;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры сериалов из выдачи случайных сериалов
        /// </summary>
        public static long album_id_random_tv = 278840440;

        /// <summary>
        /// ID группы ВКонтакте
        /// </summary>
        public static long group_id = 199604726;

        /// <summary>
        /// Поле, хранящее пользователя, с которым бот ведет диалог в данный момент времени
        /// </summary>
        public static User user;

        /// <summary>
        /// Поле, хранящее актуальное сообщение текущего пользователя (его данные хранятся в поле User)
        /// </summary>
        public static Message message;

        /// <summary>
        /// Поле, хранящее клавиатуру для отправки в диалог с текущим пользователем
        /// </summary>
        public static MessageKeyboard keyboard;

        /// <summary>
        /// Поле, хранящее карусель для отправки в диалог с текущим пользователем
        /// </summary>
        public static MessageTemplate template;

        /// <summary>
        /// Поле, хранящее вложения для отправки в диалог с текущим пользователем
        /// </summary>
        public static List<MediaAttachment> attachments;

        /// <summary>
        /// Текст капчи, который ввел пользователь
        /// </summary>
        public static string captcha_key;

        /// <summary>
        /// ID капчи, которая была отправлена пользователю
        /// </summary>
        public static long? captcha_sid;


        public static bool? IsMobileVersion;

        public static Stopwatch timer = new Stopwatch();

        public static string directory;



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
            vkapi = new VkApi();
            var service = new ServiceCollection();
            service.AddAudioBypass();
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
                vkapi.Authorize(new ApiAuthParams { AccessToken = _access_token });
                private_vkapi.Authorize(new ApiAuthParams
                {
                    Login = _vk_login,
                    Password = _vk_password
                });
                yandex_api.Authorize(_yandex_login, _yandex_password);
                //yandex_api.UseWebProxy(new System.Net.WebProxy("51.13.83.79:80", true));

                WritelnColor("Загрузка популярных фильмов...", ConsoleColor.White);
                Film.UploadPopularFilms();
                WritelnColor("Список популярных фильмов загружен", ConsoleColor.Green);
                WritelnColor("Загрузка популярных сериалов...", ConsoleColor.White);
                TV.UploadPopularTV();
                WritelnColor("Список популярных сериалов загружен", ConsoleColor.Green);
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
                    message = messages[i];

                    VkNet.Model.User Sender = vkapi.Users.Get(new long[] { messages[i].PeerId.Value },ProfileFields.Online)[0];
                    IsMobileVersion = true;
                    user = Users.GetUser(Sender, out bool IsOld);
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
                                SendMessage("Голосовые сообщения я понимаю только в вкладках 'Фильмы' и 'Сериалы': с помощью голосовых ты можешь искать фильмы и сериалы по названию");
                                continue;
                            }
                        }
                        else if (IsOld)
                        {
                            SendMessage("Мне тяжело понимать любые медивложения (фото, видео, аудиозаписи, стикеры), кроме голосовых сообщений: их ты можешь использовать во вкладках" +
                                " 'Фильмы' и 'Сериалы' для поиска фильмов и сериалов по названию");
                            continue;
                        }
                    }

                    WriteLine($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {messages[i].Text}");
                    if (!IsOld)
                    {
                        keyboard = Keyboards.MainKeyboard;
                        SendMessage($"Привет, {user.Name}! Я Freetime Planner - чат-бот Вконтакте, помогающий подобрать фильм, сериал или еду для просмотра. Ниже тебе уже доступны кнопки, " +
                            $"с помоью которых и будет проходить наше общение 🙃. Одна из них - 'Помощь', нажав на которую ты узнаешь обо мне немного больше. Приятного досуга!");
                        continue;
                    }
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

        public static string MainHelp;

        public static string FilmHelp;

        public static string TVHelp;

        public static string FoodHelp;

        public static string OptionsHelp;

        /// <summary>
        /// Командный центр, определяющий уровень пользователя и реакцию на его сообщение
        /// </summary>
        static void CommandCentre()
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
                            SendMessage("Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки");
                            break;
                        }
                        user.AddLevel(level);
                    }
                    catch (ArgumentException e)
                    {
                        keyboard = Keyboards.MainKeyboard;
                        SendMessage(e.Message);
                        break;
                    }

                    if (user.CurrentLevel() == Mode.Film)
                    {
                        keyboard = Keyboards.FilmKeyboard;
                        SendMessage("Выбери режим обзора фильмов");
                    }
                    else if (user.CurrentLevel() == Mode.TV)
                    {
                        keyboard = Keyboards.TVKeyboard;
                        SendMessage("Выбери режим обзора сериалов");
                    }
                    else if (user.CurrentLevel() == Mode.Food)
                    {
                        keyboard = Keyboards.FoodKeyboard;
                        SendMessage("Выбери тип еды под просмотр");
                    }
                    else if (user.CurrentLevel() == Mode.Help)
                    {
                        SendMessage(MainHelp);
                        user.RemoveLevel();
                    }
                    else if (user.CurrentLevel() == Mode.Options)
                    {
                        keyboard = Keyboards.Options();
                        SendMessage("Выбери настройку");
                    }
                    break;

                case 2:
                    Mode previous_level = user.CurrentLevel();
                    try
                    {
                        var level = SecondMenu(message.Text);
                        if (payload == null)
                        {
                            SendMessage("Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки");
                            break;
                        }
                        else
                            user.AddLevel(level);
                    }
                    catch (ArgumentException e)
                    {
                        SendMessage(e.Message);
                        break;
                    }

                    if (previous_level == Mode.Film)
                    {
                        if (payload != "Command")
                        {
                            if (p.type != "f")
                            {
                                if (p.type == "t")
                                    SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Фильмы'. Перейди во вкладку" +
                                    " 'Сериалы' и повтори нажатие кнопки");
                                else if (p.type == "o")
                                    SendMessage("Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                                user.RemoveLevel();
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                //"Посмотрел"
                                case Watched:
                                    user.HideFilm(int.Parse(p.filmId));
                                    user.RemovePlannedFilm(p.filmId);
                                    keyboard = Keyboards.FilmWatched(p.nameEn, p.filmId);
                                    SendMessage("Понравился фильм?");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Хочу посмотреть"
                                case WantToWatch:
                                    if (user.AddPlannedFilm(p.nameRu, p.nameEn, p.date, p.filmId))
                                    {
                                        if (user.MailFunction && User.StringToDate(p.date).CompareTo(DateTime.Now) < 0)
                                            user.AddMailObjectAsync(p.filmId, true, p.nameRu, p.nameEn, p.date);
                                        SendMessage("Добавлено в список планируемых фильмов");
                                    }
                                    else
                                        SendMessage("Фильм уже есть в списке планируемых, я о нем помню 😉");   
                                    user.RemoveLevel();
                                    break;

                                //"Саундтрек"
                                case Soundtrack:
                                    SendMessage("Собираю трек-лист...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    List<Audio> audios = new List<Audio>();
                                    if (!Film.Methods.Soundtrack(p.nameEn ?? p.nameRu, p.date, audios))
                                    {
                                        SendMessage("К сожалению, для этого фильма я не смог ничего найти... 😔");
                                        break;
                                    }
                                    attachments = audios.Select(a => a as MediaAttachment).ToList();
                                    SendMessage("");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Еда"
                                case GenreFood:
                                    SendMessage("Подбираю блюдо для данного фильма...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    attachments = new List<MediaAttachment> { Film.Methods.Food(p.genres.Split('*')) as MediaAttachment };
                                    SendMessage("");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Не показывать"
                                case BlackList:
                                    user.HideFilm(int.Parse(p.filmId));
                                    user.RemovePlannedFilm(p.filmId);
                                    SendMessage("Добавлено в список нежелаемых фильмов");
                                    user.RemoveLevel();
                                    break;

                                //"Подробнее"
                                case More:
                                    var sw = new Stopwatch();
                                    SendMessage("Готовлю детали по фильму...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (user.FilmRecommendations.TryGetValue(int.Parse(p.filmId), out Film.FilmObject film))
                                    {
                                        attachments = new List<MediaAttachment> { Attachments.PosterObject(film.data.posterUrl, film.data.filmId.ToString()) };
                                        keyboard = Keyboards.FilmSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), film.data.premiereRu ?? film.data.premiereWorld ?? film.data.year, string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereDigital ?? film.data.premiereDvd);
                                        SendMessage(Film.Methods.FullInfo(film));
                                    }
                                    else
                                    {
                                        SendMessage(Film.Methods.FullInfo(int.Parse(p.filmId)));
                                        //attachments и keyboard присваиваются внутри функции FullInfo
                                    }
                                    keyboard = null;
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Уже посмотрел"
                                case AlreadyWatched:
                                    SendMessage("Введи порядковый номер просмотренного кинофильма");
                                    break;

                                //"Да" (посмотрел, понравилось)
                                case Yes:
                                    user.LikeFilm(p.nameEn);
                                    if (user.MailFunction)
                                        user.AddMailObjectAsync(p.filmId, false);
                                    SendMessage("Круто! Буду советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                //"Нет" (посмотрел, не понравилось)
                                case No:
                                    SendMessage("Жаль... Буду стараться предлагать более интересные фильмы");
                                    user.RemoveLevel();
                                    break;

                                //"Где посмотреть"
                                case WhereToWatch:
                                    if (ServiceClass.service_data.google_requests < 100)
                                    {
                                        SendMessage("Ищу места для просмотра фильма...");
                                        keyboard = Film.Methods.ServiceLinks(p.nameRu, p.date);
                                        SendMessage("Жми одну из кнопок и смотри!");
                                        keyboard = null;
                                    }
                                    else
                                        SendMessage("К сожалению, я не смог найти места для просмотра... 😔");
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
                                    keyboard = null;
                                    SendMessage("Введи название фильма");
                                    break;

                                //"Мои рекомендации"
                                case Recommendations:
                                    SendMessage("Составляю список рекомендаций...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion != null)
                                    {
                                        template = user.GetFilmRecommendations();
                                        keyboard = null;
                                        SendMessage("Рекомендуемые фильмы");
                                        template = null;
                                    }
                                    else
                                    {
                                      
                                        keyboard = null;
                                        
                                        user.GetFilmRecommendationsMessage();
                                        attachments = null;
                                        keyboard = null;
                                    }
                                    user.RemoveLevel();
                                        break;
                                   
                                //"Планирую посмотреть"
                                case PlanToWatch:
                                    keyboard = Keyboards.FilmPlanToWatch();
                                    SendMessage(user.GetPlannedFilms());
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Рандомный фильм"
                                case Modes.Mode.Random:
                                    SendMessage("Ищу случайные фильмы...");
                                    if (IsMobileVersion != null)
                                    {
                                        vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                        template = Film.Methods.Random();
                                        keyboard = null;
                                        SendMessage("Результаты поиска");
                                        template = null;
                                    }
                                    else
                                    {

                                        vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                        
                                        Film.Methods.Random_inMessage();
                                        attachments = null;
                                        keyboard = null;
                                    }
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    keyboard = Keyboards.MainKeyboard;
                                    SendMessage("Выбери один из режимов");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(FilmHelp);
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
                                    SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Сериалы'. Перейди во вкладку" +
                                    " 'Фильмы' и повтори нажатие кнопки");
                                else if (p.type == "o")
                                    SendMessage("Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                                user.RemoveLevel();
                                break;
                            }
                            switch (user.CurrentLevel())
                            {
                                //"Посмотрел"
                                case Watched:
                                    user.HideTV(int.Parse(p.filmId));
                                    user.RemovePlannedTV(p.filmId);
                                    keyboard = Keyboards.TVWatched(p.nameEn, p.filmId);
                                    SendMessage("Понравился сериал?");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Хочу посмотреть"
                                case WantToWatch:
                                    if (user.AddPlannedTV(p.nameRu, p.nameEn, p.filmId))
                                        SendMessage("Добавлено в список планируемых сериалов");
                                    else
                                        SendMessage("Сериал уже есть в списке планируемых, я о нем помню 😉");
                                    user.RemoveLevel();
                                    break;

                                //"Саундтрек"
                                case Soundtrack:
                                    SendMessage("Собираю трек-лист...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    List<Audio> audios = new List<Audio>();
                                    if (!TV.Methods.Soundtrack(p.nameEn ?? p.nameRu, audios))
                                    {
                                        SendMessage("К сожалению, для этого сериала я не смог ничего найти... 😔");
                                        break;
                                    }
                                    attachments = audios.Select(a => a as MediaAttachment).ToList();
                                    SendMessage("");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Еда"
                                case GenreFood:
                                    SendMessage("Подбираю блюдо для данного сериала...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    attachments = new List<MediaAttachment> { TV.Methods.Food(p.genres.Split('*')) as MediaAttachment };
                                    SendMessage("");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Не показывать"
                                case BlackList:
                                    user.HideTV(int.Parse(p.filmId));
                                    user.RemovePlannedTV(p.filmId);
                                    SendMessage("Добавлено в список нежелаемых сериалов");
                                    user.RemoveLevel();
                                    break;

                                //"Подробнее"
                                case More:
                                    SendMessage("Готовлю детали по сериалу...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (user.TVRecommendations.TryGetValue(int.Parse(p.filmId), out TV.TVObject tv))
                                    {
                                        attachments = new List<MediaAttachment> { Attachments.PosterObject(tv.data.posterUrl, tv.data.filmId.ToString()) };
                                        keyboard = Keyboards.TVSearch(tv.data.nameRu, tv.data.nameEn, tv.data.filmId.ToString(), string.Join("*", tv.data.genres.Select(g => g.genre)), tv.data.premiereRu);
                                        SendMessage(TV.Methods.FullInfo(tv));
                                    }
                                    else
                                    {
                                        SendMessage(TV.Methods.FullInfo(int.Parse(p.filmId)));
                                        //attachments и keyboard присваиваются внутри функции FullInfo
                                    }
                                    keyboard = null;
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Уже посмотрел"
                                case AlreadyWatched:
                                    SendMessage("Введи порядковый номер просмотренного сериала");
                                    break;

                                //"Да" (посмотрел, понравилось)
                                case Yes:
                                    user.LikeTV(p.nameEn);
                                    if (user.MailFunction)
                                        user.AddMailObjectAsync(p.filmId, false);
                                    SendMessage("Круто! Буду советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                //"Нет" (посмотрел, не понравилось)
                                case No:
                                    SendMessage("Жаль... Буду стараться предлагать более интересные сериалы");
                                    user.RemoveLevel();
                                    break;

                                //"Где посмотреть"
                                case WhereToWatch:
                                    if (ServiceClass.service_data.google_requests < 100)
                                    {
                                        SendMessage("Ищу места для просмотра сериала...");
                                        keyboard = TV.Methods.ServiceLinks(p.nameRu, p.date);
                                        if (keyboard == null)
                                            SendMessage("К сожалению, я не смог найти места для просмотра... 😔");
                                        else
                                        {
                                            SendMessage("Жми одну из кнопок кнопку и смотри!");
                                            keyboard = null;
                                        }
                                    }
                                    else
                                        SendMessage("К сожалению, я не смог найти места для просмотра... 😔");
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
                                    keyboard = null;
                                    SendMessage("Введи название сериала");
                                    break;

                                //"Мои рекомендации"
                                case Recommendations:
                                    SendMessage("Составляю список рекомендаций...");
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion != null)
                                    {
                                        template = user.GetTVRecommendations();
                                        keyboard = null;
                                        SendMessage("Рекомендуемые сериалы");
                                        template = null;
                                    }
                                    else
                                    {
                                        keyboard = null;
                                        SendMessage("Рекомендуемые сериалы");
                                        user.GetTVRecommendationsMessage();
                                        attachments = null;
                                        keyboard = null;
                                    }
                                    user.RemoveLevel();
                                    break;

                                //"Планирую посмотреть"
                                case PlanToWatch:
                                    keyboard = Keyboards.TVPlanToWatch();
                                    SendMessage(user.GetPlannedTV());
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Рандомный сериал"
                                case Modes.Mode.Random:
                                   SendMessage("Ищу случайные сериалы...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (IsMobileVersion != null)
                                    {
                                        template = TV.Methods.Random();
                                        keyboard = null;
                                        SendMessage("Результаты поиска");
                                        template = null;
                                    }
                                    else
                                    {
                                        TV.Methods.Random_inMessage();
                                        keyboard = null;
                                        attachments = null;
                                    }
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    keyboard = Keyboards.MainKeyboard;
                                    SendMessage("Выбери один из режимов");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(TVHelp);
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
                                SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Еда под просмотр'. Перейди во вкладку" +
                                " 'Фильмы' и повтори нажатие кнопки");
                            else if (p.type == "t")
                                SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Еда под просмотр'. Перейди во вкладку" +
                                " 'Сериалы' и повтори нажатие кнопки");
                            else if (p.type == "o")
                                SendMessage("Ты хочешь поменять мои настройки. Перейди во вкладку 'Настройки' и повтори нажатие кнопки");
                            user.RemoveLevel();
                            break;
                        }
                        switch (user.CurrentLevel())
                        {
                            //"Закуски"
                            case Snack:
                                SendMessage("Подбираю закуску...");
                                attachments = new List<MediaAttachment> { Food.Snack() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            //"Сладкое"
                            case Dessert:
                                SendMessage("Подбираю десерт...");
                                attachments = new List<MediaAttachment> { Food.Dessert() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            //"Коктейли"
                            case Cocktails:
                                SendMessage("Подбираю коктейль...");
                                attachments = new List<MediaAttachment> { Food.Cocktail() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            //"Назад"
                            case Back:
                                keyboard = Keyboards.MainKeyboard;
                                SendMessage("Выбери один из режимов");
                                keyboard = null;
                                user.RemoveLevel();
                                break;

                            //"Помощь"
                            case Help:
                                SendMessage(FoodHelp);
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
                                    SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для фильмов, находясь во вкладке 'Настройки'. Перейди во вкладку" +
                                    " 'Фильмы' и повтори нажатие кнопки");
                                else if (p.type == "t")
                                    SendMessage("Ты хочешь воспользоваться кнопкой, предназначенной для сериалов, находясь во вкладке 'Настройки'. Перейди во вкладку" +
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
                                    SendMessage("Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в три дня"
                                case ThreeDays:
                                    user.MailFunction = true;
                                    user.DaysGap = 3;
                                    SendMessage("Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в пять дней"
                                case FiveDays:
                                    user.MailFunction = true;
                                    user.DaysGap = 5;
                                    SendMessage("Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Раз в неделю"
                                case EveryWeek:
                                    user.MailFunction = true;
                                    user.DaysGap = 7;
                                    SendMessage("Частота рассылки успешно изменена");
                                    user.RemoveLevel();
                                    break;

                                //"Без рассылки"
                                case NoMail:
                                    user.MailFunction = false;
                                    SendMessage("Рассылка успешно выключена");
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
                                    keyboard = Keyboards.MailFrequency();
                                    SendMessage("Выбери комфортную для себя частоту рассылки:");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Помощь"
                                case Help:
                                    SendMessage(OptionsHelp);
                                    user.RemoveLevel();
                                    break;

                                //"Назад"
                                case Back:
                                    keyboard = Keyboards.MainKeyboard;
                                    SendMessage("Выбери один из режимов");
                                    keyboard = null;
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
                        if (payload != null)
                        {
                            user.RemoveLevel();
                            CommandCentre();
                            return;
                        }

                        switch (user.CurrentLevel())
                        {
                            //<название фильма> (после кнопки "Поиск по названию")
                            case Search:
                                keyboard = null;
                                SendMessage("Ищу фильмы по введенному названию...");

                                vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                if (IsMobileVersion != null)
                                {
                                    template = Film.Methods.Search(message.Text);


                                    if (template == null)
                                        SendMessage("К сожалению, я не смог найти такой фильм... 😔");
                                    else
                                    {
                                        SendMessage("Результаты поиска");
                                        template = null;
                                    }
                                }
                                //not mobile
                                else
                                {
                                    Film.Methods.Search_inMessage(message.Text);
                                    keyboard = null;
                                    attachments = null;
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
                                        film = user.PlannedFilms[1][index];
                                        eng_name = film.data.nameEn;
                                        id = film.data.filmId;
                                        user.PlannedFilms[1].RemoveAt(index);
                                    }
                                    user.HideFilm(id);
                                    SendMessage("Фильм перенесен в список просмотренных");
                                    keyboard = Keyboards.FilmWatched(eng_name, id.ToString());
                                    SendMessage("Понравился фильм?");
                                    keyboard = null;
                                }
                                catch (FormatException)
                                {
                                    SendMessage("По-моему, ты ввел не порядковый номер, а что-то другое. Нужно ввести число, стоящее слева от просмотренного фильма");
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    SendMessage("К сожалению, я не смог найти фильм с таким номером. Попробуй вводить только те числа, которые указаны слева от фильмов");
                                }
                                break;
                            default:
                                break;
                        }
                        user.RemoveLevel();

                    }
                    else if (previous_level == Mode.TV)
                    {
                        if (payload != null)
                        {
                            user.RemoveLevel();
                            CommandCentre();
                            return;
                        }

                        switch (user.CurrentLevel())
                        {
                            //<название сериала> (после кнопки "Поиск по названию")
                            case Search:
                                keyboard = null;
                                SendMessage("Ищу сериалы по введенному названию...");
                                vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                if (IsMobileVersion != null)
                                {
                                    template = TV.Methods.Search(message.Text);
                                    if (template == null)
                                        SendMessage("К сожалению, я не смог найти такой сериал... 😔");
                                    else
                                    {
                                        SendMessage("Результаты поиска");
                                        template = null;
                                    }
                                }//not mobile
                                else
                                {
                                    TV.Methods.Search_inMessage(message.Text);
                                    keyboard = null;
                                    attachments = null;
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
                                    SendMessage("Сериал перенесен в список просмотренных");
                                    keyboard = Keyboards.TVWatched(eng_name, id.ToString());
                                    SendMessage("Понравился сериал?");
                                    keyboard = null;
                                }
                                catch (FormatException)
                                {
                                    SendMessage("По-моему, ты ввел не порядковый номер, а что-то другое. Попробуй ввести число, стоящее слева от просмотренного сериала");
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    SendMessage("К сожалению, я не смог найти сериал с таким номером. Попробуй вводить только те числа, которые указаны слева от сериалов");
                                }
                                break;
                            default:
                                break;
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
        public static int RTinterval = 60000; //1 минута - интервал проверки бездействия пользователя
        public static long reset_time = 600000; //10 минут - критическое время бездействия
        public static object synclock = new object();

        public static Timer PopularFilmsTimer;
        public static int PFTinterval = 86400000; //24 часа - интервал проверки 
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
                foreach (var pair in Users.Users_Dict)
                {
                    if (TimeIsUp(pair.Value, reset_time) && pair.Value.CurrentLevel() != Mode.Default)
                        pair.Value.ResetLevel();
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
                }
                if (DateTime.Now.CompareTo(TV.LastPopularTVUpdate.AddDays(update_time)) != -1)
                {
                    TV.UpdatePopularTV();
                    TV.LastPopularTVUpdate = DateTime.Now;
                    TV.UnloadPopularTV();
                }
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
                foreach (var user in Users.Users_Dict.Values)
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
                    foreach (var user in Users.Users_Dict.Values)
                    {
                        foreach(var film in user.PlannedFilms[1].Where(f => f.Trailer.IsNew))
                        {
                            attachments = new List<MediaAttachment> { film.Trailer.Trailer };
                            var previous_user = Bot.user;
                            Bot.user = user;
                            SendMessage($"🔥 Новый трейлер! 🔥\n\n{film.data.nameRu ?? film.data.nameEn} ({film.data.premiereRu.Substring(0, 4)})");
                            Bot.user = previous_user;
                            film.Trailer.IsNew = false;
                        }
                    }
                    attachments = null;
                    Users.Unload();
                }

                //Напоминание пойти в кино за две недели, если сейчас 19:00-19:59
                if (DateTime.Now.Hour == 19)
                {
                    foreach (var u in Users.Users_Dict.Values)
                    {
                        string message = "Напоминаю, что следующие фильмы уже совсем скоро выходят в кино:\n\n";
                        var en = u.PlannedFilms[1].Where(f => f.data.premiereRu.Length != 4).Where(f => !f.TwoWeeksNotification).Where(f => DateTime.Now.AddDays(14).CompareTo(User.StringToDate(f.data.premiereRu)) >= 0);
                        if (en.Count() == 0)
                            continue;
                        message += string.Join("\n", en.Select(f => $"{f.data.nameRu ?? f.data.nameEn} ({Film.Methods.ChangeDateType(f.data.premiereRu)})"));
                        var previous_user = user;
                        user = u;
                        SendMessage(message);
                        user = previous_user;
                        foreach (var film in en)
                            film.TwoWeeksNotification = true;
                    }
                    Users.Unload();
                }

                //Напоминание пойти в кино за день, если сейчас 20:00-20:59
                if (DateTime.Now.Hour == 20)
                {
                    foreach (var u in Users.Users_Dict.Values)
                    {
                        string message = "Напоминаю, что следующие фильмы выходят в кино уже завтра:\n\n";
                        var en = u.PlannedFilms[1].Where(f => f.data.premiereRu.Length != 4).Where(f => !f.PremiereNotification).Where(f => DateTime.Now.AddDays(1).CompareTo(User.StringToDate(f.data.premiereRu)) >= 0);
                        if (en.Count() == 0)
                            continue;
                        message += string.Join("\n", en.Select(f => $"{f.data.nameRu ?? f.data.nameEn} ({Film.Methods.ChangeDateType(f.data.premiereRu)})"));
                        var previous_user = user;
                        user = u;
                        SendMessage(message);
                        user = previous_user;
                        foreach (var film in en)
                            film.PremiereNotification = true;
                    }
                    Users.Unload();
                }

                //ежесуточный сброс числа запросов к гуглу
                if (DateTime.Now.CompareTo(ServiceClass.service_data.last_update.AddHours(24)) >= 0)
                    ServiceClass.service_data.ResetGoogleRequests();

                //рассылка кадров, фактов и саундтрека
                if (12 <= DateTime.Now.Hour && DateTime.Now.Hour <= 19)
                {
                    var r = new Random();
                    foreach (var p in Users.Users_Dict.Values.Where(u => u.MailFunction))
                    {
                        while(DateTime.Now.CompareTo(p.NextMail) >= 0 && p.MailObjects.Count != 0)
                        {
                            var mail = p.MailObjects.Dequeue();
                            string message = $"{mail.Name} ({mail.Year})";
                            if (mail.IsTrailer)
                            {
                                if (!p.PlannedFilms[0].Select(f => f.data.filmId.ToString()).Contains(mail.id))
                                    continue;
                                attachments = new List<MediaAttachment> { mail.Trailer };
                            }
                            else
                            {
                                attachments = new List<MediaAttachment>();
                                attachments.AddRange(mail.Posters.Select(p => p as MediaAttachment));
                                if (mail.SoundTrack != null && mail.SoundTrack.Count != 0)
                                    attachments.AddRange(mail.SoundTrack.Select(s => s as MediaAttachment));
                                if (mail.Facts != null)
                                {
                                    message += "\n\n";
                                    message += string.Join("\n", mail.Facts.Select(f => $"✅ {f}"));
                                }
                            }
                            var previous_user = user;
                            user = p;
                            SendMessage(message);
                            user = previous_user;
                            attachments = null;
                            var next = DateTime.Now.AddDays(p.DaysGap);
                            p.NextMail = new DateTime(next.Year, next.Month, next.Day, r.Next(12, 21), 0, 0);
                        }
                    }
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
