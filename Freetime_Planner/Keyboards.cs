using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using static VkNet.Enums.SafetyEnums.KeyboardButtonColor;
using VkNet.Model.Template;
using static VkNet.Model.Template.MessageTemplate;
using static VkNet.Enums.SafetyEnums.TemplateType;
using VkNet.Model.Template.Carousel;
using System.Linq;
using static Freetime_Planner.Film;
using static Freetime_Planner.TV;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


using VkNet.Model.Attachments;//


namespace Freetime_Planner
{
    public static class Keyboards
    {
        public static MessageKeyboard MainKeyboard { get; private set; }
        public static MessageKeyboard FilmKeyboard { get; private set; }
        public static MessageKeyboard TVKeyboard { get; private set; }
        public static MessageKeyboard FoodKeyboard { get; private set; }

        /// <summary>
        /// Клавиатура главного меню
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard MainMenu()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Фильмы", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Сериалы", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Еда под просмотр", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Настройки", "Command", Positive, "text");


            return button.Build();
        }


        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Еда под просмотр" (Build MessageKeyboard for button "Еда под просмотр")
        /// </summary>
        /// <button></button>
        public static MessageKeyboard Food()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Закуски", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Сладкое", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Коктейли", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Назад", "Command", Negative, "text");

            return button.Build();
        }

        #region Film

        //Фильмы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Фильмы" (Build MessageKeyboard for button "Фильмы")
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard Film()
        {

            var result = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            result.Clear();
            //"Поиск по названию"
            result.AddButton("Поиск по названию", "Command", Primary, "text");
            result.AddLine();
            //"Мои рекомендации"
            result.AddButton("Мои рекомендации", "Command", Primary, "text");
            result.AddLine();
            //"Планирую посмотреть"
            result.AddButton("Планирую посмотреть", "Command", Primary, "text");
            result.AddLine();
            //"Рандомный фильм"
            result.AddButton("Рандомный фильм", "Command", Primary, "text");
            result.AddLine();
            //"Назад"
            result.AddButton("Помощь", "Command", Positive, "text");
            result.AddButton("Назад", "Command", Negative, "text");

            return result.Build();

        }

        /// <summary>
        /// Создаёт клавиатуру в сообщении для кнопоки "Фильмы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>
        public static MessageKeyboard FilmSearch(string nameRu, string nameEn, string filmID, string date, string genres, string digital_release)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", $"f;{nameRu};{nameEn};{filmID};{date};;", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", $"f;;{nameEn};{filmID};;;", Primary, "text");
            button.AddLine();
            if (ServiceClass.service_data.google_requests <= 100 && digital_release != null && DateTime.Now.CompareTo(User.StringToDate(digital_release)) >= 0)
            {
                button.AddButton("Где посмотреть", $"f;{nameRu};;;{date};;{digital_release}", Primary, "text");
                button.AddLine();
            }
            button.AddButton("Саундтрек", $"f;{nameRu};{nameEn};;{date};;", Primary, "text");
            button.AddButton("Еда", $"f;;;;;{genres};", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", $"f;;;{filmID};;;", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        //"Фильмы"->"Мои рекомендации"

        /// <summary>
        /// Возвращает карусель из нескольких фильмов из списка рекомендаций
        /// </summary>
        /// <param name="farray"></param>
        /// <returns></returns>
        public static MessageTemplate FilmMyRecommendations(IEnumerable<Film.FilmObject> farray)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            foreach (var f in farray)
            {
                arr.Add(CarouselFilm(f));
            }
            carousel.Elements = arr;

            return carousel;
        }
     //-----------------------------"Мои рекомендации для не мобильного приложения"---------------------------------------
        public static void FilmMyRecommendationsMessage(IEnumerable<Film.FilmObject> farray)
        {
            int i = 0;
            var photo = Bot.private_vkapi.Photo.GetById(farray.Select(f => f.data.VKPhotoID));
            foreach (var f in farray)
            {
               Bot.attachments = new List<MediaAttachment> { Bot.private_vkapi.Photo.GetById(new string[]{ f.data.VKPhotoID})[0] };
               Bot.SendMessage(FilmMessage(f));
                ++i;
            }
      
        }

        public static string FilmMessage(Film.FilmObject film)
        {
            
                var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
                button.AddButton("Подробнее", $"f;;;{film.data.filmId};;;", Positive, "text");
                button.SetInline();
                Bot.keyboard = button.Build();
            
            string str = film.data.nameRu + "\nжанр: " + string.Join(',',film.data.genres.Select(g =>g.genre)) + ".";
            return str;
        }
        //-----------------------------"_|_"---------------------------------------

        /// <summary>
        /// Возвращает один элемент карусели фильмов-рекомендаций
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static CarouselElement CarouselFilm(Film.FilmObject film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.data.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"f;;;{film.data.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.data.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = film.data.VKPhotoID;
            return element;

        }

        /// <summary>
        /// Возвращает результаты поиска фильмов по названию в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate FilmResults(FilmResults.Results results)
        {
            IEnumerable<FilmResults.Film> films = results.films.Where(f => !f.nameRu.EndsWith("(сериал)") && !f.nameRu.EndsWith("(мини-сериал)")).Take(3);
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(films, (film, state) =>
            {
                CarouselElement template_part = null;
                if (CarouselFilmResult(film, ref template_part))
                    //if (arr.Count < 3)
                    arr.Add(template_part);
                //else
                //state.Break();
            });
            carousel.Elements = arr;
            if (arr.Count == 0)
                return null;
            else
                return carousel;
        }
        /// <summary>
        /// Возвращает один элемент карусели из результатов поиска фильма по названию
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static bool CarouselFilmResult(FilmResults.Film film, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.ResultedFilmPosterID(film);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }
        }

        /// <summary>
        /// Возвращает результаты случайного поиска фильмов в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate RandomFilmResults(RandomFilms.Results results)
        {
            IEnumerable<RandomFilms.Film> films = results.films.Shuffle().Take(3);
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(films, (film, state) =>
            {
                CarouselElement template_part = null;
                if (CarouselRandomFilmResult(film, ref template_part))
                    //if (arr.Count < 3)
                    arr.Add(template_part);
                //else
                //state.Break();
            });
            if (arr.Count != 0)
                carousel.Elements = arr;
            else
            {
                carousel = FilmMyRecommendations(PopularFilms.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль");
            }

            return carousel;
        }
        /// <summary>
        /// Возвращает один элемент карусели рандомных фильмов
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static bool CarouselRandomFilmResult(RandomFilms.Film film, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.RandomFilmPosterID(film);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }
        }
        //------------------"Рандомный фильм" для не мобильного приложения -----------------------------------------------

        public static void RandomFilmResultsMessage(RandomFilms.Results results)
        {
            IEnumerable<RandomFilms.Film> films = results.films.Shuffle().Take(3);

            var arr = new List<(string, Photo, MessageKeyboard)>();
            // паралельно создаём три фильма для рандома
            Parallel.ForEach(films, (film, state) =>
            {
               // string message_part = null;//название,жанры одного фильма/сообщения
                MessageRandomFilmResult(film, out var message_part, out var photo, out var more);
                var tuple = (message_part, photo, more);
                arr.Add(tuple);
            });
            Bot.SendMessage("Результаты поиска");
            if (arr.Count != 0)
            foreach (var m in  arr)
                {
                    Bot.attachments = new List<MediaAttachment> { m.Item2 };
                    Bot.keyboard = m.Item3; 
                    Bot.SendMessage(m.Item1);
                }
                    
            else
            {
                FilmMyRecommendationsMessage(PopularFilms.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль");
            }

            
        }
        /// <summary>
        /// Возвращает один элемент карусели рандомных фильмов
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static void MessageRandomFilmResult(RandomFilms.Film film, out string message_part, out Photo photo, out MessageKeyboard grok)
        {
            //создаём клавиатуру
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            grok = button.Build();
            //достаём постер
             photo = Attachments.PosterObject(film.posterUrl, film.filmId.ToString());
            //готовим сообщение
            message_part = " " + film.nameRu   +"\nжанр: " + string.Join(", ", film.genres.Select(g => g.genre)) + ".";
            //добро пожаловать в Рандом
        }



        //---------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// "Фильмы"->"Планирую посмотреть"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard FilmPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();
            button.AddButton("Уже посмотрел", "f;;;;;;", Primary, "text");
            button.SetInline();
            return button.Build();
        }
        /// <summary>
        ///  "Фильмы"->"Рандомный фильм"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomFilm(string nameRu, string nameEn, string filmID, string date, string genres, string digital_release)
        {
            return FilmSearch(nameRu, nameEn, filmID, date, genres, digital_release);
        }

        public static MessageKeyboard ServiceLinks(Dictionary<string, string> dict)
        {
            var kb = new Keyboard(dict);
            return JsonConvert.DeserializeObject<MessageKeyboard>(JsonConvert.SerializeObject(kb));
        }

        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard FilmWatched(string nameEn, string filmID)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", $"f;;{nameEn};{filmID};;;", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", $"f;;{nameEn};;;;", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        #endregion

        #region TV

        //Сериалы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Сериалы" (Build MessageKeyboard for button "Сериалы")
        /// </summary>
        /// <button></button>
        public static MessageKeyboard TV()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Поиск по названию", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Мои рекомендации", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Планирую посмотреть", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Рандомный сериал", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Назад", "Command", Negative, "text");

            return button.Build();
        }

        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Сериалы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>     
        public static MessageKeyboard TVSearch(string nameRu, string nameEn, string filmID, string genres)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", $"t;{nameRu};{nameEn};{filmID};;;", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", $"t;;{nameEn};{filmID};;;", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", $"t;{nameRu};{nameEn};;;;", Primary, "text");
            button.AddButton("Еда", $"t;;;;;{genres};", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", $"t;;;{filmID};;;", Negative, "text");

            button.SetInline();
            return button.Build();
        }



        //"Сериалы"->"Мои рекомендации"
        public static MessageTemplate TVMyRecommendations(IEnumerable<TVObject> tvs)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            foreach (var f in tvs)
            {
                arr.Add(CarouselTV(f));
            }
            carousel.Elements = arr;

            return carousel;
           

        }
        //Не (mobile online)
        public static void TVMyRecommendationsMessage(IEnumerable<TVObject> tvs)
        {
 
            int i = 0;
            var photo = Bot.private_vkapi.Photo.GetById(tvs.Select(f => f.data.VKPhotoID));
            foreach (var f in tvs)
            {
                Bot.attachments = new List<MediaAttachment> { Bot.private_vkapi.Photo.GetById(new string[] { f.data.VKPhotoID })[0] };
                Bot.SendMessage(TVMessage(f));
                ++i;
            }
        }
        public static string TVMessage(TVObject tv)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{tv.data.filmId};;;", Positive, "text");
            button.SetInline();
            Bot.keyboard = button.Build();

            string str = tv.data.nameRu + "\nжанр: " + string.Join(',', tv.data.genres.Select(g => g.genre)) + ".";
            return str;

        }

            public static CarouselElement CarouselTV(TVObject tv)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', tv.data.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"t;;;{tv.data.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = tv.data.nameRu.Replace("(сериал)", "");
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = tv.data.VKPhotoID;
            return element;
        }

        public static MessageTemplate TVResults(TVResults.Results results)
        {
            IEnumerable<TVResults.Film> films = results.films.Where(f => f.nameRu.EndsWith("(сериал)") || f.nameRu.EndsWith("(мини-сериал)")).Take(3);
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(films, (film, state) =>
            {
                CarouselElement template_part = null;
                if (CarouselTVResult(film, ref template_part))
                    arr.Add(template_part);
            });
            carousel.Elements = arr;
            if (arr.Count == 0)
                return null;
            else
                return carousel;
        }
        public static bool CarouselTVResult(TVResults.Film film, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"t;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu.Replace("(сериал)", "");
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.ResultedTVPosterID(film);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }
        }

        public static MessageTemplate RandomTVResults(RandomTV.Results results)
        {
            IEnumerable<RandomTV.Film> films = results.films.Shuffle().Take(3);
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(films, (film, state) =>
            {
                CarouselElement template_part = null;
                if (CarouselRandomTVResult(film, ref template_part))
                    arr.Add(template_part);
            });
            if (arr.Count != 0)
                carousel.Elements = arr;
            else
            {
                carousel = TVMyRecommendations(PopularTV.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль");
            }

            return carousel;
        }
        public static bool CarouselRandomTVResult(RandomTV.Film film, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"t;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu.Replace("(сериал)", "");
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.RandomTVPosterID(film);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }

        }
        /// <summary>
        /// "Сериалы"->"Планирую посмотреть"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "t;;;;;;", Primary, "text");

            button.SetInline();
            return button.Build();
        }

        /// <summary>
        ///  "Сериалы"->"Рандомный сериал"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomTV(string nameRu, string nameEn, string filmID, string genres)
        {
            return TVSearch(nameRu, nameEn, filmID, genres);
        }

        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVWatched(string nameEn, string TVID)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", $"t;;{nameEn};{TVID};;;", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", $"t;;{nameEn};{TVID};;;", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        #endregion

        public static MessageKeyboard Options()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Частота рассылки", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Назад", "Command", Negative, "text");

            return button.Build();
        }

        public static MessageKeyboard MailFrequency()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Ежедневно", $"o;;;;;;", Default, "text");
            button.AddLine();
            button.AddButton("Раз в три дня", $"o;;;;;;", Default, "text");
            button.AddLine();
            button.AddButton("Раз в пять дней", $"o;;;;;;", Default, "text");
            button.AddLine();
            button.AddButton("Раз в неделю", $"o;;;;;;", Default, "text");
            button.AddLine();
            button.AddButton("Без рассылки", $"o;;;;;;", Default, "text");

            button.SetInline();
            return button.Build();
        }

        public static void Init()
        {
            MainKeyboard = MainMenu();
            FilmKeyboard = Film();
            TVKeyboard = TV();
            FoodKeyboard = Food();
        }

    }


    #region ServiceLinksKeyboard

    public class Keyboard
    {
        public bool one_time = false;
        public List<List<object>> buttons = new List<List<object>>();
        public bool inline = true;
        public Keyboard(Dictionary<string, string> dict)
        {
            foreach (var kv in dict)
            {
                Button button = new Button(kv.Key, kv.Value);
                buttons.Add(new List<object>() { button });
            }
        }


    }
    public class Button
    {
        public Action action;
        public string color;
        public Button(string name, string link)
        {
            color = null;
            action = new Action(name, link);
        }

    }
    public class Action
    {
        public string type;
        public string payload;
        public string label;
        public string link;
        public Action(string name, string link)
        {
            type = "open_link";
            label = name;
            this.link = link;
            payload = "";
        }

    }

    #endregion 
}