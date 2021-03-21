using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.Template;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public static class TV
    {
        //Популярные сериалы
        #region PopularTV
        public static Dictionary<int, TVObject> PopularTV { get; set; }
        public static string PopularTVPath;
        public static DateTime LastPopularTVUpdate { get; set; }

        /// <summary>
        /// Загружает список популярных сериалов из json-файла в поле PopularTV. Если такого json-файла нет, то создает и выгружает список
        /// </summary>
        public static void UploadPopularTV()
        {
            if (!File.Exists(PopularTVPath))
            {
                UpdatePopularTV();
                LastPopularTVUpdate = DateTime.Now;
                UnloadPopularTV();
            }
            else
            {
                try
                {
                    var pair = JsonConvert.DeserializeObject<KeyValuePair<DateTime, Dictionary<int, TVObject>>>(File.ReadAllText(PopularTVPath));
                    LastPopularTVUpdate = pair.Key;
                    PopularTV = pair.Value;
                }
                catch (Exception)
                {
                    UpdatePopularTV();
                    LastPopularTVUpdate = DateTime.Now;
                    UnloadPopularTV();
                }
            }
        }

        /// <summary>
        /// Выгружает список популярных сериалов из PopularTV в json-файл
        /// </summary>
        public static void UnloadPopularTV() => File.WriteAllText(PopularTVPath, JsonConvert.SerializeObject(new KeyValuePair<DateTime, Dictionary<int, TVObject>>(LastPopularTVUpdate, PopularTV)));

        /// <summary>
        /// Обновляет список популярных сериалов
        /// </summary>
        public static void UpdatePopularTV()
        {
            var res = new Dictionary<int, TVObject>();

            //добавление первой страницы сериалов
            var clientA = new RestSharp.RestClient("https://api.tmdb.org/3/tv/popular");
            var requestA = new RestRequest(Method.GET);
            requestA.AddQueryParameter("api_key", Bot._mdb_key);
            requestA.AddQueryParameter("page", "1");
            IRestResponse responseA = clientA.Execute(requestA);
            var deserializedA = JsonConvert.DeserializeObject<MDBResultsTV>(responseA.Content);
            if (deserializedA == null || deserializedA.total_pages == 0)
                return;
            var list = deserializedA.results;

            //добавление второй страницы сериалов
            var clientB = new RestSharp.RestClient("https://api.tmdb.org/3/tv/popular");
            var requestB = new RestRequest(Method.GET);
            requestB.AddQueryParameter("api_key", Bot._mdb_key);
            requestB.AddQueryParameter("page", "2");
            IRestResponse responseB = clientA.Execute(requestB);
            var deserializedB = JsonConvert.DeserializeObject<MDBResultsTV>(responseB.Content);
            if (deserializedB != null && deserializedB.total_pages != 0)
                //объединение двух списков-страниц в один список
                list.AddRange(deserializedB.results);

            //параллельный обход списка
            foreach(var result in list)
            {
                //запрос сериала по его названию
                var KPclient1 = new RestSharp.RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var KPrequest1 = new RestRequest(Method.GET);
                KPrequest1.AddHeader("X-API-KEY", Bot._kp_key);
                KPrequest1.AddHeader("accept", "application/json");
                KPrequest1.AddQueryParameter("keyword", result.original_name);
                var KPresponse1 = KPclient1.Execute(KPrequest1);
                var deserialized = JsonConvert.DeserializeObject<TVResults.Results>(KPresponse1.Content);

                //проверка успешности десериализации
                if (deserialized != null && deserialized.pagesCount > 0)
                {
                    //выбор сериала из результатов ответа на запрос, такого, что это сериал/мини-сериал и что его еще нет в результирующем словаре
                    int id = 0;
                    foreach (var f in deserialized.films)
                        if (f.nameRu.EndsWith("(сериал)") || f.nameRu.EndsWith("(мини-сериал)"))
                        {
                            id = f.filmId;
                            break;
                        }
                    if (id != 0 && !res.ContainsKey(id))
                    {
                        //запрос выбранного сериала по его ID
                        var KPclient2 = new RestSharp.RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{id}");
                        var KPrequest2 = new RestRequest(Method.GET);
                        KPrequest2.AddHeader("X-API-KEY", Bot._kp_key);
                        KPrequest2.AddHeader("accept", "application/json");
                        KPrequest2.AddQueryParameter("append_to_response", "RATING");
                        var KPresponse2 = KPclient2.Execute(KPrequest2);
                        var film = JsonConvert.DeserializeObject<TV.TVObject>(KPresponse2.Content);
                        if (film != null)
                        {
                            film.Priority = 1;
                            film.data.VKPhotoID = Attachments.PopularTVPosterID(film);
                            //проверка валидности загруженной фотографии
                            if (film.data.VKPhotoID != null)
                                res[id] = film;
                        }
                    }
                }
            }
            PopularTV = res;
        }
        #endregion

        //Класс, необходимый для десериализации ответа с Кинопоиска при запросе сериала по его ID
        #region TVObject
        public class Country
        {
            public string country { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class Episode
        {
            public int seasonNumber { get; set; }
            public int episodeNumber { get; set; }
            public object nameRu { get; set; }
            public string nameEn { get; set; }
            public object synopsis { get; set; }
            public string releaseDate { get; set; }
        }

        public class Season
        {
            public int number { get; set; }
            public List<Episode> episodes { get; set; }
        }

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
            public object slogan { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public string ratingMpaa { get; set; }
            public string ratingAgeLimits { get; set; }
            public string premiereRu { get; set; }
            public string distributors { get; set; }
            public string premiereWorld { get; set; }
            public string premiereDigital { get; set; }
            public string premiereWorldCountry { get; set; }
            public object premiereDvd { get; set; }
            public object premiereBluRay { get; set; }
            public object distributorRelease { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public List<string> facts { get; set; }
            public List<Season> seasons { get; set; }
            public string VKPhotoID { get; set; }
        }

        public class ExternalId
        {
            public string imdbId { get; set; }
        }

        public class Budget
        {
            public object grossRu { get; set; }
            public object grossUsa { get; set; }
            public object grossWorld { get; set; }
            public object budget { get; set; }
            public object marketing { get; set; }
        }
        public class Rating
        {
            public double? rating { get; set; }
            public string ratingVoteCount { get; set; }
            public string ratingImdb { get; set; }
            public string ratingImdbVoteCount { get; set; }
            public string ratingFilmCritics { get; set; }
            public string ratingFilmCriticsVoteCount { get; set; }
            public string ratingAwait { get; set; }
            public string ratingAwaitCount { get; set; }
            public object ratingRfCritics { get; set; }
            public object ratingRfCriticsVoteCount { get; set; }
        }

        public class TVObject
        {
            public Data data { get; set; }
            public ExternalId externalId { get; set; }
            public Budget budget { get; set; }
            public Rating rating { get; set; }
            public int Priority { get; set; }
            public TVObject(string nameRu, string nameEn, int filmID)
            {
                data = new Data
                {
                    nameRu = nameRu,
                    nameEn = nameEn,
                    filmId = filmID
                };
            }
        }
        #endregion

        public static class Methods
        {
            public static string FullInfo(int TVID)
            {
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/" + TVID.ToString());
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("append_to_response", "RATING");
                IRestResponse response = client.Execute(request);

                var film = JsonConvert.DeserializeObject<TVObject>(response.Content);
                attachments = new List<MediaAttachment> { Attachments.PosterObject(film.data.posterUrl, film.data.filmId.ToString()) };
                keyboard = Keyboards.TVSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereRu);
                return FullInfo(film);
            }
            public static string FullInfo(TV.TVObject tv)
            {
                TV.Data Data = tv.data;
                string res = $"📺 {Data.nameRu ?? Data.nameEn} ({Data.year})";
                if (tv.rating.rating.HasValue)
                {
                    if (tv.rating.rating.Value != 0)
                        res += $"\n⭐ {tv.rating.rating.Value}";
                    else if (tv.rating.ratingAwait != null)
                        res += $"\n🏁 {tv.rating.ratingAwait}";
                }
                res += "\n";
                if (Data.seasons != null)
                    res += $"\n📈 Сезонов: {Data.seasons.Count}";
                if (Data.filmLength != null)
                    res += $"\n⏰ Длина серии: {Data.filmLength}";
                res += "\n";
                if (Data.countries != null)
                    res += $"\n🌎 Страна: {string.Join(", ", Data.countries.Select(x => x.country))}";
                if (Data.genres != null)
                    res += $"\n🎭 Жанр: {string.Join(", ", Data.genres.Select(x => x.genre))}";
                if (Data.ratingAgeLimits != null)
                    res += $"\n⚠ Возраст: {Data.ratingAgeLimits}+";
                else if (Data.ratingMpaa != null)
                    res += $"\n⚠ Возраст: {Data.ratingMpaa}";
                if (Data.premiereRu != null)
                    res += $"\n📅 Дата премьеры: {Film.Methods.ChangeDateType(Data.premiereRu)}";
                else if (Data.premiereWorld != null)
                    res += $"\n📅 Дата премьеры: {Film.Methods.ChangeDateType(Data.premiereWorld)}";
                res += "\n";
                if (Data.description != null)
                    res += $"\n🎬 Описание:\n{Data.description}";

                return res;
            }

            public static MessageTemplate Search(string TVName)
            {
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("keyword", TVName);
                IRestResponse response = client.Execute(request);
                TVResults.Results results = JsonConvert.DeserializeObject<TVResults.Results>(response.Content);
                if (results == null || results.pagesCount == 0)
                    return null;
                else
                    return Keyboards.TVResults(results);
            }
            //----not online---------------------------------------------------------
            public static void Search_inMessage(string TVName)
            {
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("keyword", TVName);
                IRestResponse response = client.Execute(request);
                TVResults.Results results = JsonConvert.DeserializeObject<TVResults.Results>(response.Content);
                if (results == null || results.pagesCount == 0)
                    SendMessage("К сожалению, я не смог найти такой сериал... 😔");
                else
                     Keyboards.TVResultsMessage(results);
            }
            public static MessageTemplate Random()
            {
                Random random = new Random();
                int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 5);
                //int filmYearUpperLine = random.Next(filmYearBottomLine + 5, DateTime.Now.Year+1);
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                //int filmRatingBottomLine = random.Next(4, 8);

                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "TV_SHOW");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                request.AddQueryParameter("genre", Film.PopularGenres[random.Next(0, Film.PopularGenres.Length)].ToString());
                request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                //request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                //request.AddQueryParameter("ratingFrom", filmRatingBottomLine.ToString());
                IRestResponse response = client.Execute(request);

                var results = JsonConvert.DeserializeObject<RandomTV.Results>(response.Content);
                return Keyboards.RandomTVResults(results);
            }
            //------- not mobile -------
            public static void Random_inMessage()
            {
                Random random = new Random();
                int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 5);
                //int filmYearUpperLine = random.Next(filmYearBottomLine + 5, DateTime.Now.Year+1);
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                //int filmRatingBottomLine = random.Next(4, 8);

                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "TV_SHOW");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                request.AddQueryParameter("genre", Film.PopularGenres[random.Next(0, Film.PopularGenres.Length)].ToString());
                request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                //request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                //request.AddQueryParameter("ratingFrom", filmRatingBottomLine.ToString());
                IRestResponse response = client.Execute(request);

                var results = JsonConvert.DeserializeObject<RandomTV.Results>(response.Content);
                Keyboards.RandomTVResultsMessage(results);
            }
            public static bool Soundtrack(string TVName, List<Audio> audios, int count = 6)
            {
                Yandex.Music.Api.Models.YandexAlbum album = null;
                try
                {
                    album = Bot.yandex_api.GetAlbum(Bot.yandex_api.SearchAlbums($"{TVName} сериал").First(album => album.TrackCount >= 4).Id);
                }
                catch (Exception)
                {
                    return false;
                }
                var tracks = album.Volumes[0].Take(Math.Min(count, album.TrackCount.Value));
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

            public static MessageKeyboard ServiceLinks(string TVName, string date)
            {
                var client = new RestSharp.RestClient("https://www.googleapis.com/customsearch/v1");
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("key", _google_key);
                request.AddQueryParameter("cx", _google_sid_series);
                request.AddQueryParameter("q", $"{TVName} {date.Substring(0, 4)} сериал смотреть");
                request.AddQueryParameter("num", "10");
                IRestResponse response = client.Execute(request);
                ServiceClass.service_data.IncGoogleRequests();
                var results = JsonConvert.DeserializeObject<GoogleResponse>(response.Content);
                var dict = new Dictionary<string, string>();
                foreach (var item in results.items)
                {
                    if (Regex.IsMatch(item.link, @"https://megogo.net/ru/view/.+") && !dict.ContainsKey("MEGOGO"))
                        dict["MEGOGO"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.tvigle.ru/video/.+") && !dict.ContainsKey("TVIGLE"))
                        dict["TVIGLE"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://wink.rt.ru/media_items/.+") && !dict.ContainsKey("WINK"))
                        dict["WINK"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://okko.tv/serial/.+") && !dict.ContainsKey("OKKO"))
                        dict["OKKO"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://hd.kinopoisk.ru/film/.+") && !dict.ContainsKey("КИНОПОИСК"))
                        dict["КИНОПОИСК"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.kinopoisk.ru/series/.+") && !dict.ContainsKey("КИНОПОИСК") && item.title.Contains("смотреть онлайн"))
                        dict["КИНОПОИСК"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.netflix.com/ru/title/.+") && !dict.ContainsKey("NETFLIX"))
                        dict["NETFLIX"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.amediateka.ru/watch/series.+") && !dict.ContainsKey("AMEDIATEKA"))
                        dict["AMEDIATEKA"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://more.tv/.+") && !dict.ContainsKey("MORE"))
                        dict["MORE"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.tvzavr.ru/film/.+") && !dict.ContainsKey("TVZAVR"))
                        dict["TVZAVR"] = item.link;
                    if (dict.Count == 2)
                        break;
                }
                return dict.Count == 0 ? null : Keyboards.ServiceLinks(dict);
            }
        }
    }

    //Класс, необходимый для десериализации результатов запроса сериала по названию
    #region TVResults
    public static class TVResults
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

        public class Results
        {
            public string keyword { get; set; }
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
            public int searchFilmsCountResult { get; set; }
        }
    }
    #endregion

    //Класс, необходимый для десериализации результатов запроса случайных сериалов
    #region RandomTV
    public static class RandomTV
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

    //Класс, необходимый для десериализации ответа с MDB при поиске популярных сериалов или сериалов по названию
    #region MDBResults
    public class MDBTV
    {
        public string backdrop_path { get; set; }
        public string first_air_date { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<string> origin_country { get; set; }
        public string original_language { get; set; }
        public string original_name { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class MDBResultsTV
    {
        public int page { get; set; }
        public List<MDBTV> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
    #endregion

    //Класс, необходимый для десериализации ответа с MDB при поиске рекомендуемых сериалов
    #region MDBRecommendations
    public class Logo
    {
        public string path { get; set; }
        public double aspect_ratio { get; set; }
    }

    public class Network
    {
        public int id { get; set; }
        public Logo logo { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }

    public class MDBRecom
    {
        public string backdrop_path { get; set; }
        public string first_air_date { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<string> origin_country { get; set; }
        public string original_language { get; set; }
        public string original_name { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public List<Network> networks { get; set; }
        public double popularity { get; set; }
    }

    public class MDBRecommendations
    {
        public int page { get; set; }
        public List<MDBRecom> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
    #endregion
}