using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Freetime_Planner.Modes;
using Newtonsoft.Json;
using VkNet.Model.Template;
using System.Threading.Tasks;
using System.Linq;
using RestSharp;
using VkNet.Model.Attachments;

namespace Freetime_Planner
{
    public class User
    {
        /// <summary>
        /// Имя пользовтеля
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; private set; }

        /// <summary>
        /// ID пользователя
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Уровень, на котором находится пользователь
        /// </summary>
        public LinkedList<Mode> Level { get; private set; }

        /// <summary>
        /// Последнее время, когда пользователь находился на каком-то нетривиальном уровне (т.е. не Default)
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// Словарь фильмов-рекомендаций, где ключ - это id фильма на Кинопоиске, а значение - объект класса FilmObject
        /// </summary>
        public Dictionary<int, Film.FilmObject> FilmRecommendations { get; set; }

        public  int[] PopularGenres = new int[] { 1, 3, 6, 7, 10, 13, 16, 17, 19, 22, 24, 27, 28, 29, 31 };
        public Dictionary<int, RandomFilms.Film> FilmRandomDict { get; set; }
        public bool RandomFilmsIsUpdating { get; set; }
        public Dictionary<int, RandomTV.Film> TVRandomDict { get; set; }
        public bool RandomTVIsUpdating { get; set; }

        /// <summary>
        /// Массив, состоящий из двух списков, элементы которых - объекты класса FilmObject. В первом списке вышедшие фильмы, во втором - не вышедвшие
        /// </summary>
        public List<Film.FilmObject>[] PlannedFilms { get; set; }

        public HashSet<int> HiddenFilms { get; set; }

        public Dictionary<int, TV.TVObject> TVRecommendations { get; set; }

        public List<TV.TVObject> PlannedTV { get; set; }

        public HashSet<int> HiddenTV { get; set; }

        public bool MailFunction { get; set; }

        public int DaysGap { get; set; }

        public DateTime NextMail { get; set; }

        public Queue<Mailing.MailObject> MailObjects { get; set; }

        public DateTime LastPlannedFilmsUpdate { get; set; }
        
        public Dictionary<string, FilmSountracks> FilmTracks { get; set; }
        public Dictionary<string, FilmSountracks> TVTracks { get; set; }
        public Dictionary<string, string> LastFood { get; set; }
        public string LastGenreFood { get; set; }
        public bool OnlyHealthyFood { get; set; }

        /// <summary>
        /// Конструктор пользователя
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="id"></param>
        public User(string name, string surname, long id)
        {
            Name = name;
            Surname = surname;
            ID = id;
            Level = new LinkedList<Mode>();
            Level.AddLast(Mode.Default);
            FilmRecommendations = Film.PopularFilms;
            HiddenFilms = new HashSet<int>();
            PlannedFilms = new List<Film.FilmObject>[] { new List<Film.FilmObject>(), new List<Film.FilmObject>() };
            TVRecommendations = TV.PopularTV;
            HiddenTV = new HashSet<int>();
            PlannedTV = new List<TV.TVObject>();
            MailFunction = true;
            DaysGap = 1;
            var next = DateTime.Now.AddDays(DaysGap);
            var r = new Random();
            NextMail = new DateTime(next.Year, next.Month, next.Day, r.Next(12, 21), 0, 0);
            MailObjects = new Queue<Mailing.MailObject>();
            LastPlannedFilmsUpdate = DateTime.Now;
            FilmRandomDict = Film.RandomFilms;
            RandomFilmsIsUpdating = false;
            TVRandomDict = TV.RandomTV;
            RandomTVIsUpdating = false;
            FilmTracks = new Dictionary<string, FilmSountracks>();
            TVTracks = new Dictionary<string, FilmSountracks>();
            LastFood = new Dictionary<string, string>
            {
                ["Cocktail"] = "",
                ["Dessert"] = "",
                ["Snack"] = ""
            };
            OnlyHealthyFood = false;
        }

        /// <summary>
        /// Функция, добавляющая новый уровень
        /// </summary>
        /// <param name="m"></param>
        public void AddLevel(Mode m)
        {
            Level.AddLast(m);
            LastTime = DateTime.Now;
            Users.Unload();
        }

        /// <summary>
        /// Функция, удаляющая последний уровень
        /// </summary>
        public void RemoveLevel()
        {
            if (Level.Last.Value == Mode.Default)
                return;
            Level.RemoveLast();
            if (CurrentLevel() != Mode.Default)
                LastTime = DateTime.Now;
            Users.Unload();
        }

        /// <summary>
        /// Возвращает текущий уровень
        /// </summary>
        /// <returns></returns>
        public Mode CurrentLevel() => Level.Last.Value;

        /// <summary>
        /// Возвращает значение предыдущего уровня
        /// </summary>
        /// <returns></returns>
        public Mode PreviousLevel() => Level.Last.Previous.Value;

        /// <summary>
        /// Устанавливает первый уровень по умолчанию Default
        /// </summary>
        public void ResetLevel()
        {
            Level = new LinkedList<Mode>();
            Level.AddLast(Mode.Default);
            Users.Unload();
            //Bot.keyboard = Keyboards.Mainmenu();
            //Bot.SendMessage("Жми любую кнопку");
        }

        public async void AddMailObjectAsync(string id, bool isTrailer, string ruName = null, string engName = null, string date = null)
        {
            await Task.Run(() => AddMailObject(id, isTrailer, ruName, engName, date));
        }

        public void AddMailObject(string id, bool isTrailer, string ruName = null, string engName = null, string date = null)
        {
            var mail = new Mailing.MailObject();
            if (isTrailer)
                mail.createTrailer(id, ruName, engName, date.Substring(0, 4));
            else
                mail.createPostersFacts(this, id);
            if (mail.IsValid && !MailObjects.Any(o => o.id == id))
                MailObjects.Enqueue(mail);
            Users.Unload();
        }

        //--------------Пользовательские методы для фильмов--------------

        /// <summary>
        /// Возвращает карусель фильмов из списка рекомендаций
        /// </summary>
        /// <returns></returns>
        public MessageTemplate GetFilmRecommendations()
        {
            return Keyboards.FilmMyRecommendations(FilmRecommendations.Shuffle().Take(5).Select(kv => kv.Value));
        }

        public void GetFilmRecommendationsMessage(User user)
        {
            
            Keyboards.FilmMyRecommendationsMessage(user, FilmRecommendations.Shuffle().Take(5).Select(kv => kv.Value));

        }
        /// <summary>
        /// Возвращает карусель из фильмов, которые были получены в результате случайного поиска фильма (используется класс FilmResults)
        /// </summary>
        /// <returns></returns>
        public MessageTemplate RandomFilms()
        { 
            if (!RandomFilmsIsUpdating)
            {
                RandomFilmsIsUpdating = true;
                UpdateFilmRandomAsync();
            }
            if (FilmRandomDict == null || FilmRandomDict.Count == 0)
                FilmRandomDict = Film.RandomFilms;
            return Keyboards.RandomFilmResults(FilmRandomDict.Shuffle().Take(3).Select(kv => kv.Value));
        }
        
     

        /// <summary>
        /// Возвращает список планируемых фильмов в текстовом виде (с разделением на вышедшие фильмы и те, что выйдут)
        /// </summary>
        /// <returns></returns>
        public string GetPlannedFilms()
        {
            string res = "Премьера состоялась:\n";
            for (int i = 0; i < PlannedFilms[0].Count; i++)
                res += $"{i + 1}. {PlannedFilms[0][i].data.nameRu ?? PlannedFilms[0][i].data.nameEn}\n";
            res += "\nПремьера в будущем:\n";
            for (int i = 0; i < PlannedFilms[1].Count; i++)
            {
                string date = "";
                if (PlannedFilms[1][i].data.premiereRu.Length != 4)
                    date = Film.Methods.ChangeDateType(PlannedFilms[1][i].data.premiereRu);
                else
                    date = PlannedFilms[1][i].data.premiereRu;
                res += $"{i + 1 + PlannedFilms[0].Count}. {PlannedFilms[1][i].data.nameRu ?? PlannedFilms[1][i].data.nameEn} ({date})\n";
            }
            return res;
        }

        /// <summary>
        /// Добавляет фильм в список планируемых фильмов и обновляет список рекомендаций
        /// </summary>
        /// <param name="nameRu"></param>
        /// <param name="nameEn"></param>
        /// <param name="Date"></param>
        public bool AddPlannedFilm(string nameRu, string nameEn, string Date, string filmID)
        {
            DateTime premiere;
            if (Date.Length != 4)
                premiere = StringToDate(Date);
            else
                premiere = new DateTime(int.Parse(Date), 1, 1);
            if (DateTime.Now.CompareTo(premiere) < 0)
            {
                if (PlannedFilms[1].Any(f => f.data.filmId.ToString() == filmID))
                    return false;
                else
                {
                    var film = new Film.FilmObject(nameRu, nameEn, Date, int.Parse(filmID));
                    PlannedFilms[1].Add(film);
                    film.CreateTrailerAsync();
                }
            }
            else
            {
                if (PlannedFilms[0].Any(f => f.data.filmId.ToString() == filmID))
                    return false;
                else
                    PlannedFilms[0].Add(new Film.FilmObject(nameRu, nameEn, Date, int.Parse(filmID)));
            }
            Users.Unload();
            UpdateFilmRecommendationsAsync(nameEn);
            return true;
        }

        /// <summary>
        /// Обновляет список планируемых фильмов с учетом сегодняшней даты (замечание: после обновления поля PlannedFilms список пользователей не выгружается)
        /// </summary>
        public void UpdatePlannedFilms()
        {
            var dict = new SortedDictionary<DateTime, Film.FilmObject>();
            foreach (var film in PlannedFilms[1])
                dict[StringToDate(film.data.premiereRu)] = film;
            int count = dict.Count(kv => DateTime.Now.CompareTo(kv.Key) >= 0);
            PlannedFilms[0].AddRange(dict.Take(count).Select(kv => kv.Value));
            PlannedFilms[1] = dict.Skip(count).Select(kv => kv.Value).ToList();
            LastPlannedFilmsUpdate = DateTime.Now;
        }

        public void RemovePlannedFilm(string filmID)
        {
            var ind = PlannedFilms[0].FindIndex(film => film.data.filmId == int.Parse(filmID));
            if (ind!= -1)
            {
                PlannedFilms[0].RemoveAt(ind);
                return;
            }
            ind = PlannedFilms[1].FindIndex(film => film.data.filmId == int.Parse(filmID));
            if (ind != -1)
                PlannedFilms[1].RemoveAt(ind);
            Users.Unload();
        }

        /// <summary>
        /// Обновляет список рекомендации на основании понравившегося фильма
        /// </summary>
        /// <param name="nameEn"></param>
        public void LikeFilm(string nameEn)
        {
            UpdateFilmRecommendationsAsync(nameEn);
        }

        /// <summary>
        /// Добавляет фильм в черный список (HiddenFilms) и удаляет его из рекомендаций, если он там есть
        /// </summary>
        /// <param name="filmID"></param>
        public void HideFilm(int filmID)
        {
            HiddenFilms.Add(filmID);
            FilmRecommendations.Remove(filmID);
            if (FilmRecommendations.Count < 6)
                IncRecommendedFilmsAsync();
            Users.Unload();
        }

        /// <summary>
        /// Возвращает список аудиозаписей по названию фильма
        /// </summary>
        /// <param name="filmName"></param>
        /// <returns></returns>
        public bool FilmSoundtrack(string filmName, string addition, ref List<Audio> audios)
        {
            if (!FilmTracks.ContainsKey(filmName))
                AddFilmSoundtrack(filmName, addition);
            var obj = FilmTracks[filmName];
            while (obj.IsLoading) { }
            if (obj.IsEmpty)
                return false;
            else
            {
                audios = obj.Tracks;
                return true;
            }
        }

        /// <summary>
        /// Добавляет популярные фильмы в список рекомендуемых фильмов асинхронно
        /// </summary>
        private async void IncRecommendedFilmsAsync()
        {
            await Task.Run(() => IncRecommendedFilms());
        }
        /// <summary>
        /// Добавляет популярные фильмы в список рекомендуемых фильмов синхронно
        /// </summary>
        private void IncRecommendedFilms()
        {
            Parallel.ForEach(Film.PopularFilms, (kv) =>
            {
                FilmRecommendations.Add(kv.Key, kv.Value);
            });
            Users.Unload();
        }

        /// <summary>
        /// Асинхронно вызывает функцию UpdateFilmRecommendations
        /// </summary>
        /// <param name="nameEn"></param>
        private async void UpdateFilmRecommendationsAsync(string nameEn)
        {
            await Task.Run(() => UpdateFilmRecommendations(nameEn));
        }

        /// <summary>
        /// На основании названия фильма добавляет в список рекомендаций похожие фильмы
        /// </summary>
        /// <param name="nameEn"></param>
        private void UpdateFilmRecommendations(string nameEn)
        {
            if (nameEn == null || nameEn == string.Empty)
                return;
            //урезание списка рекомендаций, чтобы после добавления новых рекомендаций количество элементов в нем не превышало 50
            int difference = FilmRecommendations.Count - 45;
            var new_array = FilmRecommendations.OrderBy(kv => kv.Value.Priority).Skip(difference).ToDictionary(kv => kv.Key, kv => kv.Value);
            var required_count = new_array.Count + 5;

            //поиск фильма в англоязычной базе данных с целью получения его ID
            var client1 = new RestSharp.RestClient("https://api.tmdb.org/3/search/movie");
            var request1 = new RestRequest(Method.GET);
            request1.AddQueryParameter("api_key", Bot._mdb_key);
            request1.AddQueryParameter("query", nameEn);
            IRestResponse response1 = client1.Execute(request1);
            MDBResults deserialized1;
            try { deserialized1 = JsonConvert.DeserializeObject<MDBResults>(response1.Content); }
            catch(Exception) { return; }
            if (deserialized1 == null || deserialized1.total_pages == 0)
                return;
            string sid = deserialized1.results.First().id.ToString();

            //поиск в англоязычной базе данных рекомендуемых фильмов к данному, используя ID данного фильма
            var client2 = new RestSharp.RestClient($"https://api.tmdb.org/3/movie/{sid}/recommendations");
            var request2 = new RestRequest(Method.GET);
            request2.AddQueryParameter("api_key", Bot._mdb_key);
            IRestResponse response2 = client2.Execute(request2);
            MDBResults deserialized2;
            try { deserialized2 = JsonConvert.DeserializeObject<MDBResults>(response2.Content); }
            catch(Exception) { return; }
            if (deserialized2 == null || deserialized2.total_pages == 0)
                return;
            string[] names = deserialized2.results.Select(film => film.title).ToArray();

            foreach (var name in names)
            {
                //поиск рекомендуемого фильма по его названию на Кинопоиске с целью получения его ID
                var KPclient1 = new RestSharp.RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var KPrequest1 = new RestRequest(Method.GET);
                KPrequest1.AddHeader("X-API-KEY", Bot._kp_key);
                KPrequest1.AddHeader("accept", "application/json");
                KPrequest1.AddQueryParameter("keyword", name);
                IRestResponse KPresponse1 = KPclient1.Execute(KPrequest1);
                FilmResults.Results deserialized;
                try { deserialized = JsonConvert.DeserializeObject<FilmResults.Results>(KPresponse1.Content); }
                catch(Exception) { deserialized = null; }

                //проверка успешности десериализации
                if (deserialized != null)
                    if (deserialized.pagesCount > 0)
                    {
                        int id = 0;
                        //проверка, чтобы найденный фильм не был сериалом, не входил в список рекомендаций и не содержался в HiddenFilms
                        foreach (var f in deserialized.films)
                            if (!f.nameRu.EndsWith("(сериал)") && !f.nameRu.EndsWith("(мини-сериал)"))
                            {
                                id = f.filmId;
                                break;
                            }
                        if (id == 0 || new_array.ContainsKey(id) || HiddenFilms.Contains(id))
                            continue;
                        //поиск фильма по выбранному ID
                        var KPclient2 = new RestSharp.RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{id}");
                        var KPrequest2 = new RestRequest(Method.GET);
                        KPrequest2.AddHeader("X-API-KEY", Bot._kp_key);
                        KPrequest2.AddHeader("accept", "application/json");
                        KPrequest2.AddQueryParameter("append_to_response", "BUDGET");
                        KPrequest2.AddQueryParameter("append_to_response", "RATING");
                        IRestResponse KPresponse2 = KPclient2.Execute(KPrequest2);
                        Film.FilmObject film;
                        try { film = JsonConvert.DeserializeObject<Film.FilmObject>(KPresponse2.Content); }
                        catch(Exception) { film = null; }
                        if (film == null)
                            continue;
                        film.Priority = 2;
                        
                        film.data.VKPhotoID   = Attachments.RecommendedFilmPosterID(film);
                      //  film.data.VKPhotoID_2 = full_photo_ID ;
                        //проверка валидности изображения
                        if (film.data.VKPhotoID != null)
                        {
                            new_array[id] = film;
                            //добавляем только требуемое количество
                            if (new_array.Count >= required_count)
                                break;
                        }
                    }
            }
            FilmRecommendations = new_array;
            Users.Unload();
        }

        private async void UpdateFilmRandomAsync()
        {
            await Task.Run(() => UpdateFilmRandom());
        }

        /// <summary>
        /// На основании названия фильма добавляет в список рекомендаций похожие фильмы
        /// </summary>
        /// <param name="nameEn"></param>
        private void UpdateFilmRandom()
        {
            while (true)
            {
                var dict = new Dictionary<int, RandomFilms.Film>();
                Random random = new Random();

                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "FILM");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                int opt = random.Next(0, 2);
                if (opt == 0)
                    request.AddQueryParameter("genre", PopularGenres[random.Next(0, PopularGenres.Length)].ToString());
                else
                {
                    int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 10);
                    int filmYearUpperLine = random.Next(filmYearBottomLine + 10, DateTime.Now.Year + 1);
                    request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                    request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                }
                IRestResponse response = client.Execute(request);
                RandomFilms.Results results;
                try { results = JsonConvert.DeserializeObject<RandomFilms.Results>(response.Content); }
                catch(Exception) { results = null; }
                if (results == null && results.films.Count == 0)
                    continue;
                for (int i = 0; i < results.films.Count; ++i)
                {
                    var t = results.films[i];
                    t.VKPhotoID = Attachments.RandomFilmPosterID(t);
                    if (t.VKPhotoID == null)
                        continue;
                    dict[t.filmId] = t;
                }
                FilmRandomDict = dict;
                RandomFilmsIsUpdating = false;
                Users.Unload();
                return;
            }
        }

        public async void AddFilmSoundtrackAsync(string name, string addition)
        {
            await Task.Run(() => AddFilmSoundtrack(name, addition));
        }

        public void AddFilmSoundtrack(string name, string addition)
        {
            string[] song_names;
            var res = new FilmSountracks();
            try
            {
                //song_names = SpotifyTracks.GetTracks(SpotifyPlaylists.SearchPlaylist($"{name} {addition}"), "6").ToArray();
                var tracks = Bot.yandex_api.GetAlbum(Bot.yandex_api.SearchAlbums($"{name} {addition}")[0].Id).Volumes[0];
                song_names = tracks.Take(Math.Min(6, tracks.Count)).Select(n => $"{n.Title} {string.Join(' ', n.Artists.Select(a => a.Name))}").ToArray();
                var audios = new List<Audio>();
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
                res.Update(audios);
                FilmTracks[name] = res;
                if (res.Tracks.Count == 0)
                    FilmTracks[name].IsEmpty = true;
                Users.Unload();
            }
            catch (Exception)
            {
                FilmTracks[name] = res;
                FilmTracks[name].IsLoading = false;
                return;
            }
        }

            //--------------Пользовательские методы для сериалов--------------

        public MessageTemplate GetTVRecommendations()
        {
            return Keyboards.TVMyRecommendations(TVRecommendations.Shuffle().Take(5).Select(kv => kv.Value));
        }

        public void GetTVRecommendationsMessage(User user)
        {
            Keyboards.TVMyRecommendationsMessage(user, TVRecommendations.Shuffle().Take(5).Select(kv => kv.Value));
        }

        public MessageTemplate RandomTV()
        { 
            if (!RandomTVIsUpdating)
            {
                RandomTVIsUpdating = true;
                UpdateTVRandomAsync();
            }
            if (TVRandomDict == null || TVRandomDict.Count == 0)
                TVRandomDict = TV.RandomTV;
            //Console.WriteLine(string.Join("\n", TVRandomDict.Values.Select(f => f.nameRu)));
            return Keyboards.RandomTVResults(TVRandomDict.Shuffle().Take(3).Select(kv => kv.Value));
        }

        public string GetPlannedTV()
        {
            string res = "Планируемые к просмотру сериалы:\n";
            for (int i = 0; i < PlannedTV.Count; i++)
                res += $"{i + 1}. {PlannedTV[i].data.nameRu ?? PlannedTV[i].data.nameEn}\n";
            return res;
        }

        public bool AddPlannedTV(string nameRu, string nameEn, string filmID)
        {
            if (PlannedTV.Any(f => f.data.filmId.ToString() == filmID))
                return false;
            else
                PlannedTV.Add(new TV.TVObject(nameRu, nameEn, int.Parse(filmID)));
            UpdateTVRecommendationsAsync(nameEn);
            Users.Unload();
            return true;
        }

        public void RemovePlannedTV(string filmID)
        {
            var ind = PlannedTV.FindIndex(film => film.data.filmId == int.Parse(filmID));
            if (ind != -1)
                PlannedTV.RemoveAt(ind);
        }

        public void LikeTV(string nameEn)
        {
            UpdateTVRecommendationsAsync(nameEn);
            //return;
        }

        public void HideTV(int TVID)
        {
            HiddenTV.Add(TVID);
            TVRecommendations.Remove(TVID);
            if (TVRecommendations.Count < 6)
                IncRecommendedTVAsync();
            Users.Unload();
        }

        /// <summary>
        /// Возвращает список аудиозаписей по названию фильма
        /// </summary>
        /// <param name="filmName"></param>
        /// <returns></returns>
        public bool TVSoundtrack(string filmName, string addition, ref List<Audio> audios)
        {
            if (!TVTracks.ContainsKey(filmName))
                AddTVSoundtrack(filmName, addition);
            var obj = TVTracks[filmName];
            while (obj.IsLoading) { }
            if (obj.IsEmpty)
                return false;
            else
            {
                audios = obj.Tracks;
                return true;
            }
        }
        /*
        public void AlreadyWatchedTV(int index, int TVID)
        {
            //нужно убрать сериал из списка PlannedTV
            HideTV(TVID);
            return;
        }

        public string FullTVInfo(int TVID)
        {
            return null;
        }
        */

        private async void UpdateTVRecommendationsAsync(string nameEn)
        {
            await Task.Run(() => UpdateTVRecommendations(nameEn));
        }

        private void UpdateTVRecommendations(string nameEn)
        {
            if (nameEn == null || nameEn == string.Empty)
                return;
            //урезание списка рекомендаций, чтобы после добавления новых рекомендаций количество элементов в нем не превышало 50
            int difference = TVRecommendations.Count - 45;
            var new_array = TVRecommendations.OrderBy(kv => kv.Value.Priority).Skip(difference).ToDictionary(kv => kv.Key, kv => kv.Value);
            var required_count = new_array.Count + 5;

            //поиск сериала в англоязычной базе данных с целью получения его ID
            var client1 = new RestSharp.RestClient("https://api.tmdb.org/3/search/tv");
            var request1 = new RestRequest(Method.GET);
            request1.AddQueryParameter("api_key", Bot._mdb_key);
            request1.AddQueryParameter("query", nameEn);
            IRestResponse response1 = client1.Execute(request1);
            MDBResultsTV deserialized1;
            try { deserialized1 = JsonConvert.DeserializeObject<MDBResultsTV>(response1.Content); }
            catch(Exception) { return; }
            if (deserialized1 == null || deserialized1.total_pages == 0)
                return;
            string sid = deserialized1.results.First().id.ToString();

            //поиск в англоязычной базе данных рекомендуемых сериалов к данному, используя ID данного сериала
            var client2 = new RestSharp.RestClient($"https://api.tmdb.org/3/tv/{sid}/recommendations");
            var request2 = new RestRequest(Method.GET);
            request2.AddQueryParameter("api_key", Bot._mdb_key);
            IRestResponse response2 = client2.Execute(request2);
            MDBRecommendations deserialized2;
            try { deserialized2 = JsonConvert.DeserializeObject<MDBRecommendations>(response2.Content); }
            catch(Exception) { return; }
            if (deserialized2 == null || deserialized2.total_pages == 0)
                return;
            string[] names = deserialized2.results.Select(film => film.name).ToArray();

            foreach (var name in names)
            {
                //поиск рекомендуемого сериала по его названию на Кинопоиске с целью получения его ID
                var KPclient1 = new RestSharp.RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword");
                var KPrequest1 = new RestRequest(Method.GET);
                KPrequest1.AddHeader("X-API-KEY", Bot._kp_key);
                KPrequest1.AddHeader("accept", "application/json");
                KPrequest1.AddQueryParameter("keyword", name);
                IRestResponse KPresponse1 = KPclient1.Execute(KPrequest1);
                TVResults.Results deserialized;
                try { deserialized = JsonConvert.DeserializeObject<TVResults.Results>(KPresponse1.Content); }
                catch (Exception) { deserialized = null; }

                //проверка успешности десериализации
                if (deserialized != null)
                    if (deserialized.pagesCount > 0)
                    {
                        int id = 0;
                        //проверка, чтобы найденный сериал являлся действительно сериалом, не входил в список рекомендаций и не содержался в HiddenTV
                        foreach (var f in deserialized.films)
                            if (f.nameRu.EndsWith("(сериал)") || f.nameRu.EndsWith("(мини-сериал)"))
                            {
                                id = f.filmId;
                                break;
                            }
                        if (id == 0 || new_array.ContainsKey(id) || HiddenTV.Contains(id))
                            continue;
                        //поиск сериала по выбранному ID
                        var KPclient2 = new RestSharp.RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{id}");
                        var KPrequest2 = new RestRequest(Method.GET);
                        KPrequest2.AddHeader("X-API-KEY", Bot._kp_key);
                        KPrequest2.AddHeader("accept", "application/json");
                        KPrequest2.AddQueryParameter("append_to_response", "RATING");
                        IRestResponse KPresponse2 = KPclient2.Execute(KPrequest2);
                        TV.TVObject tv;
                        try { tv = JsonConvert.DeserializeObject<TV.TVObject>(KPresponse2.Content); }
                        catch(Exception) { tv = null; }
                        if (tv == null)
                            continue;
                        tv.Priority = 2;
                        tv.data.VKPhotoID = Attachments.RecommendedTVPosterID(tv);
                        //проверка валидности изображения
                        if (tv.data.VKPhotoID != null)
                        {
                            new_array[id] = tv;
                            //добавляем только требуемое количество
                            if (new_array.Count >= required_count)
                                break;
                        }
                    }
            }
            TVRecommendations = new_array;
            Users.Unload();
        }

        /// <summary>
        /// Добавляет популярные сериалы в список рекомендуемых сериалов асинхронно
        /// </summary>
        private async void IncRecommendedTVAsync()
        {
            await Task.Run(() => IncRecommendedTV());
        }
        /// <summary>
        /// Добавляет популярные сериалы в список рекомендуемых сериалов синхронно
        /// </summary>
        private void IncRecommendedTV()
        {
            Parallel.ForEach(TV.PopularTV, (kv) =>
            {
                TVRecommendations.Add(kv.Key, kv.Value);
            });
            Users.Unload();
        }

        private async void UpdateTVRandomAsync()
        {
            await Task.Run(() => UpdateTVRandom());
        }

        /// <summary>
        /// На основании названия фильма добавляет в список рекомендаций похожие фильмы
        /// </summary>
        /// <param name="nameEn"></param>
        private void UpdateTVRandom()
        {
            while (true)
            {
                var dict = new Dictionary<int, RandomTV.Film>();
                Random random = new Random();               
                string[] order = new string[] { "YEAR", "RATING", "NUM_VOTE" };
                var client = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-filters");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                request.AddQueryParameter("type", "TV_SHOW");
                request.AddQueryParameter("order", order[random.Next(0, order.Length)]);
                int opt = random.Next(0, 2);
                if (opt == 0)
                    request.AddQueryParameter("genre", PopularGenres[random.Next(0, PopularGenres.Length)].ToString());
                else
                {
                    int filmYearBottomLine = random.Next(1950, DateTime.Now.Year - 10);
                    int filmYearUpperLine = random.Next(filmYearBottomLine + 10, DateTime.Now.Year + 1);
                    request.AddQueryParameter("yearFrom", filmYearBottomLine.ToString());
                    request.AddQueryParameter("yearTo", filmYearUpperLine.ToString());
                }
                IRestResponse response = client.Execute(request);
                RandomTV.Results results;
                try { results = JsonConvert.DeserializeObject<RandomTV.Results>(response.Content); }
                catch(Exception) { results = null; }
                if (results == null && results.films.Count == 0)
                    continue;
                for (int i = 0; i < results.films.Count; ++i)
                {
                    var t = results.films[i];
                    t.VKPhotoID = Attachments.RandomTVPosterID(t);
                    if (t.VKPhotoID == null)
                        continue;
                    dict[t.filmId] = t;
                }
                TVRandomDict = dict;
                RandomTVIsUpdating = false;
                Users.Unload();
                return;
            }
        }

        public async void AddTVSoundtrackAsync(string name, string addition)
        {
            await Task.Run(() => AddTVSoundtrack(name, addition));
        }

        public void AddTVSoundtrack(string name, string addition)
        {
            string[] song_names;
            var res = new FilmSountracks();
            try
            {
                //song_names = SpotifyTracks.GetTracks(SpotifyPlaylists.SearchPlaylist($"{name} {addition}"), "6").ToArray();
                var tracks = Bot.yandex_api.GetAlbum(Bot.yandex_api.SearchAlbums($"{name} {addition}")[0].Id).Volumes[0];
                song_names = tracks.Take(Math.Min(6, tracks.Count)).Select(n => $"{n.Title} {string.Join(' ', n.Artists.Select(a => a.Name))}").ToArray();
                var audios = new List<Audio>();
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
                res.Update(audios);
                TVTracks[name] = res;
                if (res.Tracks.Count == 0)
                    TVTracks[name].IsEmpty = true;
                Users.Unload();
            }
            catch (Exception)
            {
                TVTracks[name] = res;
                TVTracks[name].IsLoading = false;
                return;
            }
        }


        //-----------------------------------Другие функции--------------------------------------
        /// <summary>
        /// Возвращает последовательность значений словаря в случайном порядке
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new Random();
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }

        /// <summary>
        /// Конвертирует текстовую дату в дату формата DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StringToDate(string date)
        {
            try
            {
                Match m = Regex.Match(date, @"(\d{4})-(\d{2})-(\d{2})");
                return new DateTime(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
            }
            catch(Exception)
            {
                return DateTime.MinValue;
            }
        }
    }


    /// <summary>
    /// класс объекта типа Message.Payload
    /// </summary>
    public class Payload
    {
        public string text { get; set; }
        public string type { get; set; }
        public string nameRu { get; set; }
        public string nameEn { get; set; }
        public string filmId { get; set; }
        public string date { get; set; }
        public string genres { get; set; }
        public string digital_release { get; set; }

        public Payload(string payload)
        {
            if (payload == null)
                return;
            text = Regex.Match(payload, "{\\\".*\\\" *: *\\\"(.*)\\\"}").Groups[1].Value;
            if (!text.Contains(';'))
                return;
            GroupCollection m = Regex.Match(text, "(.*);(.*);(.*);(.*);(.*);(.*);(.*)").Groups;
            type = m[1].Value;
            nameRu = m[2].Value;
            nameEn = m[3].Value;
            filmId = m[4].Value;
            date = m[5].Value;
            genres = m[6].Value;
            digital_release = m[7].Value;
        }

        //public static string PayloadValue(string payload) => payload == null ? "" : Regex.Match(payload, "{\\\".*\\\" *: *\\\"(.*)\\\"").Groups[1].Value; //JsonConvert.DeserializeObject<Payload>(payload).text;
    }
}