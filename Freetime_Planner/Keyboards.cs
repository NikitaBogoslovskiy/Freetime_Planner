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

        public static MessageKeyboard GenresKeyboard { get; private set; }

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
            //"Поиск По названию и По жанрам"
            result.AddButton("По названию", "Command", Primary, "text");
            result.AddButton("По жанру", "Command", Primary, "text");
            result.AddLine();
            //"Рекомендованно и В планах"
            result.AddButton("Рекомендовано", "Command", Primary, "text");
            result.AddButton("Запланировано", "Command", Primary, "text");                                
            result.AddLine();
            //"Рандомный фильм"
            result.AddButton("Случайный фильм", "Command", Primary, "text");
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
            if (ServiceClass.service_data.google_requests < 100 && digital_release != null && DateTime.Now.CompareTo(User.StringToDate(digital_release)) >= 0)
            {
                button.AddButton("Смотреть", $"f;{nameRu};;;{date};;{digital_release}", Primary, "text");
                
            }
            button.AddButton("Актеры", $"f;;;{filmID};;;", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", $"f;{nameRu};{nameEn};;{date};;", Primary, "text");
            button.AddButton("Еда", $"f;;;;;{genres};", Primary, "text"); 
            button.AddLine();
            button.AddButton("Не показывать", $"f;;;{filmID};;;", Negative, "text");

            

            button.SetInline();
            return button.Build();
        }

        public static MessageKeyboard SearchGenreList()
        {
            var result = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            result.Clear();

            result.AddButton("Фантастика", $"Command", Primary,"text");
            result.AddButton("Детектив", $"Command", Primary, "text");
            result.AddButton("Боевик", $"Command", Primary, "text");
            result.AddLine();
            result.AddButton("Комедия", $"Command", Primary, "text");
            result.AddButton("Аниме", $"Command", Primary, "text");
            result.AddButton("Фэнтези", $"Command", Primary, "text");
            result.AddLine();
            result.AddButton("Драма", $"Command", Primary, "text");
            result.AddButton("Военный", $"Command", Primary, "text");
            result.AddButton("Триллер", $"Command", Primary, "text");
            result.AddLine();
            result.AddButton("Криминал", $"Command", Primary, "text");
            result.AddButton("Семейный", $"Command", Primary, "text");
            result.AddButton("Ужасы", $"Command", Primary, "text");

            result.AddLine();
            result.AddButton("Назад", "Command", Negative, "text");
            //result.SetInline();
            return result.Build();
        }

        //"Фильмы"->"Мои рекомендации"

        //-----------------------------"Мои рекомендации для мобильного приложения"---------------------------------------
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
        //-----------------------------"Мои рекомендации для не мобильного приложения"---------------------------------------
        public static void FilmMyRecommendationsMessage(User user, IEnumerable<Film.FilmObject> farray, bool b = true)
        {
            Bot.SendMessage(user, "Результаты поиска");
            foreach(var film in farray)
            {
                var message_part = FilmMessage(film, out var photo, out var more);
                Bot.SendMessage(user, message_part, more, null, new List<MediaAttachment> { photo });
            }
        }

        public static string FilmMessage(Film.FilmObject film, out Photo photo, out MessageKeyboard more)
        {

            photo = Bot.private_vkapi.Photo.GetById(new string[] { film.data.VKPhotoID_2 })[0];

            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{film.data.filmId};;;", Positive, "text");
            button.SetInline();
            more = button.Build();
            
            string str = film.data.nameRu + "\nЖанр: " + string.Join(", ", film.data.genres.Select(g =>g.genre));
            return str;
        }
        //-----------------------------"_|_"---------------------------------------

       

        //------------------"Поиск по названию" для мобильного приложения -----------------------------------------------

        /// <summary>
        /// Возвращает результаты поиска фильмов по названию в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate FilmResults(User user, FilmResults.Results results)
        {
            IEnumerable<FilmResults.Film> films = results.films.Where(f => !f.nameRu.EndsWith("(сериал)") && !f.nameRu.EndsWith("(мини-сериал)")).Take(3);
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            var l = new List<Task>();
            foreach(var film in films)
            {
                var t = Task.Run(() =>
                {
                    Console.WriteLine($"Выполняю загрузку {film.filmId}...");
                    CarouselElement template_part = null;
                    if (CarouselFilmResult(film, ref template_part))
                    {
                        user.AddSearchPosterAsync(film).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                        arr.Add(template_part);
                    }
                    Console.WriteLine($"Выполнил загрузку {film.filmId}!");
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());
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
        public static bool CarouselFilmResult(FilmResults.Film film,ref CarouselElement template_part)
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
        //------------------"Поиск по названию" для не мобильного приложения -----------------------------------------------

        public static void FilmResultsMessage(User user, FilmResults.Results results)
        {
            IEnumerable<FilmResults.Film> films = results.films.Where(f => !f.nameRu.EndsWith("(сериал)") && !f.nameRu.EndsWith("(мини-сериал)")).Take(3);
            var arr = new List<(string, Photo, MessageKeyboard)>();
            var l = new List<Task>();
            foreach (var film in films)
            {
                var t = Task.Run(() =>
                {
                    Console.WriteLine($"Выполняю загрузку {film.filmId}...");
                    if (MessageFilmResult(user, film, out var message_part, out Photo photo, out var more))
                        arr.Add((message_part, photo, more));
                    Console.WriteLine($"Выполнил загрузку {film.filmId}!");
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());

            if (arr.Count != 0)
            {
                Bot.SendMessage(user, "Результаты поиска");
                foreach(var m in arr)
                    Bot.SendMessage(user, m.Item1, m.Item3, null, new List<MediaAttachment> { m.Item2 });
            }
            else
                Bot.SendMessage(user, "К сожалению, я не смог найти такой фильм... 😔");
        }


        public static bool MessageFilmResult(User user, FilmResults.Film film, out string message_part, out Photo photo,out MessageKeyboard more)
        {
            message_part = null; photo = null; more = null;
            if (film.filmId.ToString() == null)
                return false;
            var s = Attachments.FullResultedFilmPosterID(film, out var p);
            if (s == null)
                return false;
            user.Posters[film.filmId.ToString()] = (s, DateTime.Now);
            photo = p;
            //создаём клавиатуру
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            more = button.Build();
            //готовим сообщение
            message_part = " " + film.nameRu + "\nЖанр: " + string.Join(", ", film.genres.Select(g => g.genre));
            //Начинаем Поиск по названию
            return true;

        }
        //---------------------------------------------------------------------------------------------------------------



        //------------------"Рандомный фильм" для мобильного приложения -----------------------------------------------
        /*/// <summary>
        /// Возвращает результаты случайного поиска фильмов в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate RandomFilmResults(RandomFilms.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            //проверка
            if (results == null || results.pagesCount == 0)
            {
                carousel = FilmMyRecommendations(PopularFilms.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль");
                return carousel;
            }

            IEnumerable<RandomFilms.Film> films = results.films.Shuffle().Take(3);
            
            var arr = new List<CarouselElement>();
            Parallel.ForEach(films, (film) =>
            {
                CarouselElement template_part = null;
                if (CarouselRandomFilmResult(film, ref template_part)) мы проверял зачем то 
                    arr.Add(template_part);
            });
            if (arr.Count != 0)
                carousel.Elements = arr;
            else
            {
                carousel = FilmMyRecommendations(PopularFilms.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль");
            }
    
            return carousel;
        }*/
        /// <summary>
        /// Возвращает результаты случайного поиска фильмов в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate RandomFilmResults(User user, IEnumerable<RandomFilms.Film> results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            foreach (RandomFilms.Film f in results)
            {
                arr.Add(CarouselFilm(f));
                user.Posters[f.filmId.ToString()] = (f.VKPhotoID_2, DateTime.Now);
            }
            carousel.Elements = arr;
            
            return carousel;
        }
        /*/// <summary>
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
            if (element.PhotoId == null)//но надо будет да да да при загрузке в файл не забыть
                return false;
            else
            {
                template_part = element;
                return true;
            }
        }*/

        /// <summary>
        /// Возвращает актеров в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate ActorResults(ActorsTemplate actors, IEnumerable<ActorResults.Actor> results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            var l = new List<Task>();
            foreach(var actor in results)
            {
                var t = Task.Run(() =>
                {
                    CarouselElement template_part = null;
                    if (CarouselActors(actor, ref template_part))
                    {
                        arr.Add(template_part);
                        actors.ActorsIDs[actor.staffId.ToString()] = actor.posterUrl;
                    }
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());
            carousel.Elements = arr;

            return carousel;
        }
        //------------------"Carusel Actors -----------------------------------------------
        public static bool CarouselActors(ActorResults.Actor actor, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Узнать больше", $"f;;;{actor.staffId};;;", Positive, "text");
            var element = new CarouselElement();
            if (actor.nameRu != null && actor.nameRu != string.Empty)
                element.Title = actor.nameRu;
            else if (actor.nameEn != null && actor.nameEn != string.Empty)
                element.Title = actor.nameEn;
            else return false;
            if (actor.description == null || actor.description == string.Empty || actor.description.Length >= 80)
                element.Description = "Безымянный";
            else
                element.Description = actor.description;
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.ActorPosterID(actor);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }
            

        }

        public static MessageTemplate ActorResultsTV(ActorsTemplate actors, IEnumerable<ActorResults.Actor> results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            var l = new List<Task>();
            foreach(var actor in results)
            {
                var t = Task.Run(() =>
                {
                    CarouselElement template_part = null;
                    if (CarouselActorsTV(actor, ref template_part))
                    {
                        arr.Add(template_part);
                        actors.ActorsIDs[actor.staffId.ToString()] = actor.posterUrl;
                    }
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());
            carousel.Elements = arr;

            return carousel;
        }
        public static bool CarouselActorsTV(ActorResults.Actor actor, ref CarouselElement template_part)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Узнать больше", $"t;;;{actor.staffId};;;", Positive, "text");
            var element = new CarouselElement();
            if (actor.nameRu != null && actor.nameRu != string.Empty)
                element.Title = actor.nameRu;
            else if (actor.nameEn != null && actor.nameEn != string.Empty)
                element.Title = actor.nameEn;
            else return false;
            if (actor.description == null || actor.description == string.Empty)
                element.Description = "Безымянный";
            else
                element.Description = actor.description;
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.ActorPosterID(actor);
            if (element.PhotoId == null)
                return false;
            else
            {
                template_part = element;
                return true;
            }


        }


        //------------------"Carousel Film -----------------------------------------------
        public static CarouselElement CarouselFilm(RandomFilms.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = film.VKPhotoID;
            return element;

        }
        //------------------"Рандомный фильм" для не мобильного приложения -----------------------------------------------

        public static void FilmMyRandomMessage(User user, IEnumerable<RandomFilms.Film> farray, bool b = true)
        {
            Bot.SendMessage(user, "Результаты поиска");
            foreach (var film in farray)
            {
                var message_part = FilmRandomMessage(film, out var photo, out var more);
                Bot.SendMessage(user, message_part, more, null, new List<MediaAttachment> { photo });
                user.Posters[film.filmId.ToString()] = (film.VKPhotoID_2, DateTime.Now);
            }
        }

        public static string FilmRandomMessage(RandomFilms.Film film, out Photo photo, out MessageKeyboard more)
        {

            photo = Bot.private_vkapi.Photo.GetById(new string[] { film.VKPhotoID_2 })[0];

            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            more = button.Build();

            string str = film.nameRu + "\nЖанр: " + string.Join(',', film.genres.Select(g => g.genre));
            return str;
        }

        /*public static void RandomFilmResultsMessage(User user, RandomFilms.Results results)
        {
            if (results == null || results.pagesCount == 0)
            {
                FilmMyRecommendationsMessage(user, PopularFilms.Shuffle().Take(3).Select(kv => kv.Value),false);
                Console.WriteLine("Костыль"); 
                return;
            }
           
            IEnumerable<RandomFilms.Film> films = results.films.Shuffle().Take(3);

            var arr = new List<(string, Photo, MessageKeyboard)>();
            // паралельно создаём три фильма для рандома
            Parallel.ForEach(films, (film) =>
            {
               // string message_part = null;//название,жанры одного фильма/сообщения
                if (MessageRandomFilmResult(user, film, out var message_part, out var photo, out var more))
                    arr.Add((message_part, photo, more));
            });
            
            if (arr.Count != 0)
            {
                Bot.SendMessage(user, "Результаты поиска");
                foreach(var m in arr)
                {
                    //Bot.attachments = new List<MediaAttachment> { m.Item2 };
                    //Bot.keyboard = m.Item3;
                    Bot.SendMessage(user, m.Item1, m.Item3, null, new List<MediaAttachment> { m.Item2 });
                }
            }
            else
            {
                FilmMyRecommendationsMessage(user, PopularFilms.Shuffle().Take(3).Select(kv => kv.Value), false);
                Console.WriteLine("Костыль");
            }

            
        }*/
        /// <summary>
        /// Возвращает один элемент карусели рандомных фильмов
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        /*public static bool MessageRandomFilmResult(User user, RandomFilms.Film film, out string message_part, out Photo photo, out MessageKeyboard grok)
        {
            message_part = null;
            photo = null;
            grok = null;
            if (!Attachments.PosterObject(user, film.posterUrl, film.filmId.ToString(), out photo))
                return false;
            //создаём клавиатуру
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"f;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            grok = button.Build();
            //готовим сообщение
            message_part = " " + film.nameRu   +"\nЖанр: " + string.Join(", ", film.genres.Select(g => g.genre));
            //добро пожаловать в Рандом
            return true;
        }*/



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
        /*public static MessageKeyboard RandomFilm(string nameRu, string nameEn, string filmID, string date, string genres, string digital_release)
        {
            return FilmSearch(nameRu, nameEn, filmID, date, genres, digital_release);
        }*/

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
            var result = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            result.Clear();
            //"Поиск По названию и По жанрам"
            result.AddButton("По названию", "Command", Primary, "text");
            result.AddButton("По жанру", "Command", Primary, "text");
            result.AddLine();
            //"Рекомендованно и В планах"
            result.AddButton("Рекомендовано", "Command", Primary, "text");
            result.AddButton("Запланировано", "Command", Primary, "text");
            result.AddLine();
            //"Рандомный фильм"
            result.AddButton("Случайный сериал", "Command", Primary, "text");
            result.AddLine();
            //"Назад"
            result.AddButton("Помощь", "Command", Positive, "text");
            result.AddButton("Назад", "Command", Negative, "text");

            return result.Build();
        }

        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Сериалы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>     
        public static MessageKeyboard TVSearch(string nameRu, string nameEn, string filmID, string genres, string date)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", $"t;{nameRu};{nameEn};{filmID};;;", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", $"t;;{nameEn};{filmID};;;", Primary, "text");
            button.AddLine();
            if (ServiceClass.service_data.google_requests < 100 && date != null && DateTime.Now.CompareTo(User.StringToDate(date)) >= 0)
            {
                button.AddButton("Смотреть", $"t;{nameRu};;;{date};;", Primary, "text");
                
            }
            button.AddButton("Актеры", $"t;;;{filmID};;;", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", $"t;{nameRu};{nameEn};;;;", Primary, "text");
            button.AddButton("Еда", $"t;;;;;{genres};", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", $"t;;;{filmID};;;", Negative, "text");

            button.SetInline();
            return button.Build();
        }



        //"Сериалы"->"Мои рекомендации"

        //---------Сериалы--------------------"Мои рекомендации для мобильного приложения"---------------------------------------

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
            //
            
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
        //Не (mobile online)
        //---------Сериалы--------------------"Мои рекомендации для не мобильного приложения"---------------------------------------

        public static void TVMyRecommendationsMessage(User user, IEnumerable<TVObject> tvs)
        {
            Bot.SendMessage(user, "Результаты поиска");
            foreach (var film in tvs)
            {
                var message_part = TVMessage(film, out var more, out var photo);
                Bot.SendMessage(user, message_part, more, null, new List<MediaAttachment> { photo });
            }
        }
        public static string TVMessage(TVObject tv, out MessageKeyboard keyboard, out Photo photo)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"t;;;{tv.data.filmId};;;", Positive, "text");
            button.SetInline();
            keyboard = button.Build();
            photo = Bot.private_vkapi.Photo.GetById(new string[] { tv.data.VKPhotoID_2 })[0];

            string str = tv.data.nameRu.Replace("(сериал)","") + "\nЖанр: " + string.Join(',', tv.data.genres.Select(g => g.genre));
            return str;

        }

        //---Сериалы-------------------------------------------------------------------------------------------------------------------------------------


        //-----Сериалы-------------"Поиск по названию" для мобильного приложения -----------------------------------------------

        public static MessageTemplate TVResults(User user, TVResults.Results results)
        {
            IEnumerable<TVResults.Film> films = results.films.Where(f => f.nameRu.EndsWith("(сериал)") || f.nameRu.EndsWith("(мини-сериал)")).Take(3);
            
            var arr = new List<CarouselElement>();
            var l = new List<Task>();
            foreach (var film in films)
            {
                var t = Task.Run(() =>
                {
                    Console.WriteLine($"Выполняю загрузку {film.filmId}...");
                    CarouselElement template_part = null;
                    if (CarouselTVResult(film, ref template_part))
                    {
                        user.AddSearchPosterAsync(film).ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                        //if (arr.Count < 3)
                        arr.Add(template_part);
                        //else
                        //state.Break();
                    }
                    Console.WriteLine($"Выполнил загрузку {film.filmId}!");
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
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
        //

        //-----Сериалы-------------"Поиск по названию" для не мобильного приложения -----------------------------------------------

        public static void TVResultsMessage(User user, TVResults.Results results)
        {
            IEnumerable<TVResults.Film> films = results.films.Where(f => f.nameRu.EndsWith("(сериал)") || f.nameRu.EndsWith("(мини-сериал)")).Take(3);

            var arr = new List<(string, Photo, MessageKeyboard)>();
            var l = new List<Task>();
            foreach (var film in films)
            {
                var t = Task.Run(() =>
                {
                    Console.WriteLine($"Выполняю загрузку {film.filmId}...");
                    if (MessageTVResult(user, film, out var message_part, out Photo photo, out var more))
                        arr.Add((message_part, photo, more));
                    Console.WriteLine($"Выполнил загрузку {film.filmId}!");
                });
                l.Add(t);
            }
            Task.WaitAll(l.ToArray());

            if (arr.Count != 0)
            {
                Bot.SendMessage(user, "Результаты поиска");
                foreach(var m in arr)
                    Bot.SendMessage(user, m.Item1, m.Item3, null, new List<MediaAttachment> { m.Item2 });
            }
            else
                Bot.SendMessage(user, "К сожалению, я не смог найти такой сериал... 😔");
        }




        public static bool MessageTVResult(User user, TVResults.Film film, out string message_part, out Photo photo, out MessageKeyboard more)
        {
            message_part = null; photo = null; more = null;
            if (film.filmId.ToString() == null)
                return false;
            var s = Attachments.FullResultedTVPosterID(film, out var p);
            if (s == null)
                return false;
            photo = p;
            user.Posters[film.filmId.ToString()] = (s, DateTime.Now);
            //создаём клавиатуру
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"t;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            more = button.Build();
            //готовим сообщение
            message_part = " " + film.nameRu.Replace("(сериал)", "") + "\nЖанр: " + string.Join(", ", film.genres.Select(g => g.genre));
            //Начинаем Поиск по названию
            return true;

        }
        //---------------------------------------------------------------------------------------------------------------




        //----Сериалы--------------"Рандомный сериал" для  мобильного приложения -----------------------------------------------
        /*public static MessageTemplate RandomTVResults(RandomTV.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            //проверка
            if (results == null || results.pagesCount == 0)
            {
                carousel = TVMyRecommendations(PopularTV.Shuffle().Take(3).Select(kv => kv.Value));
                Console.WriteLine("Костыль_1");
                return carousel;
            }

            IEnumerable<RandomTV.Film> films = results.films.Shuffle().Take(3);

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
                Console.WriteLine("Костыль_2");
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

        }*/

        public static MessageTemplate RandomTVResults(User user, IEnumerable<RandomTV.Film> results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            foreach (RandomTV.Film f in results)
            {
                user.Posters[f.filmId.ToString()] = (f.VKPhotoID_2, DateTime.Now);
                arr.Add(CarouselFilm(f));
            }
            carousel.Elements = arr;

            return carousel;
        }

        public static CarouselElement CarouselFilm(RandomTV.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"t;;;{film.filmId};;;", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu.Replace("(сериал)", "");
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = film.VKPhotoID;
            return element;

        }

        //----Сериалы--------------"Рандомный сериал" для не мобильного приложения" -----------------------------------------------

        public static void RandomTVResultsMessage(User user, IEnumerable<RandomTV.Film> films)
        {
            Bot.SendMessage(user, "Результаты поиска");
            foreach (var film in films)
            {
                MessageRandomTVResult(user, film, out var message_part, out var photo, out var more);
                Bot.SendMessage(user, message_part, more, null, new List<MediaAttachment> { photo });
                user.Posters[film.filmId.ToString()] = (film.VKPhotoID_2, DateTime.Now);
            }
        }
        /// <summary>
        /// Возвращает один элемент карусели рандомных фильмов
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static void MessageRandomTVResult(User user, RandomTV.Film film, out string message_part, out Photo photo, out MessageKeyboard grok)
        {
            photo = Bot.private_vkapi.Photo.GetById(new string[] { film.VKPhotoID_2 })[0];

            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", $"t;;;{film.filmId};;;", Positive, "text");
            button.SetInline();
            grok = button.Build();

            message_part = film.nameRu.Replace("(сериал)", "") + "\nЖанр: " + string.Join(',', film.genres.Select(g => g.genre));
        }



        //---------------------------------------------------------------------------------------------------------------------------------------------




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
        /*public static MessageKeyboard RandomTV(string nameRu, string nameEn, string filmID, string genres)
        {
            return TVSearch(nameRu, nameEn, filmID, genres, "");
        }*/

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
            button.AddButton("Режим питания", "Command", Primary, "text");
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

        public static MessageKeyboard DietMode()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Без ограничений", $"o;;;;;;", Default, "text");
            button.AddLine();
            button.AddButton("Здоровое питание", $"o;;;;;;", Default, "text");
            button.AddLine();

            button.SetInline();
            return button.Build();
        }

        public static void Init()
        {
            MainKeyboard = MainMenu();
            FilmKeyboard = Film();
            TVKeyboard = TV();
            FoodKeyboard = Food();
            GenresKeyboard = SearchGenreList();
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