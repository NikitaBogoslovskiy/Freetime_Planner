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

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены служебные изображения
        /// </summary>
        public static long album_id = 277695979;

        /// <summary>
        /// ID альбома в Вконтакте, в котором размещены постеры популярных фильмов
        /// </summary>
        public static long album_id_popular = 278759103;

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

        //Функции региона MainArea

        /// <summary>
        /// Конструктор класса Bot, внутри которого выполняются все подготовительные действия
        /// </summary>
        public Bot()  
        {
            Init();
            Users.Upload();
            WritelnColor("Пользователи загружены", ConsoleColor.Green);
            AccessTokens.Upload();
            WritelnColor("Токены и ключи доступа загружены", ConsoleColor.Green);
            Keyboards.Init();
            WritelnColor("Клавиатуры инициализированы", ConsoleColor.Green);
            ServiceClass.UploadServiceData();
            WritelnColor("Сервисные данные загружены", ConsoleColor.Green);
            Food.UploadGenreFood();
            WritelnColor("Файл с жанрами и едой загружен", ConsoleColor.Green);
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
                private_vkapi.Authorize(new ApiAuthParams {
                    Login = _vk_login,
                    Password = _vk_password
                });
                yandex_api.Authorize(_yandex_login, _yandex_password);

                WritelnColor("Загрузка популярных фильмов...", ConsoleColor.White);
                Film.UploadPopularFilms();
                WritelnColor("Список популярных фильмов загружен", ConsoleColor.Green);
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
                WriteLine($"Успешно отправлен ответ: {message}");
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
                    message = messages[i];
                    VkNet.Model.User Sender = vkapi.Users.Get(new long[] { messages[i].PeerId.Value })[0];
                    user = Users.GetUser(Sender, out bool IsOld);
                    WriteLine($"Новое сообщение от пользователя {Sender.FirstName} {Sender.LastName}: {messages[i].Text}");
                    if (!IsOld)
                    {
                        keyboard = Keyboards.MainKeyboard;
                        SendMessage($"Привет, {user.Name}! Я - чат-бот Вконтакте, который сможет подсказать фильм, сериал или еду для просмотра. Пока я нахожусь в тестовой версии," +
                            $" используемый функционал ограничен. Для больших возможностей используй мобильное приложение или мобильную версию ВКонтакте. Тебе уже доступны кнопки, открывай быстрее!");
                        return;
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

        public static string MainHelp = $"Привет! Меня зовут Freetime Planner, и я чат-бот ВКонакте. Моя главная цель - помочь тебе разнообразить твой досуг. " +
                            "Весь мой функционал находится в тестовом состоянии, однако уже сейчас ты можешь попробовать " +
                            "некоторые функции.\n\nВ этом меню есть три кнопки:\n- 'Фильмы': если ты хочешь узнать какую-то информацию о фильме или не знаешь, что посмотреть, то " +
                            "тогда тебе сюда\n- 'Сериалы': если ты преследуешь те же цели, что и в предыдущем пункте, но тебя интересуют сериалы - жми на эту кнопку\n" +
                            "- 'Еда под просмотр': если ты любитель перекусить чего-нибудь вкусненького во время фильма или сериала - быстрее жми на эту кнопку\n\nБудет классно, если ты будешь использовать мобильное приложение ВКонакте - " +
                            "так тебе будут доступны бОльшие возможности. Дальше ты так же сможешь найти" +
                            " кнопки 'Помощь', на которые можешь смело нажимать, если не знаешь, что делать. Хорошего досуга!";

        public static string FilmHelp = "В этом меню тебе доступны самые разные кнопки, давай посмотрим, что каждая делает:\n- 'Поиск по названию': если ты хочешь посмотреть " +
                                        "подробную информацию по конкретному фильму и знаешь его название, то жми сюда - дальше я тебе предложу ввести название фильма\n- 'Мои рекомендации': " +
                                        "если ты не знаешь, что посмотреть, то эта кнопка для тебя - на основе тех фильмов, которые тебе понравились или которые ты только хочешь посмотреть" +
                                        ", я составлю список персональных рекомендаций и отправлю их в этот диалог (поэтому больше взаимодействуй со мной - так я лучше пойму твои предпочтения)\n" +
                                        "- 'Планирую посмотреть': при нажатии на эту кнопку ты получишь список всех фильмов, которые ты хотел посмотреть и отложил их сюда; внизу списка внутри сообщения тебе будет " +
                                        "доступна кнопка 'Уже посмотрел', которую ты можешь использовать, если уже посмотрел планируемый к просмотру фильм и хочешь вычеркнуть его из списка (при" +
                                        " нажатии на нее ты так же сможешь оценить просмотренный фильм)\n- 'Рандомный фильм': название говорит само за себя - жми на эту кнопку, если хочешь посмотреть подробную" +
                                        " информацию по абсолютно случайному фильму.\n\nТакже тебе могут встретиться кнопки, которые расположены внутри сообщения (в большинстве своем они расположены внутри сообщения с подробной информацией о фильме):" +
                                        "\n- 'Хочу посмотреть': если тебя интересует данный фильм, то смело жми на нее - этот фильм появится в списке" +
                                        " планируемых к просмотру фильмов (его можно получить по кнопке 'Планирую посмотреть'), а также я буду стараться рекомендовать тебе похожие фильмы (рекомендации" +
                                        " можно посмотреть по кнопке 'Мои рекомендации')\n- 'Посмотрел': если этот фильм уже просмотрен тобой, то используй эту кнопку - я предложу тебе оценить его; так " +
                                        "я смогу понимать, какие фильмы больше не предлагать к просмотру, а также буду рекомендовать похожие фильме, если этот тебе понравился\n- 'Саундтрек': если " +
                                        "ты хочешь послушать песни из фильма прямо в этом диалоге, то эта опция для тебя - я сброшу тебе официальный саундтрек фильма в виде аудиозаписей ВКонтакте\n" +
                                        "- 'Что поесть': если ты хочешь чего-нибудь перекусить конкретно под этот фильм, то я в зависимости от характеристик фильма подберу тебе несложное блюдо и пришлю видео, как его готовить\n" +
                                        "- 'Не показывать': если ты не хочешь больше видеть этот фильм в рекомендациях, то используй эту кнопку\n- 'Подробнее': эту кнопку ты сможешь найти в списке рекомендуемых" +
                                        " к просмотру фильмов (кнопка 'Мои рекомендации') - нажав на нее, ты увидишь подробную информацию по выбранному кинофильму";

        public static string TVHelp = "В этом меню тебе доступны самые разные кнопки, давай посмотрим, что каждая делает:\n- 'Поиск по названию': если ты хочешь посмотреть " +
                                       "подробную информацию по конкретному сериалу и знаешь его название, то жми сюда - дальше я тебе предложу ввести название сериала\n- 'Мои рекомендации': " +
                                       "если ты не знаешь, что посмотреть, то эта кнопка для тебя - на основе тех сериалов, которые тебе понравились или которые ты только хочешь посмотреть" +
                                       ", я составлю список персональных рекомендаций и отправлю их в этот диалог (поэтому больше взаимодействуй со мной - так я лучше пойму твои предпочтения)\n" +
                                       "- 'Планирую посмотреть': при нажатии на эту кнопку ты получишь список всех сериалов, которые ты хотел посмотреть и отложил их сюда; внизу списка внутри сообщения тебе будет" +
                                       "доступна кнопка 'Уже посмотрел', которую ты можешь использовать, если уже посмотрел планируемый к просмотру сериал и хочешь вычеркнуть его из списка - при" +
                                       " нажатии на нее ты так же сможешь оценить просмотренный сериал\n- 'Рандомный сериал': название говорит само за себя - жми на эту кнопку, если хочешь посмотреть подробную" +
                                       " информацию по абсолютно случайному сериалу\n\nТакже тебе могут встретиться кнопки, которые расположены внутри сообщения (в большинстве своем они расположены внутри сообщения с подробной информацией о сериале):" +
                                       "\n- 'Хочу посмотреть': если тебя интересует данный сериал, то смело жми на нее - этот сериал появится в списке" +
                                       " планируемых к просмотру сериалов (его можно получить по кнопке 'Планирую посмотреть'), а также я буду стараться рекомендовать тебе похожие сериалы (рекомендации" +
                                       " можно посмотреть по кнопке 'Мои рекомендации')\n- 'Посмотрел': если этот сериал уже просмотрен тобой, то используй эту кнопку - я предложу тебе оценить его; так " +
                                       "я смогу понимать, какие сериалы больше не предлагать к просмотру, а также буду рекомендовать похожие сериалы, если этот тебе понравился\n- 'Саундтрек': если " +
                                       "ты хочешь послушать песни из сериала прямо в этом диалоге, то эта опция для тебя - я сброшу тебе официальный саундтрек сериала в виде аудиозаписей ВКонтакте\n" +
                                       "- 'Что поесть': если ты хочешь чего-нибудь перекусить конкретно под этот сериал, то я в зависимости от характеристик фильма подберу тебе несложное блюдо и пришлю видео, как его готовить\n" +
                                       "- 'Не показывать': если ты не хочешь больше видеть этот сериал в рекомендациях, то используй эту кнопку\n- 'Подробнее': эту кнопку ты сможешь найти в списке рекомендуемых" +
                                       " к просмотру фильмов (кнопка 'Мои рекомендации') - нажав на нее, ты увидишь подробную информацию по выбранному сериалу";

        public static string FoodHelp = "Здесь тебе доступны три кнопки:\n- 'Сладкое': если ты хочешь приготовить что-нибудь сладенькое под просмотр фильма, не желая тратить на это много" +
                                    " времени, то жми сюда - я пришлю видео приготовления несложного десерта\n- 'Закуски': если тебя интересуют холодные закуски, то эта кнопка для тебя - тебе так же придет" +
                                    " видео приготовления блюда\n- 'Коктели': если хочешь приготовить несложный безалкогольный коктейль, то используй эту кнопку, чтобы получить видео-инструкцию приготовления";

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
                        var level = MainMenu(message.Text);
                        if (payload == string.Empty)
                        {
                            SendMessage("Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки");
                            break;
                        }
                        else
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
                    break;

                case 2:
                    Mode previous_level = user.CurrentLevel();
                    try
                    {
                        var level = SecondMenu(message.Text);
                        if (payload == string.Empty)
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
                            switch (user.CurrentLevel())
                            {
                                //"Посмотрел"
                                case Watched:
                                    user.HideFilm(int.Parse(p.filmId));
                                    keyboard = Keyboards.FilmWatched(p.nameEn);
                                    SendMessage("Понравился фильм?");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                //"Хочу посмотреть"
                                case WantToWatch:
                                    user.AddPlannedFilm(p.nameRu, p.nameEn, p.date);
                                    SendMessage("Добавлено в список планируемых фильмов");
                                    user.RemoveLevel();
                                    break;

                                //"Саундтрек"
                                case Soundtrack:
                                    SendMessage("Собираю трек-лист...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    List<Audio> audios = new List<Audio>();
                                    if (!Film.Methods.Soundtrack(p.nameEn ?? p.nameRu, p.date.Substring(0, 4), audios))
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
                                    attachments = new List<MediaAttachment> { Film.Methods.Food(p.genres.Split('*')) as MediaAttachment };
                                    SendMessage("");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Не показывать"
                                case BlackList:
                                    user.HideFilm(int.Parse(p.filmId));
                                    SendMessage("Добавлено в список нежелаемых фильмов");
                                    user.RemoveLevel();
                                    break;

                                //"Подробнее"
                                case More:
                                    SendMessage("Готовлю детали по фильму...");
                                    vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    if (user.FilmRecommendations.TryGetValue(int.Parse(p.filmId), out Film.FilmObject film))
                                    {
                                        attachments = new List<MediaAttachment> { Attachments.PosterObject(film.data.posterUrl, film.data.filmId.ToString()) }; //скачать изображение
                                        keyboard = Keyboards.FilmSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), film.data.premiereRu ?? film.data.premiereWorld, string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereDigital ?? film.data.premiereDvd);
                                        SendMessage(Film.Methods.FullInfo(film));
                                    }
                                    else
                                    {
                                        SendMessage(Film.Methods.FullInfo(int.Parse(p.filmId)));
                                        //где-то нужно присвоить attachments и keyboard
                                    }
                                    keyboard = null;
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                //"Уже посмотрел"
                                case AlreadyWatched:
                                    SendMessage("Введи номер просмотренного кинофильма");
                                    break;

                                //"Да" (посмотрел, понравилось)
                                case Yes:
                                    user.LikeFilm(p.nameEn);
                                    SendMessage("Круто! Будем советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                //"Нет" (посмотрел, не понравилось)
                                case No:
                                    SendMessage("Жаль... Постараемся подобрать что-нибудь получше");
                                    user.RemoveLevel();
                                    break;

                                //"Где посмотреть"
                                case WhereToWatch:
                                    if (ServiceClass.service_data.google_requests < 100)
                                    {
                                        SendMessage("Ищу места для просмотра фильма...");
                                        keyboard = Film.Methods.ServiceLinks(p.nameRu, p.date, p.digital_release);
                                        SendMessage("Жми любую кнопку и смотри!");
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
                                    template = user.GetFilmRecommendations();                                  
                                    keyboard = null;
                                    SendMessage("Рекомендуемые фильмы");
                                    template = null;
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
                                    template = Film.Methods.Random();
                                    keyboard = null;
                                    SendMessage("Подборка случайных фильмов");
                                    template = null;
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
                            switch (user.CurrentLevel())
                            {
                                case Watched:
                                    keyboard = Keyboards.TVWatched(p.nameRu, p.nameEn, p.filmId, p.genres);
                                    SendMessage("Понравился сериал?");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                case WantToWatch:
                                    user.AddPlannedTV(p.nameRu, p.nameEn);
                                    SendMessage("Добавлено в список планируемых сериалов");
                                    user.RemoveLevel();
                                    break;

                                case Soundtrack:
                                    attachments = TV.Methods.Soundtrack(p.nameRu).Select(a => a as MediaAttachment).ToList();
                                    SendMessage("Саундтрек к сериалу");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                case GenreFood:
                                    attachments = new List<MediaAttachment> { TV.Methods.Food(p.genres.Split('*')) as MediaAttachment };
                                    SendMessage("Видео-инструкция приготовления несложного блюда");
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                case BlackList:
                                    user.HideTV(int.Parse(p.filmId));
                                    SendMessage("Добавлено в список нежелаемых сериалов");
                                    user.RemoveLevel();
                                    break;

                                case More:
                                    if (user.TVRecommendations.TryGetValue(int.Parse(p.filmId), out TV.TVObject tv))
                                    {
                                        attachments = null; //скачать изображение
                                        keyboard = Keyboards.TVSearch(p.nameRu, p.nameEn, p.filmId, p.genres);
                                        SendMessage(TV.Methods.FullInfo(tv));
                                    }
                                    else
                                    {
                                        SendMessage(TV.Methods.FullInfo(int.Parse(p.filmId)));
                                        //где-то нужно присвоить attachments и keyboard
                                    }
                                    SendMessage("Подробная информация по выбранному фильму");
                                    keyboard = null;
                                    attachments = null;
                                    user.RemoveLevel();
                                    break;

                                case AlreadyWatched:
                                    SendMessage("Введи номер просмотренного сериала");
                                    break;

                                case Yes:
                                    user.LikeTV(p.nameEn);
                                    SendMessage("Круто! Будем советовать похожие");
                                    user.RemoveLevel();
                                    break;

                                case No:
                                    SendMessage("Жаль... Постараемся подобрать что-нибудь получше");
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
                                case Search:
                                    keyboard = null;
                                    SendMessage("Введи название сериала");
                                    break;

                                case Recommendations:
                                    //vkapi.Messages.SetActivity(user.ID.ToString(), MessageActivityType.Typing, user.ID, ulong.Parse(group_id.ToString()));
                                    keyboard = null;
                                    template = user.GetTVRecommendations();
                                    SendMessage("Рекомендуемые сериалы");
                                    template = null;
                                    user.RemoveLevel();
                                    break;

                                case PlanToWatch:
                                    keyboard = Keyboards.TVPlanToWatch();
                                    SendMessage(user.GetPlannedTV());
                                    keyboard = null;
                                    user.RemoveLevel();
                                    break;

                                case Modes.Mode.Random:
                                    template = TV.Methods.Random();
                                    //TODO
                                    SendMessage("Подборка случайных сериалов");
                                    template = null;
                                    user.RemoveLevel();
                                    break;
                                case Back:
                                    keyboard = Keyboards.MainKeyboard;
                                    SendMessage("Выбери один из режимов");
                                    keyboard = null;
                                    user.RemoveLevel();
                                    user.RemoveLevel();
                                    break;
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
                        switch (user.CurrentLevel())
                        {
                            case Snack:
                                SendMessage("Подбираю закуску...");
                                attachments = new List<MediaAttachment> { Food.Snack() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            case Dessert:
                                SendMessage("Подбираю десерт...");
                                attachments = new List<MediaAttachment> { Food.Dessert() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            case Cocktails:
                                SendMessage("Подбираю коктейль...");
                                attachments = new List<MediaAttachment> { Food.Cocktail() as MediaAttachment };
                                SendMessage("");
                                attachments = null;
                                break;

                            case Back:
                                keyboard = Keyboards.MainKeyboard;
                                SendMessage("Выбери один из режимов");
                                keyboard = null;
                                user.RemoveLevel();
                                break;
                            case Help:
                                SendMessage(FoodHelp);
                                break;

                        }
                        user.RemoveLevel();
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
                                template = Film.Methods.Search(p.nameRu);
                                SendMessage("Результаты поиска");
                                template = null;
                                break;

                            //<порядковый номер фильма> (после кнопки "Уже посмотрел")
                            case AlreadyWatched:
                                int index = int.Parse(message.Text) - 1;
                                Film.FilmObject film = null;
                                if (user.PlannedFilms[0].Count <= index && index < (user.PlannedFilms[0].Count+ user.PlannedFilms[1].Count))
                                    film = user.PlannedFilms[1][index];
                                else if (0 <= index && index < user.PlannedFilms[0].Count)
                                    film = user.PlannedFilms[0][index];
                                else
                                {
                                    SendMessage("Номер фильма некорректен");
                                    break;
                                }
                                user.HideFilm(film.data.filmId);
                                SendMessage("Фильм перенесен в список просмотренных");
                                keyboard = Keyboards.FilmWatched(film.data.nameEn);
                                SendMessage("Понравился фильм?");
                                keyboard = null;
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
                            case Search:
                                template = Film.Methods.Search(p.nameRu);
                                SendMessage("Результаты поиска");
                                template = null;
                                break;
                            case AlreadyWatched:
                                int index = int.Parse(message.Text) - 1;
                                if (index < 0 || index >= user.PlannedTV.Count)
                                {
                                    SendMessage("Номер сериала некорректен");
                                    break;
                                }
                                TV.TVObject tv = user.PlannedTV[index];
                                user.HideTV(tv.data.filmId);
                                SendMessage("Сериал перенесен в список просмотренных");
                                keyboard = Keyboards.TVWatched(tv.data.nameRu, tv.data.nameEn, tv.data.filmId.ToString(), string.Join('*', tv.data.genres.Select(g => g.genre)));
                                SendMessage("Понравился сериал?");
                                keyboard = null;
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
        public static int update_time = 7; //7 дней - срок, после которого обновляются популярные фильмы
        public static object PFTsynclock = new object();

        public static Timer PlannedFilmsTimer;
        public static int PlFTinterval = 3600000; //1 час - интервал проверки 
        public static int day_time = 0; //0 - час в сутках, в который обновляются планируемые фильмы (т.е. в диапозоне 0:00-0:59)
        public static object PlFTsynclock = new object();

        /// <summary>
        /// Инициализация таймеров
        /// </summary>
        public static void InitTimers()
        {
            ResetTimer = new Timer(new TimerCallback(Reset), null, 0, RTinterval); //таймер, вызывающий каждый интервал времени RTinterval функцию Reset
            PopularFilmsTimer = new Timer(new TimerCallback(RegularPopularFilmsUpdating), null, 0, PFTinterval); //таймер, вызывающий каждый интервал времени PFTinterval функцию RegularPopularFilmsUpdating
            PlannedFilmsTimer = new Timer(new TimerCallback(DailyPlannedFilmsUpdating), null, 0, PlFTinterval); //таймер, вызывающий каждый интервал времени PlFTinterval функцию DailyPlannedFilmsUpdating
        }

        /// <summary>
        /// Функция, вызывающая функцию ResetLevel в случае, если истекло время из поля reset_time
        /// </summary>
        /// <param name="obj"></param>
        public static void Reset(object obj)
        {
            lock(synclock)
            {
                foreach(var pair in Users.Users_Dict)
                {
                    if (TimeIsUp(pair.Value, reset_time) && pair.Value.CurrentLevel() != Mode.Default)
                        pair.Value.ResetLevel();
                }
            }
        }

        /// <summary>
        /// Функция, вызывающая Film.UpdatePopularFilms, если прошла неделя после последнего обновления списка популярных фильмов
        /// </summary>
        /// <param name="obj"></param>
        public static void RegularPopularFilmsUpdating(object obj)
        {
            lock(PFTsynclock)
            {
                if (DateTime.Now.CompareTo(Film.LastPopularFilmsUpdate.AddDays(update_time)) != -1)
                {
                    Film.UpdatePopularFilms();
                    Film.LastPopularFilmsUpdate = DateTime.Now;
                    Film.UnloadPopularFilms();
                }
            }
        }

        /// <summary>
        /// Функция, вызывающая для каждого пользователя UpdatePlannedFilms, если время суток в диапозоне 0:00-0:59
        /// </summary>
        /// <param name="obj"></param>
        public static void DailyPlannedFilmsUpdating(object obj)
        {
            lock(PlFTsynclock)
            {
                if (DateTime.Now.Hour == 0)
                {
                    foreach (var user in Users.Users_Dict.Values)
                        user.UpdatePlannedFilms();
                    Users.Unload();
                    //ежесуточный сброс числа запросов к гуглу
                    ServiceClass.service_data.ResetGoogleRequests();
                }
                if (DateTime.Now.CompareTo(ServiceClass.service_data.last_update.AddHours(24)) >= 0)
                    //ежесуточный сброс числа запросов к гуглу
                    ServiceClass.service_data.ResetGoogleRequests();
            }
        }

        #endregion

        /// <summary>
        /// Функция, возвращающая true, если время истекло
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool TimeIsUp(User user, long critical_time) => DateTime.Now.CompareTo(user.LastTime.AddMilliseconds(critical_time)) != -1;

    }
}
