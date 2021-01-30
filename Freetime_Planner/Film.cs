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
using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using Yandex.Music.Api;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Claims;

namespace Freetime_Planner
{
    public static class Film
    {
        //Популярные фильмы
        #region PopularFilms
        public static Dictionary<int, FilmObject> PopularFilms { get; set; }
        public static string PopularFilmsPath = "PopularFilms.json";
        public static DateTime LastPopularFilmsUpdate { get; set; }

        /// <summary>
        /// Загружает список популярных фильмов из json-файла в поле PopularFilms. Если такого json-файла нет, то создает и выгружает список
        /// </summary>
        public static void UploadPopularFilms()
        {
            if (!File.Exists(PopularFilmsPath))
            {
                UpdatePopularFilms();
                LastPopularFilmsUpdate = DateTime.Now;
                UnloadPopularFilms();
            }
            else
            {
                try
                {
                    var pair = JsonConvert.DeserializeObject<KeyValuePair<DateTime, Dictionary<int, FilmObject>>>(File.ReadAllText(PopularFilmsPath));
                    LastPopularFilmsUpdate = pair.Key;
                    PopularFilms = pair.Value;
                }
                catch (Exception e)
                {
                    UpdatePopularFilms();
                    LastPopularFilmsUpdate = DateTime.Now;
                    UnloadPopularFilms();
                }
            }
        }

        /// <summary>
        /// Выгружает список популярных фильмов из PopularFilms в json-файл
        /// </summary>
        public static void UnloadPopularFilms() => File.WriteAllText(PopularFilmsPath, JsonConvert.SerializeObject(new KeyValuePair<DateTime, Dictionary<int, FilmObject>>(LastPopularFilmsUpdate, PopularFilms)));

        /// <summary>
        /// Обновляет список популярных фильмов
        /// </summary>
        public static void UpdatePopularFilms()
        {
            var res = new Dictionary<int, FilmObject>();

            //добавление первой страницы фильмов
            var clientA = new RestSharp.RestClient("https://api.themoviedb.org/3/movie/popular");
            var requestA = new RestRequest(Method.GET);
            requestA.AddQueryParameter("api_key", Bot._mdb_key);
            requestA.AddQueryParameter("page", "1");
            IRestResponse responseA = clientA.Execute(requestA);
            var list = JsonConvert.DeserializeObject<MDBResults>(responseA.Content).results;

            //добавление второй страницы фильмов
            var clientB = new RestSharp.RestClient("https://api.themoviedb.org/3/movie/popular");
            var requestB = new RestRequest(Method.GET);
            requestB.AddQueryParameter("api_key", Bot._mdb_key);
            requestB.AddQueryParameter("page", "2");
            IRestResponse responseB = clientA.Execute(requestB);

            //объединение двух списков-страниц в один список
            list.AddRange(JsonConvert.DeserializeObject<MDBResults>(responseB.Content).results);

            //параллельный обход списка
            Parallel.ForEach(list, (result) =>
            {
                //запрос фильма по его названию
                var KPclient1 = new RestSharp.RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var KPrequest1 = new RestRequest(Method.GET);
                KPrequest1.AddHeader("X-API-KEY", Bot._kp_key);
                KPrequest1.AddHeader("accept", "application/json");
                KPrequest1.AddQueryParameter("keyword", result.original_title);
                var KPresponse1 = KPclient1.Execute(KPrequest1);
                var deserialized = JsonConvert.DeserializeObject<FilmResults.Results>(KPresponse1.Content);
                
                //проверка успешности десериализации
                if (deserialized.pagesCount > 0)
                {
                    //выбор фильма из результатов ответа на запрос, такого, что это не сериал и что его еще нет в списке
                    int id = 0;
                    foreach (var f in deserialized.films)
                        if (!f.nameRu.EndsWith("(сериал)") && !res.ContainsKey(f.filmId))
                        {
                            id = f.filmId;
                            break;
                        }
                    //запрос выбранного фильма по его ID
                    var KPclient2 = new RestSharp.RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{id}");
                    var KPrequest2 = new RestRequest(Method.GET);
                    KPrequest2.AddHeader("X-API-KEY", Bot._kp_key);
                    KPrequest2.AddHeader("accept", "application/json");
                    var KPresponse2 = KPclient2.Execute(KPrequest2);
                    var film = JsonConvert.DeserializeObject<Film.FilmObject>(KPresponse2.Content);
                    film.Priority = 1;
                    film.data.VKPhotoID = Attachments.PopularFilmObjectPosterID(film);
                    //проверка валидности загруженной фотографии
                    if (film.data.VKPhotoID != null)
                        res[id] = film;
                }
            });
            PopularFilms = res;
        }
        #endregion

        //Класс, необходимый для десериализации ответа с Кинопоиска при поиске фильма по его ID
        #region FilmObject

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
            public string ratingAgeLimits { get; set; }
            public string premiereRu { get; set; }
            public string distributors { get; set; }
            public string premiereWorld { get; set; }
            public string premiereDigital { get; set; }
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
        #endregion

        //Методы для работы с фильмами
        #region Methods
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
            public static bool Soundtrack(string filmName, string year, List<Audio> audios)
            {
                Yandex.Music.Api.Models.YandexAlbum album = null;
                try
                {
                    album = Bot.yandex_api.GetAlbum(Bot.yandex_api.SearchAlbums($"{filmName} {year} film").First(album => album.TrackCount >= 4).Id);
                }
                catch(Exception e)
                {
                    return false;
                }
                var tracks = album.Volumes[0].Take(Math.Min(6, album.TrackCount.Value));
                var song_names = tracks.Select(track => track.Title + " " + string.Join(" ", track.Artists.Select(artist => artist.Name))).ToArray();
                Parallel.For(0, song_names.Length, (i, state) =>
                {
                    var collection = private_vkapi.Audio.Search(new VkNet.Model.RequestParams.AudioSearchParams
                    {
                        Autocomplete = true,
                        Query = song_names[i]
                    });
                    if (collection.Count > 0)
                    {
                        audios.Add(collection[0]);
                    }
                });
                return true;
            }

            /// <summary>
            /// Возвращает видеорецепт блюда по жанрам фильма
            /// </summary>
            /// <param name="genres"></param>
            /// <returns></returns>
            public static Video Food(string[] genres)
            {
                Random r = new Random();
                WebClient wc = new WebClient();
                var client = new RestSharp.RestClient("https://www.googleapis.com/youtube/v3/search");
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("key", Bot._youtube_key);
                request.AddQueryParameter("part", "snippet");
                var meal = Freetime_Planner.Food.GenreFood[genres[r.Next(0, genres.Length)]];
                request.AddQueryParameter("q", meal[r.Next(0, meal.Length)]);
                request.AddQueryParameter("videoDuration", "short");
                request.AddQueryParameter("type", "video");
                IRestResponse response = client.Execute(request);
                var results = JsonConvert.DeserializeObject<YouTube.YouTubeResults>(response.Content);

                var video = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
                {
                    Link = $"https://www.youtube.com/watch?v={results.items[r.Next(0, 5)].id.videoId}"
                });
                wc.DownloadString(video.UploadUrl);
                return video;
            }

            /// <summary>
            /// Возвращает клавиатуру с ссылками на сервисы для просмотра фильма
            /// </summary>
            /// <param name="filmName"></param>
            /// <param name="year"></param>
            /// <returns></returns>
            public static MessageKeyboard ServiceLinks(string filmName, string date, string digital_release)
            {
                var client = new RestSharp.RestClient("https://www.googleapis.com/customsearch/v1");
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("key", _google_key);
                request.AddQueryParameter("cx", _google_sid);
                request.AddQueryParameter("q", $"{filmName} {date.Substring(0,4)} смотреть");
                request.AddQueryParameter("num", "10");
                IRestResponse response = client.Execute(request);
                ServiceClass.service_data.IncGoogleRequests();
                var results = JsonConvert.DeserializeObject<GoogleResponse>(response.Content);
                var dict = new Dictionary<string, string>();
                foreach(var item in results.items)
                {
                    if (Regex.IsMatch(item.link, @"https://www.ivi.ru/watch/\d+$") && !dict.ContainsKey("IVI"))
                        dict["IVI"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://megogo.net/ru/view/.+") && !dict.ContainsKey("MEGOGO"))
                        dict["MEGOGO"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://okko.tv/movie/.+") && !dict.ContainsKey("OKKO"))
                        dict["OKKO"] = item.link;
                    if (dict.Count == 3)
                        break;
                }
                return Keyboards.ServiceLinks(dict);
            }
        }
        #endregion
    }

    //Класс, необходимый для десериализации ответа с Кинопоиска при поиске фильма по названию
    #region FilmResults
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
    #endregion

    //Класс, необходимый для десериализации ответа с Кинопоиска при поиске фильма случайным образом
    #region RandomFilms
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
    #endregion

    //Класс, необходимый для десериализации ответа с MDB при поиске популярных или рекомендуемых фильмов
    #region MDBResults
    public class MDBFilm
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class MDBResults
    {
        public int page { get; set; }
        public List<MDBFilm> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
    #endregion

    #region GoogleResults

    public class Url
    {
        public string type { get; set; }
        public string template { get; set; }
    }

    public class Request
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class NextPage
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class Queries
    {
        public List<Request> request { get; set; }
        public List<NextPage> nextPage { get; set; }
    }

    public class Context
    {
        public string title { get; set; }
    }

    public class SearchInformation
    {
        public double searchTime { get; set; }
        public string formattedSearchTime { get; set; }
        public string totalResults { get; set; }
        public string formattedTotalResults { get; set; }
    }

    public class CseThumbnail
    {
        public string src { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Organization
    {
        public string name { get; set; }
        public string logo { get; set; }
        public string url { get; set; }
    }

    public class Metatag
    {
        [JsonProperty("application-name")]
        public string ApplicationName { get; set; }
        [JsonProperty("msapplication-starturl")]
        public string MsapplicationStarturl { get; set; }
        [JsonProperty("og:image")]
        public string OgImage { get; set; }
        [JsonProperty("msapplication-config")]
        public string MsapplicationConfig { get; set; }
        [JsonProperty("theme-color")]
        public string ThemeColor { get; set; }
        [JsonProperty("og:type")]
        public string OgType { get; set; }
        [JsonProperty("ya:ovs:upload_date")]
        public DateTime YaOvsUploadDate { get; set; }
        [JsonProperty("video:duration")]
        public string VideoDuration { get; set; }
        [JsonProperty("og:title")]
        public string OgTitle { get; set; }
        [JsonProperty("apple-mobile-web-app-title")]
        public string AppleMobileWebAppTitle { get; set; }
        [JsonProperty("og:description")]
        public string OgDescription { get; set; }
        [JsonProperty("msapplication-navbutton-color")]
        public string MsapplicationNavbuttonColor { get; set; }
        [JsonProperty("apple-mobile-web-app-status-bar-style")]
        public string AppleMobileWebAppStatusBarStyle { get; set; }
        [JsonProperty("og:video")]
        public string OgVideo { get; set; }
        public string viewport { get; set; }
        [JsonProperty("apple-mobile-web-app-capable")]
        public string AppleMobileWebAppCapable { get; set; }
        public string mrc__share_title { get; set; }
        [JsonProperty("mobile-web-app-capable")]
        public string MobileWebAppCapable { get; set; }
        [JsonProperty("og:video:type")]
        public string OgVideoType { get; set; }
        [JsonProperty("og:url")]
        public string OgUrl { get; set; }
        [JsonProperty("ya:ovs:allow_embed")]
        public string YaOvsAllowEmbed { get; set; }
        [JsonProperty("ya:ovs:adult")]
        public string YaOvsAdult { get; set; }
        [JsonProperty("ya:ovs:country")]
        public string YaOvsCountry { get; set; }
        [JsonProperty("ya:ovs:comments")]
        public string YaOvsComments { get; set; }
        [JsonProperty("ya:ovs:rating")]
        public string YaOvsRating { get; set; }
        public string title { get; set; }
        [JsonProperty("ya:ovs:dislikes")]
        public string YaOvsDislikes { get; set; }
        [JsonProperty("ya:ovs:person:role")]
        public string YaOvsPersonRole { get; set; }
        [JsonProperty("ya:ovs:content_id")]
        public string YaOvsContentId { get; set; }
        [JsonProperty("ya:ovs:category")]
        public string YaOvsCategory { get; set; }
        [JsonProperty("ya:ovs:subtitle:language")]
        public string YaOvsSubtitleLanguage { get; set; }
        [JsonProperty("ya:ovs:person")]
        public string YaOvsPerson { get; set; }
        [JsonProperty("og:restrictions:age")]
        public string OgRestrictionsAge { get; set; }
        [JsonProperty("ya:ovs:genre")]
        public string YaOvsGenre { get; set; }
        [JsonProperty("ya:ovs:languages")]
        public string YaOvsLanguages { get; set; }
        [JsonProperty("ya:ovs:likes")]
        public string YaOvsLikes { get; set; }
        [JsonProperty("twitter:card")]
        public string TwitterCard { get; set; }
        [JsonProperty("al:android:package")]
        public string AlAndroidPackage { get; set; }
        [JsonProperty("msapplication-tileimage")]
        public string MsapplicationTileimage { get; set; }
        [JsonProperty("al:ios:url")]
        public string AlIosUrl { get; set; }
        [JsonProperty("al:ios:app_store_id")]
        public string AlIosAppStoreId { get; set; }
        [JsonProperty("twitter:image")]
        public string TwitterImage { get; set; }
        [JsonProperty("twitter:player")]
        public string TwitterPlayer { get; set; }
        [JsonProperty("twitter:player:height")]
        public string TwitterPlayerHeight { get; set; }
        [JsonProperty("twitter:site")]
        public string TwitterSite { get; set; }
        [JsonProperty("og:video:duration")]
        public string OgVideoDuration { get; set; }
        [JsonProperty("og:video:height")]
        public string OgVideoHeight { get; set; }
        [JsonProperty("msapplication-tilecolor")]
        public string MsapplicationTilecolor { get; set; }
        [JsonProperty("x-ua-compatible")]
        public string XUaCompatible { get; set; }
        [JsonProperty("twitter:title")]
        public string TwitterTitle { get; set; }
        [JsonProperty("al:ios:app_name")]
        public string AlIosAppName { get; set; }
        [JsonProperty("og:video:width")]
        public string OgVideoWidth { get; set; }
        [JsonProperty("al:android:url")]
        public string AlAndroidUrl { get; set; }
        [JsonProperty("twitter:description")]
        public string TwitterDescription { get; set; }
        [JsonProperty("twitter:player:width")]
        public string TwitterPlayerWidth { get; set; }
        [JsonProperty("al:android:app_name")]
        public string AlAndroidAppName { get; set; }
    }

    public class CseImage
    {
        public string src { get; set; }
    }

    public class Listitem
    {
        public string item { get; set; }
        public string name { get; set; }
        public string position { get; set; }
    }

    public class Website
    {
        public string image { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string sameas { get; set; }
    }

    public class Movie
    {
        public string duration { get; set; }
        public string isfamilyfriendly { get; set; }
        public string name { get; set; }
        public string genre { get; set; }
        public string datepublished { get; set; }
    }

    public class Aggregaterating
    {
        public string ratingvalue { get; set; }
        public string ratingcount { get; set; }
        public string worstrating { get; set; }
        public string bestrating { get; set; }
    }

    public class Moviereview
    {
        public string ratingstars { get; set; }
        public string release_date { get; set; }
        public string name { get; set; }
        public string release_year { get; set; }
        public string genre { get; set; }
        public string runtime { get; set; }
        public string best { get; set; }
        public string votes { get; set; }
        public string originalrating { get; set; }
    }

    public class Searchaction
    {
        [JsonProperty("query-input")]
        public string QueryInput { get; set; }
        public string target { get; set; }
    }

    public class Pagemap
    {
        public List<CseThumbnail> cse_thumbnail { get; set; }
        public List<Organization> organization { get; set; }
        public List<Metatag> metatags { get; set; }
        public List<CseImage> cse_image { get; set; }
        public List<Listitem> listitem { get; set; }
        public List<Website> website { get; set; }
        public List<Movie> movie { get; set; }
        public List<Aggregaterating> aggregaterating { get; set; }
        public List<Moviereview> moviereview { get; set; }
        public List<Searchaction> searchaction { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string title { get; set; }
        public string htmlTitle { get; set; }
        public string link { get; set; }
        public string displayLink { get; set; }
        public string snippet { get; set; }
        public string htmlSnippet { get; set; }
        public string cacheId { get; set; }
        public string formattedUrl { get; set; }
        public string htmlFormattedUrl { get; set; }
        public Pagemap pagemap { get; set; }
    }

    public class GoogleResponse
    {
        public string kind { get; set; }
        public Url url { get; set; }
        public Queries queries { get; set; }
        public Context context { get; set; }
        public SearchInformation searchInformation { get; set; }
        public List<Item> items { get; set; }
    }

    #endregion
    /*
    #region MDBSearch
    public class MDBFilmObject
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class MDBFilmSearch
    {
        public int page { get; set; }
        public List<MDBFilmObject> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
    #endregion
    */
}
