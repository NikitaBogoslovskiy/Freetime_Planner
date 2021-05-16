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
using static System.Console;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;

namespace Freetime_Planner
{



    public static class Film
    {
        public static int[] PopularGenres = new int[] { 1, 3, 6, 7, 10, 13, 16, 17, 19, 22, 24, 27, 28, 29, 31 };
        public static Dictionary<string, int> GenresConverts = new Dictionary<string, int>
        {
            ["Фантастика"] = 2,
            ["Детектив"] = 17,
            ["Боевик"] = 3,
            ["Комедия"] = 6,
            ["Аниме"] = 1750,
            ["Фэнтези"] = 5,
            ["Драма"] = 8,
            ["Военный"] = 19,
            ["Триллер"] = 4,
            ["Криминал"] = 16,
            ["Семейный"] = 11,
            ["Ужасы"] = 1
        };

        //Популярные фильмы
        #region PopularFilms
        public static Dictionary<int, FilmObject> PopularFilms { get; set; }
        public static string PopularFilmsPath;
        public static DateTime LastPopularFilmsUpdate { get; set; }
        public static Queue<Mailing.MailObject> PopularFilmsQueue { get; set; }
        public static string PopularFilmsQueuePath;
        public static Dictionary<string, List<RandomFilms.Film>> GenreFilms = new Dictionary<string, List<RandomFilms.Film>>();
        public static string GenreFilmsPath;
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
                catch (Exception)
                {
                    UpdatePopularFilms();
                    LastPopularFilmsUpdate = DateTime.Now;
                    UnloadPopularFilms();
                }
            }
        }
        public static void UploadPopularFilmsQueue()
        {
            if (!File.Exists(PopularFilmsQueuePath))
            {
                UpdatePopularFilmsQueue();
                UnloadPopularFilmsQueue();
                return;
            }
            else
            {
                try
                {
                    PopularFilmsQueue = JsonConvert.DeserializeObject<Queue<Mailing.MailObject>>(File.ReadAllText(PopularFilmsQueuePath));
                }
                catch (Exception)
                {
                    UpdatePopularFilmsQueue();
                    UnloadPopularFilmsQueue();
                }
            }
        }

        public static void UploadGenreFilms()
        {
            if (!File.Exists(GenreFilmsPath))
            {
                UpdateGenreFilms();
                UnloadGenreFilms();
                return;
            }
            else
            {
                try
                {
                    GenreFilms = JsonConvert.DeserializeObject<Dictionary<string, List<RandomFilms.Film>>>(File.ReadAllText(GenreFilmsPath));
                }
                catch (Exception)
                {
                    UpdateGenreFilms();
                    UnloadGenreFilms();
                }
            }
        }
        /// <summary>
        /// Выгружает список популярных фильмов из PopularFilms в json-файл
        /// </summary>
        public static void UnloadPopularFilms() => File.WriteAllText(PopularFilmsPath, JsonConvert.SerializeObject(new KeyValuePair<DateTime, Dictionary<int, FilmObject>>(LastPopularFilmsUpdate, PopularFilms)));

        public static void UnloadPopularFilmsQueue() => File.WriteAllText(PopularFilmsQueuePath, JsonConvert.SerializeObject(PopularFilmsQueue));

        public static void UnloadGenreFilms() => File.WriteAllText(GenreFilmsPath, JsonConvert.SerializeObject(GenreFilms));
        /// <summary>
        /// Обновляет список популярных фильмов
        /// </summary>
        public static void UpdatePopularFilms()
        {
            var res = new Dictionary<int, FilmObject>();

            //добавление первой страницы фильмов
            var clientA = new RestSharp.RestClient("https://api.tmdb.org/3/movie/popular");
            var requestA = new RestRequest(Method.GET);
            requestA.AddQueryParameter("api_key", Bot._mdb_key);
            requestA.AddQueryParameter("page", "1");
            IRestResponse responseA = clientA.Execute(requestA);
            MDBResults deserializedA;
            try { deserializedA = JsonConvert.DeserializeObject<MDBResults>(responseA.Content); }
            catch (Exception) { return; }
            if (deserializedA == null || deserializedA.total_pages == 0)
                return;
            var list = deserializedA.results;

            //добавление второй страницы фильмов
            var clientB = new RestSharp.RestClient("https://api.tmdb.org/3/movie/popular");
            var requestB = new RestRequest(Method.GET);
            requestB.AddQueryParameter("api_key", Bot._mdb_key);
            requestB.AddQueryParameter("page", "2");
            IRestResponse responseB = clientB.Execute(requestB);
            MDBResults deserializedB;
            try { deserializedB = JsonConvert.DeserializeObject<MDBResults>(responseB.Content); }
            catch (Exception) { deserializedB = null; }
            if (deserializedB != null && deserializedB.total_pages != 0)
                //объединение двух списков-страниц в один список
                list.AddRange(deserializedB.results);

            //параллельный обход списка
            foreach (var result in list)
            {
                //запрос фильма по его названию
                var KPclient1 = new RestSharp.RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var KPrequest1 = new RestRequest(Method.GET);
                KPrequest1.AddHeader("X-API-KEY", Bot._kp_key);
                KPrequest1.AddHeader("accept", "application/json");
                KPrequest1.AddQueryParameter("keyword", result.original_title);
                var KPresponse1 = KPclient1.Execute(KPrequest1);
                FilmResults.Results deserialized;
                try { deserialized = JsonConvert.DeserializeObject<FilmResults.Results>(KPresponse1.Content); }
                catch (Exception) { deserialized = null; }

                //проверка успешности десериализации
                if (deserialized != null && deserialized.pagesCount > 0)
                {
                    //выбор фильма из результатов ответа на запрос, такого, что это не сериал и что его еще нет в списке
                    int id = 0;
                    foreach (var f in deserialized.films)
                        if (!f.nameRu.EndsWith("(сериал)") && !f.nameRu.EndsWith("(мини-сериал)"))
                        {
                            id = f.filmId;
                            break;
                        }
                    if (id != 0 && !res.ContainsKey(id))
                    {
                        //запрос выбранного фильма по его ID
                        var KPclient2 = new RestSharp.RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{id}");
                        var KPrequest2 = new RestRequest(Method.GET);
                        KPrequest2.AddHeader("X-API-KEY", Bot._kp_key);
                        KPrequest2.AddHeader("accept", "application/json");
                        KPrequest2.AddQueryParameter("append_to_response", "BUDGET");
                        KPrequest2.AddQueryParameter("append_to_response", "RATING");
                        var KPresponse2 = KPclient2.Execute(KPrequest2);
                        FilmObject film;
                        try { film = JsonConvert.DeserializeObject<Film.FilmObject>(KPresponse2.Content); }
                        catch (Exception) { film = null; }
                        if (film != null)
                        {
                            film.Priority = 1;
                            string photoID2;

                            //Video trailer = null;

                            film.data.VKPhotoID = Attachments.PopularFilmPosterID(film, out photoID2);
                            film.data.VKPhotoID_2 = photoID2;

                            //проверка валидности загруженной фотографии
                            if (film.data.VKPhotoID != null && film.data.VKPhotoID_2 != null)
                            {
                                //FilmObject.GetTrailer(film.data.filmId, ref trailer);
                                //film.TrailerInfo = trailer;

                                res[id] = film;
                            }
                        }
                    }
                }
            }
            PopularFilms = res;

        }
        public static void UpdatePopularFilmsQueue()
        {
            var q = new Queue<Mailing.MailObject>();
            foreach (var film in PopularFilms.Values)
            {
                var m = new Mailing.MailObject();
                string date = film.data.premiereRu ?? film.data.premiereWorld;
                if (DateTime.Now.CompareTo(User.StringToDate(date)) <= 0)
                {
                    m.createTrailer(film.data.filmId.ToString(), film.data.nameRu, film.data.nameEn, film.data.year);
                    q.Enqueue(m);
                }
                else
                {
                    m.createPostersFacts(film);
                    q.Enqueue(m);
                }
            }
            PopularFilmsQueue = q;
        }

        public static void UpdateGenreFilms()
        {
            var dict = new Dictionary<string, List<RandomFilms.Film>>();
            foreach (var pair in GenresConverts)
            {
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                var l = new List<RandomFilms.Film>();
                Random random = new Random();
                while (true)
                {
                    var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("X-API-KEY", Bot._kp_key);
                    request.AddQueryParameter("type", "FILM");
                    request.AddQueryParameter("order", order[random.Next(0, 2)]);
                    request.AddQueryParameter("genre", pair.Value.ToString());
                    int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 15);
                    int filmYearUpperLine = random.Next(filmYearBottomLine + 15, DateTime.Now.Year + 1);
                    request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                    request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                    IRestResponse response = client.Execute(request);
                    RandomFilms.Results results;
                    try { results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content); }
                    catch (Exception) { results = null; }
                    if (results == null || results.films.Count == 0)
                        continue;
                    for (int i = 0; i < Math.Min(results.films.Count, 5); ++i)
                    {
                        var t = results.films[i];
                        string photoID2;
                        t.VKPhotoID = Attachments.RandomFilmPosterID(t, out photoID2);
                        t.VKPhotoID_2 = photoID2;
                        if (t.VKPhotoID == null || t.VKPhotoID_2 == null)
                            continue;
                        l.Add(t);
                    }
                    dict[pair.Key] = l;
                    break;
                }
            }
            GenreFilms = dict;
        }
        #endregion


        #region RandomFilms

        public static Dictionary<int, RandomFilms.Film> RandomFilms { get; set; }
        public static string RandomFilmsPath;
        public static DateTime LastRandomFilmsUpdate { get; set; }

        /// <summary>
        /// Загружает список популярных фильмов из json-файла в поле PopularFilms. Если такого json-файла нет, то создает и выгружает список
        /// </summary>
        public static void UploadRandomFilms()
        {
            if (!File.Exists(RandomFilmsPath))
            {
                UpdateRandomFilms();
                LastRandomFilmsUpdate = DateTime.Now;
                UnloadRandomFilms();
            }
            else
            {
                try
                {
                    var pair = JsonConvert.DeserializeObject<KeyValuePair<DateTime, Dictionary<int, RandomFilms.Film>>>(File.ReadAllText(RandomFilmsPath));
                    LastRandomFilmsUpdate = pair.Key;
                    RandomFilms = pair.Value;
                }
                catch (Exception)
                {
                    UpdateRandomFilms();
                    LastRandomFilmsUpdate = DateTime.Now;
                    UnloadRandomFilms();
                }
            }
        }

        /// <summary>
        /// Выгружает список популярных фильмов из PopularFilms в json-файл
        /// </summary>
        public static void UnloadRandomFilms() => File.WriteAllText(RandomFilmsPath, JsonConvert.SerializeObject(new KeyValuePair<DateTime, Dictionary<int, RandomFilms.Film>>(LastRandomFilmsUpdate, RandomFilms)));

        /// <summary>
        /// Обновляет список популярных фильмов
        /// </summary>
        public static void UpdateRandomFilms()
        {
            while (true)
            {
                Random random = new Random();
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "FILM");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                request.AddQueryParameter("genre", PopularGenres[random.Next(0, PopularGenres.Length)].ToString());
                IRestResponse response = client.Execute(request);
                List<RandomFilms.Film> results;
                try { results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content).films; }
                catch (Exception) { continue; }
                if (results == null || results.Count == 0)
                    continue;

                var dict = new Dictionary<int, RandomFilms.Film>();
                for (int i = 0; i < results.Count; ++i)
                {
                    string photoID2;
                    results[i].VKPhotoID = Attachments.RandomFilmPosterID(results[i], out photoID2);
                    results[i].VKPhotoID_2 = photoID2;
                    if (results[i].VKPhotoID == null && results[i].VKPhotoID_2 == null)
                        continue;
                    dict[results[i].filmId] = results[i];
                }
                RandomFilms = dict;
                return;
            }
        }

        #endregion

        //Класс, необходимый для десериализации ответа с Кинопоиска при поиске фильма по его ID
        #region FilmObject

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
            public string VKPhotoID_2 { get; set; }

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
            public string grossRu { get; set; }
            public string grossUsa { get; set; }
            public string grossWorld { get; set; }
            public string budget { get; set; }
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
            public string ratingRfCritics { get; set; }
            public string ratingRfCriticsVoteCount { get; set; }
        }

        /// <summary>
        /// Объект фильма, который создается из ответа сайта Кинопоиск на запрос фильма по его ID
        /// </summary>
        public class FilmObject
        {
            public Data data { get; set; }
            public ExternalId externalId { get; set; }
            public Budget budget { get; set; }
            public Rating rating { get; set; }
            public int Priority { get; set; }
            public bool TwoWeeksNotification { get; set; }
            public bool PremiereNotification { get; set; }
            public NewTrailer Trailer { get; set; }
            public Video TrailerInfo { get; set; }

            public FilmObject(string nameRu, string nameEn, string date, int filmID)
            {
                data = new Data
                {
                    nameRu = nameRu,
                    nameEn = nameEn,
                    premiereRu = date,
                    filmId = filmID
                };
                TwoWeeksNotification = false;
                PremiereNotification = false;
            }

            public async void CreateTrailerAsync()
            {
                await Task.Run(() => CreateTrailer());
            }
            private void CreateTrailer()
            {
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{data.filmId}/videos");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response = client.Execute(request);
                IEnumerable<Trailer> trailers;
                try { trailers = JsonConvert.DeserializeObject<MovieVideos>(response.Content).trailers.Where(t => t.site.ToLower() == "youtube"); }
                catch (Exception)
                {
                    Trailer = new NewTrailer(new HashSet<string>());
                    Trailer.IsNew = false;
                    return;
                }
                if (trailers.Count() == 0)
                    Trailer = new NewTrailer(new HashSet<string>());
                else
                    Trailer = new NewTrailer(trailers.Select(t => t.url).ToHashSet());
                Trailer.IsNew = false;
            }
            public void UpdateTrailer()
            {
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{data.filmId}/videos");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response = client.Execute(request);
                IEnumerable<Trailer> trailers;
                try { trailers = JsonConvert.DeserializeObject<MovieVideos>(response.Content).trailers.Where(t => t.site.ToLower() == "youtube"); }
                catch (Exception) { return; }
                var difference = trailers.Select(t => t.url).ToHashSet();
                difference.SymmetricExceptWith(Trailer.Links);
                if (difference.Count == 0)
                    return;
                var wc = new WebClient();
                Trailer.Trailer = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
                {
                    Link = difference.First()
                });
                wc.DownloadString(Trailer.Trailer.UploadUrl);
                Trailer.IsNew = true;
                Trailer.Links.UnionWith(difference);
            }

            public static void GetTrailer(int filmID, ref Video trailer)
            {
                var wc = new WebClient();
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmID}/videos");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response = client.Execute(request);
                List<Trailer> trailers;
                try { trailers = JsonConvert.DeserializeObject<MovieVideos>(response.Content).trailers.Where(t => t.site.ToLower() == "youtube").ToList(); }
                catch (Exception)
                {
                    trailer = null;
                    return;
                }
                if ((trailers == null) || (trailers.Count() == 0))
                {
                    trailer = null;
                    return;
                }

                Random random = new Random();
                int rnd = random.Next(0, trailers.Count);
                trailer = Bot.private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
                {
                    Link = trailers[rnd].url
                });

                wc.DownloadString(trailer.UploadUrl);
                Console.WriteLine(trailers[rnd].url);
            }
        }

        public class NewTrailer
        {
            public Video Trailer;
            public bool IsNew;
            public HashSet<string> Links;
            public NewTrailer(HashSet<string> links)
            {
                Links = links;
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


            /*public static MessageTemplate ActorMessage(string filmID)
            {
                var proverka = Film.Methods.Actors(filmID);
                if (proverka != null)
                    return Keyboards.ActorResults(proverka.Take(Math.Min(5,proverka.Count)));
                else return null;
            }*/


            public static string ActorDescriptionMessage(User user, string personId, out IEnumerable<MediaAttachment> attachments)
            {
                var ActInf = Film.Methods.ActorInfo(personId);
                attachments = new List<MediaAttachment> { Attachments.PosterObject(user, ActInf.posterUrl, personId) };
                string ActorInfoObj = "";
                if (ActInf.hasAwards == 1)
                    ActorInfoObj = "🏆 ";
                if (ActInf.nameRu == null || ActInf.nameRu == string.Empty)
                    ActorInfoObj += ActInf.nameEn + "\n";
                else if (ActInf.nameEn == null || ActInf.nameEn == string.Empty)
                    ActorInfoObj += ActInf.nameRu + "\n";

                else if (ActInf.nameRu != null && ActInf.nameEn != null && ActInf.nameRu != string.Empty && ActInf.nameEn != string.Empty)
                    ActorInfoObj += ActInf.nameRu + "/" + ActInf.nameEn + "\n\n";

                if (ActInf.growth != null && ActInf.growth != "0")
                    ActorInfoObj += "🕺Рост: " + ActInf.growth + "\n";
                if (ActInf.birthday != null && ActInf.birthday != string.Empty)
                    ActorInfoObj += "👶День рождения: " + Film.Methods.ChangeDateType(ActInf.birthday) + "\n";
                if (ActInf.death != null && ActInf.death != string.Empty)
                    ActorInfoObj += "💀Дата смерти: " + Film.Methods.ChangeDateType(ActInf.death) + "\n";
                if (ActInf.age != 0)
                    ActorInfoObj += "⏰Возраст: " + ActInf.age + "\n\n";
                if (ActInf.facts != null && ActInf.facts.Count != 0)
                    ActorInfoObj += "✨Интересные факты\n" + string.Join("\n", ActInf.facts.Take(Math.Min(3, ActInf.facts.Count)).Select(f => $"✔ {f}")) + "\n";
                ActorInfoObj += "\n📽Фильмография\n";
                int i = 0;
                var l = new List<int>();

                foreach (var item in ActInf.films)
                {
                    if (i == 10)
                        break;

                    if (l.Contains(item.filmId))
                        continue;

                    if (item.nameRu == null)
                        ActorInfoObj += item.nameEn;
                    else
                        ActorInfoObj += item.nameRu;


                    if (item.rating != null)
                    {
                        var m = Regex.Match(item.rating, @"\b(\d)\.(\d)\b");
                        if (m.Success && m.Groups[1].Value != "0" && m.Groups[2].Value != "0")
                            ActorInfoObj += " ⭐" + item.rating;
                    }
                    ActorInfoObj += "\n";
                    i++;
                    l.Add(item.filmId);
                }


                return ActorInfoObj;
            }
            /// <summary>
            /// Возвращает подробную информацию по ID фильма (отправляет запрос на Кинопоиск)
            /// </summary>
            /// <param name="filmID"></param>
            /// <returns></returns>
            public static string FullInfo(User user, int filmID, out MessageKeyboard keyboard, out List<MediaAttachment> attachments)
            {
                Video trailer = null;
                var t = new Task(() => FilmObject.GetTrailer(filmID, ref trailer));
                t.Start();

                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/" + filmID.ToString());
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("append_to_response", "BUDGET");
                request.AddQueryParameter("append_to_response", "RATING");
                IRestResponse response = client.Execute(request);

                FilmObject film;
                try { film = JsonConvert.DeserializeObject<FilmObject>(response.Content); }
                catch (Exception) { keyboard = null; attachments = null; return "При загрузке информации о фильме что-то произошло... 😔 Попробуй повторно выполнить запрос"; }

               
                var poster = Attachments.PosterObject(user, film.data.posterUrl, film.data.filmId.ToString());

                t.Wait();
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                Console.Write("Duration: ");
                Console.WriteLine(trailer.Duration);
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                attachments = new List<MediaAttachment> { poster };
                if (trailer != null)
                    attachments.Add(trailer);

                if (film.data.nameEn != null && film.data.nameEn != string.Empty)
                    user.AddFilmSoundtrackAsync(film.data.nameEn, "ost");
                else
                    user.AddFilmSoundtrackAsync(film.data.nameRu, "саундтрек");
                keyboard = Keyboards.FilmSearch(film.data.nameRu, film.data.nameEn, film.data.filmId.ToString(), film.data.premiereRu ?? film.data.premiereWorld ?? film.data.year, string.Join("*", film.data.genres.Select(g => g.genre)), film.data.premiereDigital ?? film.data.premiereDvd);
                return FullInfo(film);
            }

            /// <summary>
            /// Возвращает подробную информацию по фильму из его объекта (просто переводит в текстовый вид)
            /// </summary>
            /// <param name="film"></param>
            /// <returns></returns>
            public static string FullInfo(Film.FilmObject film)
            {
                Data filmData = film.data;
                string res = $"📽 {filmData.nameRu ?? filmData.nameEn} ({filmData.year})";
                if (film.rating.rating.HasValue)
                {
                    if (film.rating.rating.Value != 0)
                        res += $"\n⭐ {film.rating.rating.Value}";
                    else if (film.rating.ratingAwait != null)
                        res += $"\n🏁 {film.rating.ratingAwait}";
                }
                if (filmData.filmLength != null)
                    res += $"\n⏰ {filmData.filmLength}";
                res += "\n";
                if (filmData.countries != null)
                    res += $"\n🌎 Страна: {string.Join(", ", filmData.countries.Select(x => x.country))}";
                if (filmData.genres != null)
                    res += $"\n🎭 Жанр: {string.Join(", ", filmData.genres.Select(x => x.genre))}";
                res += "\n";
                if (filmData.ratingAgeLimits != null)
                    res += $"\n⚠ Возраст: {filmData.ratingAgeLimits}+";
                else if (filmData.ratingMpaa != null)
                    res += $"\n⚠ Возраст: {filmData.ratingMpaa}";
                if (film.budget?.budget != null)
                    res += $"\n💰 Бюджет: {film.budget.budget}";
                if (filmData.premiereRu != null)
                    res += $"\n📅 Дата премьеры: {ChangeDateType(filmData.premiereRu)}";
                else if (filmData.premiereWorld != null)
                    res += $"\n📅 Дата премьеры: {ChangeDateType(filmData.premiereWorld)}";
                res += "\n";
                if (filmData.description != null)
                    res += $"\n🎬 Описание:\n{filmData.description}";

                return res;
            }

            public static string ChangeDateType(string eng_date)
            {
                try
                {
                    GroupCollection gr = Regex.Match(eng_date, @"(\d{4})-(\d{2})-(\d{2})").Groups;
                    return $"{gr[3]}.{gr[2]}.{gr[1]}";
                }
                catch (Exception) { return ""; }
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
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("keyword", filmName);
                IRestResponse response = client.Execute(request);
                FilmResults.Results results;
                try { results = JsonConvert.DeserializeObject<FilmResults.Results>(response.Content); }
                catch (Exception) { results = null; }
                if (results == null || results.pagesCount == 0)
                    return null;
                else
                    return Keyboards.FilmResults(results);
            }
            //not mobile
            public static void Search_inMessage(User user, string filmName)
            {
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("keyword", filmName);
                IRestResponse response = client.Execute(request);
                FilmResults.Results results;
                try { results = JsonConvert.DeserializeObject<FilmResults.Results>(response.Content); }
                catch (Exception) { results = null; }
                if (results == null || results.pagesCount == 0)
                    Bot.SendMessage(user, "К сожалению, я не смог найти такой фильм... 😔");
                else
                    Keyboards.FilmResultsMessage(user, results);
            }



            /// <summary>
            /// Возвращает карусель из фильмов, которые были получены в результате случайного поиска фильма (используется класс FilmResults)
            /// </summary>
            /// <returns></returns>
            /* public static MessageTemplate Random()
             {
                 Random random = new Random();
                 //int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 5);
                 //int filmYearUpperLine = random.Next(filmYearBottomLine + 5, DateTime.Now.Year+1);
                 string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                 //int filmRatingBottomLine = random.Next(4, 8);

                 var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                 var request = new RestRequest(Method.GET);
                 request.AddHeader("X-API-KEY", Bot._kp_key);
                 request.AddQueryParameter("type", "FILM");
                 request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                 request.AddQueryParameter("genre", PopularGenres[random.Next(0, PopularGenres.Length)].ToString());
                 //request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                 //request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                 //request.AddQueryParameter("ratingFrom", filmRatingBottomLine.ToString());
                 IRestResponse response = client.Execute(request);

                 var results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content);
                 return Keyboards.RandomFilmResults(results);
             }*/
            //not mobile
            /*public static void Random_inMessage(User user)
            {
                Random random = new Random();
                //int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 5);
                //int filmYearUpperLine = random.Next(filmYearBottomLine + 5, DateTime.Now.Year+1);
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                //int filmRatingBottomLine = random.Next(4, 8);

                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "FILM");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                request.AddQueryParameter("genre", PopularGenres[random.Next(0, PopularGenres.Length)].ToString());
                //request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                //request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                //request.AddQueryParameter("ratingFrom", filmRatingBottomLine.ToString());
                IRestResponse response = client.Execute(request);

                RandomFilms.Results results;
                try { results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content); }
                catch(Exception) { results = null; }
                

                Keyboards.RandomFilmResultsMessage(user, results);
            }*/
            /// <summary>
            /// Возвращает список аудиозаписей по названию фильма
            /// </summary>
            /// <param name="filmName"></param>
            /// <returns></returns>
            public static bool DownloadSoundtrack(string filmName, string addition, List<Audio> audios, int count)
            {
                string[] song_names;
                try
                {
                    song_names = SpotifyTracks.GetTracks(SpotifyPlaylists.SearchPlaylist($"{filmName} {addition}"), count.ToString()).ToArray();
                    //var tracks = yandex_api.GetAlbum(yandex_api.SearchAlbums($"{filmName} {addition}")[0].Id).Volumes[0];
                    //song_names = tracks.Take(Math.Min(count, tracks.Count)).Select(n => $"{n.Title} {string.Join(' ', n.Artists.Select(a => a.Name))}").ToArray();
                    for (int i = 0; i < song_names.Length; ++i)
                    {
                        var collection = Bot.private_vkapi.Audio.Search(new VkNet.Model.RequestParams.AudioSearchParams
                        {
                            Autocomplete = true,
                            Query = song_names[i]
                        });
                        if (collection.Count > 0)
                        {
                            audios.Add(collection[0]);
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Возвращает список актеров по ID фильма
            /// </summary>
            /// <param name="filmId"></param>
            /// <returns></returns>
            public static List<ActorResults.Actor> Actors(string filmId)
            {
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v1/staff?filmId={ filmId }");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddHeader("accept", "application/json");
                //request.AddQueryParameter("filmId", filmId);

                IRestResponse response = client.Execute(request);
                List<ActorResults.Actor> result = null;
                try { result = JsonConvert.DeserializeObject<List<ActorResults.Actor>>(response.Content).Where(x => x.professionKey == "ACTOR").ToList(); }
                catch (Exception) { result = null; }
                if (result == null || result.Count == 0)
                    return null;

                return result;
            }

            /// <summary>
            /// Возвращает информацию по ID актера
            /// </summary>
            /// <param name="staffId"></param>
            /// <returns></returns>
            public static ActorResults.ActorFullInfo ActorInfo(string staffId)
            {
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v1/staff/{staffId}");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddHeader("accept", "application/json");

                IRestResponse response = client.Execute(request);

                var result = JsonConvert.DeserializeObject<ActorResults.ActorFullInfo>(response.Content);

                return result;
            }

            /// <summary>
            /// Возвращает видеорецепт блюда по жанрам фильма
            /// </summary>
            /// <param name="genres"></param>
            /// <returns></returns>
            public static Video Food(string[] genres, User user)
            {
                Random r = new Random();
                WebClient wc = new WebClient();
                var client = new RestSharp.RestClient("https://www.googleapis.com/youtube/v3/search");
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("key", Bot._youtube_key);
                request.AddQueryParameter("part", "snippet");
                string[] meal;
                if (user.OnlyHealthyFood)
                    meal = Freetime_Planner.Food.GenreHealthyFood[genres[r.Next(0, genres.Length)]];
                else
                    meal = Freetime_Planner.Food.GenreFood[genres[r.Next(0, genres.Length)]];
                int ind;
                do { ind = r.Next(0, meal.Length); } while (meal[ind] == user.LastGenreFood);
                user.LastGenreFood = meal[ind];
                request.AddQueryParameter("q", meal[ind]);
                request.AddQueryParameter("videoDuration", "short");
                request.AddQueryParameter("type", "video");
                IRestResponse response = client.Execute(request);
                YouTube.YouTubeResults results;
                try { results = JsonConvert.DeserializeObject<YouTube.YouTubeResults>(response.Content); }
                catch (Exception) { return null; }

                var video = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
                {
                    Link = $"https://www.youtube.com/watch?v={results.items[r.Next(0, 5)].id.videoId}",
                    Name = "Приятного аппетита!"
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
            public static MessageKeyboard ServiceLinks(string filmName, string date)
            {
                var client = new RestSharp.RestClient("https://www.googleapis.com/customsearch/v1");
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("key", _google_key);
                request.AddQueryParameter("cx", _google_sid);
                request.AddQueryParameter("q", $"{filmName} {date.Substring(0, 4)} смотреть");
                request.AddQueryParameter("num", "10");
                IRestResponse response = client.Execute(request);
                ServiceClass.service_data.IncGoogleRequests();
                GoogleResponse results;
                try { results = JsonConvert.DeserializeObject<GoogleResponse>(response.Content); }
                catch (Exception) { return null; }
                var dict = new Dictionary<string, string>();
                foreach (var item in results.items)
                {
                    if (Regex.IsMatch(item.link, @"https://www.ivi.ru/watch/\d+$") && !dict.ContainsKey("IVI"))
                        dict["IVI"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://megogo.net/ru/view/.+") && !dict.ContainsKey("MEGOGO"))
                        dict["MEGOGO"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://okko.tv/movie/.+") && !dict.ContainsKey("OKKO"))
                        dict["OKKO"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://hd.kinopoisk.ru/film/.+") && !dict.ContainsKey("КИНОПОИСК"))
                        dict["КИНОПОИСК"] = item.link;
                    else if (Regex.IsMatch(item.link, @"https://www.kinopoisk.ru/film/.+") && !dict.ContainsKey("КИНОПОИСК") && item.title.Contains("смотреть онлайн"))
                        dict["КИНОПОИСК"] = item.link;
                    if (dict.Count == 4)
                        break;
                }
                return dict.Count == 0 ? null : Keyboards.ServiceLinks(dict);
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
            public string VKPhotoID { get; set; }
            public string VKPhotoID_2 { get; set; }
        }

        public class Results
        {
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
        }
    }
    #endregion

    //Класс, необходимый для десериализации ответа с Кинопоиска при поиске актера
    #region Actor


    public static class ActorResults
    {


        public class Actor
        {
            public int staffId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string posterUrl { get; set; }
            public string professionText { get; set; }
            public string professionKey { get; set; }
            public string description { get; set; }
        }

        public class ActorFullInfo
        {
            public int personId { get; set; }
            public string webUrl { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string sex { get; set; }
            public string posterUrl { get; set; }
            public string growth { get; set; }
            public string birthday { get; set; }
            public string death { get; set; }
            public int age { get; set; }
            public string birthplace { get; set; }
            public string deathplace { get; set; }
            public int hasAwards { get; set; }
            public string profession { get; set; }
            public List<string> facts { get; set; }
            public List<Spouse> spouses { get; set; }
            public List<Film> films { get; set; }
        }

        public class Spouse
        {
            public int personId { get; set; }
            public string name { get; set; }
            public string divorced { get; set; }
            public string divorcedReason { get; set; }
            public string sex { get; set; }
            public int children { get; set; }
            public string webUrl { get; set; }
            public string relation { get; set; }
        }

        public class Film
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string rating { get; set; }
            public bool general { get; set; }
            public string description { get; set; }
            public string professionKey { get; set; }
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

    #region Spotify

    public class SpotifyTracks
    {
        public class Artist
        {
            public string name { get; set; }
        }

        public class Track
        {
            public List<Artist> artists { get; set; }
            public string name { get; set; }
        }

        public class Item
        {
            public Track track { get; set; }
        }

        public class Tracks
        {
            public List<Item> items { get; set; }
        }

        public static IEnumerable<string> GetTracks(string id, string limit)
        {
            var client = new RestSharp.RestClient($"https://api.spotify.com/v1/playlists/{id}/tracks");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {_spotify_token}");
            request.AddQueryParameter("market", "RU");
            request.AddQueryParameter("fields", "items(track(name,artists(name)))");
            request.AddQueryParameter("limit", limit);
            return JsonConvert.DeserializeObject<Tracks>(client.Execute(request).Content).items.Select(i => $"{i.track.name} {string.Join(" ", i.track.artists.Select(n => n.name))}");
        }
    }

    public class SpotifyPlaylists
    {
        public class ExternalUrls
        {
            public string spotify { get; set; }
        }

        public class Image
        {
            public object height { get; set; }
            public string url { get; set; }
            public object width { get; set; }
        }

        public class Owner
        {
            public string display_name { get; set; }
            public ExternalUrls external_urls { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Tracks
        {
            public string href { get; set; }
            public int total { get; set; }
        }

        public class Item
        {
            public bool collaborative { get; set; }
            public string description { get; set; }
            public ExternalUrls external_urls { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public List<Image> images { get; set; }
            public string name { get; set; }
            public Owner owner { get; set; }
            public object primary_color { get; set; }
            public object @public { get; set; }
            public string snapshot_id { get; set; }
            public Tracks tracks { get; set; }
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Playlists
        {
            public string href { get; set; }
            public List<Item> items { get; set; }
            public int limit { get; set; }
            public string next { get; set; }
            public int offset { get; set; }
            public object previous { get; set; }
            public int total { get; set; }
        }

        public class Root
        {
            public Playlists playlists { get; set; }
        }


        public static string SearchPlaylist(string query)
        {
            var client = new RestSharp.RestClient($"https://api.spotify.com/v1/search");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {Bot._spotify_token}");
            request.AddQueryParameter("q", query);
            request.AddQueryParameter("type", "playlist");
            request.AddQueryParameter("limit", "1");
            return JsonConvert.DeserializeObject<SpotifyPlaylists.Root>(client.Execute(request).Content).playlists.items[0].id;
        }
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
