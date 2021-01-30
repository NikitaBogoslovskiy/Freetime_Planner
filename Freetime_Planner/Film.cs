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
using Newtonsoft.Json;
using RestSharp;
using System.Security.Claims;

namespace Freetime_Planner
{
    public static class Film
    {
        public static string APIKey = "cfcf375a-ec0e-4bd3-9070-0c58fe97e43c";

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
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/" + filmID.ToString());
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", APIKey);
                IRestResponse response = client.Execute(request);

                var pro = JsonConvert.DeserializeObject<FilmObject>(response.Content);
                return FullInfo(pro);
                //присвоить поля attachments и keyboard
            }

            /// <summary>
            /// Возвращает подробную информацию по фильму из его объекта (просто переводит в текстовый вид)
            /// </summary>
            /// <param name="film"></param>
            /// <returns></returns>
            public static string FullInfo(Film.FilmObject film)
            {
                Data filmData = film.data;

                string res = $"Название: {filmData.nameRu} / {filmData.nameEn}\n" +
                    $"Описание: {filmData.description}\n" +
                    $"Жанры: {string.Join(", ", filmData.genres.Select(x => x.genre))}\n" +
                    $"Год выпуска: {ConvertIntIntoEmojis(filmData.year)}\n" +
                    $"Длительность: {ConvertIntIntoEmojis(':', filmData.filmLength)}\n" +
                    $"Возрастное ограничение: {filmData.ratingAgeLimits}\n" +
                    $"Ссылка: {filmData.webUrl}\n";

                return res;
            }

            #region Методы для превращения цифр в эмодзи

            /// Реверсирует int
            private static int ReverseInt(int num)
            {
                int reverse = 0;
                while (num != 0)
                {
                    int digit = num % 10;
                    reverse = reverse * 10 + digit;
                    num /= 10;
                }
                return reverse;
            }

            private static string ConvertIntIntoEmojis(string value)
            {
                string result = "";
                int reversedValue = ReverseInt(int.Parse(value));
                while (reversedValue != 0)
                {
                    int digit = reversedValue % 10;
                    result += digit.ToString() + "&#8419;";
                    reversedValue /= 10;
                }
                return result;
            }

            private static string ConvertIntIntoEmojis(char separator, string value)
            {
                string result = "";
                string[] tempStrs = value.Split(separator);

                result += ConvertIntIntoEmojis(tempStrs[0]) + "&#10135;" + ConvertIntIntoEmojis(tempStrs[1]);
                return result;
            }
            #endregion

            /// <summary>
            /// Возвращает карусель из фильмов, которые были получены в результате поиска фильма по названию (используется класс FilmResults)
            /// </summary>
            /// <param name="filmName"></param>
            /// <returns></returns>
            public static MessageTemplate Search(string filmName)
            {
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", APIKey);
                request.AddQueryParameter("keyword", filmName);
                IRestResponse response = client.Execute(request);
                FilmResults.Results pro = JsonConvert.DeserializeObject<FilmResults.Results>(response.Content);

                return Keyboards.FilmResults(pro);
            }

            /// <summary>
            /// Возвращает карусель из фильмов, которые были получены в результате случайного поиска фильма (используется класс FilmResults)
            /// </summary>
            /// <returns></returns>
            public static MessageTemplate Random()
            {
                RandomFilms.Results results = null;

                Random random = new Random();
                int filmYearBottomLine = random.Next(1950, DateTime.Now.AddYears(-6).Year);
                int filmYearUpperLine = random.Next(filmYearBottomLine + 5, DateTime.Now.Year);
                int filmRatingBottomLine = random.Next(4, 7);

                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", APIKey);
                request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                request.AddQueryParameter("ratingFrom", filmRatingBottomLine.ToString());
                IRestResponse response = client.Execute(request);

                results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content);
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
