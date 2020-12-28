using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Freetime_Planner.Modes;
using Newtonsoft.Json;
using VkNet.Model.Template;
using System.Threading.Tasks;

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

        public Dictionary<int, Film.FilmObject> FilmRecommendations { get; set; }

        public List<Film.FilmObject>[] PlannedFilms { get; set; }
        
        public List<int> HiddenFilms { get; set; }

        public Dictionary<int, TV.TVObject> TVRecommendations { get; set; }

        public List<TV.TVObject> PlannedTV { get; set; }

        public List<int> HiddenTV { get; set; }

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
            FilmRecommendations = new Dictionary<int, Film.FilmObject>();
            HiddenFilms = new List<int>();
            PlannedFilms = new List<Film.FilmObject>[] { new List<Film.FilmObject>(), new List<Film.FilmObject>() };
            TVRecommendations = new Dictionary<int, TV.TVObject>();
            HiddenTV = new List<int>();
            PlannedTV = new List<TV.TVObject>();
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

        //--------------Пользовательские методы для фильмов--------------

        /// <summary>
        /// Возвращает карусель фильмов из списка рекомендаций
        /// </summary>
        /// <returns></returns>
        public MessageTemplate GetFilmRecommendations()
        {
            return null;
        }

        /// <summary>
        /// Возвращает список планируемых фильмов в текстовом виде (с разделением на вышедшие фильмы и те, что выйдут)
        /// </summary>
        /// <returns></returns>
        public string GetPlannedFilms()
        {
            return "<планируемые фильмы>";
        }

        /// <summary>
        /// Добавляет фильм в список планируемых фильмов и обновляет список рекомендаций
        /// </summary>
        /// <param name="nameRu"></param>
        /// <param name="nameEn"></param>
        /// <param name="Date"></param>
        public void AddPlannedFilm(string nameRu, string nameEn, string Date)
        {
            UpdateFilmRecommendationsAsync(nameEn);
            return;
        }

        /// <summary>
        /// Обновляет список рекомендации на основании понравившегося фильма
        /// </summary>
        /// <param name="nameEn"></param>
        public void LikeFilm(string nameEn)
        {
            UpdateFilmRecommendationsAsync(nameEn);
            return;
        }

        /// <summary>
        /// Добавляет фильм в черный список и удаляет его из рекомендаций, если он там есть
        /// </summary>
        /// <param name="filmID"></param>
        public void HideFilm(int filmID)
        {
            //также необходимо редактировать список рекомендаций
            return;
        }

        /// <summary>
        /// Удаляет фильм из списка планируемых фильмов и добавляет его в черный список через вызов HideFilm()
        /// </summary>
        /// <param name="index"></param>
        /// <param name="filmID"></param>
        public void AlreadyWatchedFilm(int index, int filmID)
        {
            //нужно удалить фильм из списка PlannedFilms
            HideFilm(filmID);
            return;
        }

        /// <summary>
        /// Возвращает информацию о фильме в текстовом виде по его ID
        /// </summary>
        /// <param name="filmID"></param>
        /// <returns></returns>
        public string FullFilmInfo(int filmID)
        {
            return null;
        }

        /// <summary>
        /// Асинхронно вызывает функцию UpdateFilmRecommendations
        /// </summary>
        /// <param name="nameEn"></param>
        private async void UpdateFilmRecommendationsAsync(string nameEn)
        {
            await Task.Run(()=> UpdateFilmRecommendations(nameEn));
        }

        /// <summary>
        /// На основании названия фильма добавляет в список рекомендаций похожие фильмы
        /// </summary>
        /// <param name="nameEn"></param>
        private void UpdateFilmRecommendations(string nameEn)
        {

        }

        //--------------Пользовательские методы для сериалов--------------

        public MessageTemplate GetTVRecommendations()
        {
            return null;
        }

        public string GetPlannedTV()
        {
            return "<планируемые сериалы>";
        }

        public void AddPlannedTV(string nameRu, string nameEn)
        {
            UpdateTVRecommendationsAsync(nameEn);
            return;
        }

        public void LikeTV(string nameEn)
        {
            UpdateTVRecommendationsAsync(nameEn);
            return;
        }

        public void HideTV(int TVID)
        {
            //также необходимо редактировать список рекомендаций
            return;
        }
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

        private async void UpdateTVRecommendationsAsync(string nameEn)
        {
            await Task.Run(() => UpdateTVRecommendations(nameEn));
        }

        private void UpdateTVRecommendations(string nameEn)
        {

        }
    }

    /// <summary>
    /// класс объекта типа Message.Payload
    /// </summary>
    public class Payload
    {
        public string text { get; set; }
        public string nameRu { get; set; }
        public string nameEn { get; set; }
        public string filmId { get; set; }
        public string date { get; set; }
        public string genres { get; set; }

        public Payload(string payload)
        {
            if (payload == null)
                return;
            text = Regex.Match(payload, "{\\\".*\\\" *: *\\\"(.*)\\\"}").Groups[1].Value;
            if (!text.Contains(';'))
                return;
            GroupCollection m = Regex.Match(text, "(.*);(.*);(.*);(.*);(.*)").Groups;
            nameRu = m[1].Value;
            nameEn = m[2].Value;
            filmId = m[3].Value;
            date = m[4].Value;
            genres = m[5].Value;
        }

        //public static string PayloadValue(string payload) => payload == null ? "" : Regex.Match(payload, "{\\\".*\\\" *: *\\\"(.*)\\\"").Groups[1].Value; //JsonConvert.DeserializeObject<Payload>(payload).text;
    }
}
