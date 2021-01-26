using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VkNet.Model.Attachments;
using VkNet.Model.Template;
using VkNet.Model.Keyboard;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;
using System.Linq;
using VkNet.Enums.SafetyEnums;

namespace Freetime_Planner
{
    public static class Film
    {
        /// <summary>
        /// Вспомогательный класс для Data, хранящий стрну производства фильма
        /// </summary>
        public class Country
        {
            public string country { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для Data, хранящий жанр фильма
        /// </summary>
        public class Genre
        {
            public string genre { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для FilmObject, хранящий основную информацию о фильме
        /// </summary>
        public class Data
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string webUrl { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
            public string year { get; set; }
            public string filmLength { get; set; }
            public string slogan { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public string ratingMpaa { get; set; }
            public int ratingAgeLimits { get; set; }
            public string premiereRu { get; set; }
            public string distributors { get; set; }
            public string premiereWorld { get; set; }
            public object premiereDigital { get; set; }
            public string premiereWorldCountry { get; set; }
            public string premiereDvd { get; set; }
            public string premiereBluRay { get; set; }
            public string distributorRelease { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public List<string> facts { get; set; }
            public List<object> seasons { get; set; }
            public string VKPhotoID { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для FilmObject, хранящий ID фильма на сайте IMDB
        /// </summary>
        public class ExternalId
        {
            public string imdbId { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для FilmObject, хранящий бюджет фильма
        /// </summary>
        public class Budget
        {
            public int grossRu { get; set; }
            public int grossUsa { get; set; }
            public int grossWorld { get; set; }
            public string budget { get; set; }
            public object marketing { get; set; }
        }

        /// <summary>
        /// Объект фильма, который создается из ответа сайта Кинопоиск на запрос фильма по его ID
        /// </summary>
        public class FilmObject
        {
            public Data data { get; set; }
            public ExternalId externalId { get; set; }
            public Budget budget { get; set; }
            public int Priority { get; set; }

            public FilmObject(string nameRu, string nameEn, string date)
            {
                data = new Data();
                data.nameRu = nameRu;
                data.nameEn = nameEn;
                data.premiereRu = date;
            }
        }

        /// <summary>
        /// Методы по работе с фильмами
        /// </summary>
        public static class Methods
        {
            /// <summary>
            /// Возвращает подробную информацию по ID фильма (отправляет запрос на Кинопоиск)
            /// </summary>
            /// <param name="filmID"></param>
            /// <returns></returns>
            public static string FullInfo(int filmID)
            {
                //TODO
                return null;
                //присвоить поля attachments и keyboard
            }

            /// <summary>
            /// Возвращает подробную информацию по фильму из его объекта (просто переводит в текстовый вид)
            /// </summary>
            /// <param name="film"></param>
            /// <returns></returns>
            public static string FullInfo(Film.FilmObject film)
            {
                //TODO
                return null;
            }

            /// <summary>
            /// Возвращает карусель из фильмов, которые были получены в результате поиска фильма по названию (используется класс FilmResults)
            /// </summary>
            /// <param name="filmName"></param>
            /// <returns></returns>
            public static MessageTemplate Search(string filmName)
            {
                FilmResults.Results results = null; 
                //TODO
                return Keyboards.FilmResults(results);
            }

            /// <summary>
            /// Возвращает карусель из фильмов, которые были получены в результате случайного поиска фильма (используется класс FilmResults)
            /// </summary>
            /// <returns></returns>
            public static MessageTemplate Random()
            {
                RandomFilms.Results results = null; 
                //TODO
                return Keyboards.RandomFilmResults(results);
            }

            /// <summary>
            /// Возвращает список аудиозаписей по названию фильма
            /// </summary>
            /// <param name="filmName"></param>
            /// <returns></returns>
            public static List<Audio> Soundtrack(string filmName)
            {
                return null;
            }

            /// <summary>
            /// Возвращает видеорецепт блюда по жанрам фильма
            /// </summary>
            /// <param name="genres"></param>
            /// <returns></returns>
            public static Video Food(string[] genres)
            {
                return null;
            }

            /// <summary>
            /// Возвращает клавиатуру с ссылками на сервисы для просмотра фильма
            /// </summary>
            /// <param name="filmName"></param>
            /// <param name="year"></param>
            /// <returns></returns>
            public static MessageKeyboard ServiceLinks(string filmName, string year)
            {
                return null;
            }
        }
    }




    /// <summary>
    /// Объект, который создается из ответа Кинопоиска на запрос фильма по его названию
    /// </summary>
    public static class FilmResults
    {
        /// <summary>
        /// Вспомогательный класс для Film
        /// </summary>
        public class Country
        {
            public string country { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для Film
        /// </summary>
        public class Genre
        {
            public string genre { get; set; }
        }

        /// <summary>
        /// Вспомогательный класс для Results, хранящий основную информацию по фильму
        /// </summary>
        public class Film
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string type { get; set; }
            public string year { get; set; }
            public string description { get; set; }
            public string filmLength { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public string rating { get; set; }
            public int ratingVoteCount { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
        }

        /// <summary>
        /// Класс, хранящий результаты поиска
        /// </summary>
        public class Results
        {
            public string keyword { get; set; }
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
            public int searchFilmsCountResult { get; set; }
        }
    }




    /// <summary>
    /// Объект, который создается из ответа Кинопоиска на запрос случайного фильма (аналогичен FilmResults)
    /// </summary>
    public static class RandomFilms
    {
        public class Country
        {
            public string country { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class Film
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string type { get; set; }
            public string year { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public string rating { get; set; }
            public int ratingVoteCount { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
            public string nameEn { get; set; }
        }

        public class Results
        {
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
        }
    }
}
