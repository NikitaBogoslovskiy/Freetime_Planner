using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{

    public class Movie
    {

        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Genres { get; set; }

        public string ReleaseDate { get; set; }

        public double Rate { get; set; }

        public string ImageURL { get; set; }

        public string FilmLength { get; set; }

        public string AgeLimit { get; set; }

        public string Budget { get; set; }

        public string Countries { get; set; }

        Movie(string _ID, string _Name, string _Description, string _Genres, string _ReleaseDate, double _Rate, string _ImageURL, string _FilmLength, string _AgeLimit, string _Budget, string _Countries)
        {
            ID = _ID;
            Name = _Name;
            Description = _Description;
            Genres = _Genres;
            ReleaseDate = _ReleaseDate;
            Rate = _Rate;
            ImageURL = _ImageURL;
            FilmLength = _FilmLength;
            AgeLimit = _AgeLimit;
            Budget = _Budget;
            Countries = _Countries;
        }

        Movie()
        { 
        
        }

        public static Dictionary<string, Movie> Films { get; set; }

        public static void InitFilms()
        {
            Films = new Dictionary<string, Movie>
            {
                //Шрек третий
                ["84020"] = new Movie("84020",
                "Шрэк Третий",
                "Король Гарольд внезапно умер, и теперь великан Шрек вынужден стать королем Далекой-Далекой страны. " +
                "Шрек уважает семейные традиции своей жены Фионы, но править страной очень не хочет, и поэтому и отправляется с Ослом и Котом в сапогах на поиски нового короля. " +
                "Но пока Шрек ищет короля Артура - другого наследника, принц Чарминг замышляет новые пакости.",
                "ультфильм, фэнтези, комедия, приключения, семейный",
                "17 мая 2007",
                6.676,
                "https://avatars.mds.yandex.net/get-kinopoisk-image/1900788/3e76a63d-16ba-4609-a691-41358270e93d/300x450",
                "01:33",
                "0+",
                "$160 000 000",
                "США"),

                //Матрица
                ["301"] = new Movie("301",
                "Матрица",
                "Жизнь Томаса Андерсона разделена на две части: днём он - самый обычный офисный работник, получающий нагоняи от начальства, а ночью превращается в хакера по имени Нео, и нет места в сети, куда он не смог бы дотянуться. " +
                "Но однажды всё меняется — герой, сам того не желая, узнаёт страшную правду: всё, что его окружает — не более, чем иллюзия, Матрица, а люди — всего лишь источник питания для искусственного интеллекта, поработившего человечество. " +
                "И только Нео под силу изменить расстановку сил в этом чужом и страшном мире.",
                "фантастика, боевик",
                "14 октября 1999",
                8.489,
                "https://avatars.mds.yandex.net/get-kinopoisk-image/1704946/eed1de3a-5400-43b3-839e-22490389bf54/300x450",
                "02:16",
                "16+",
                "$63 000 000",
                "США"),

                //Аватар
                ["251733"] = new Movie("251733",
                "Аватар",
                "Джейк Салли - бывший морской пехотинец, прикованный к инвалидному креслу. Несмотря на немощное тело, Джейк в душе по-прежнему остается воином. " +
                "Он получает задание совершить путешествие в несколько световых лет к базе землян на планете Пандора, где корпорации добывают редкий минерал, имеющий огромное значение для выхода Земли из энергетического кризиса.",
                "фантастика, боевик, драма, приключения",
                "17 декабря 2009",
                7.936,
                "https://avatars.mds.yandex.net/get-kinopoisk-image/1599028/4adf61aa-3cb7-4381-9245-523971e5b4c8/300x450",
                "02:42",
                "12+",
                "$237 000 000",
                "США"),


            };
        }

        /// <summary>
        /// Основное меню кнопки "Фильмы"
        /// </summary>
        public static void Menu()
        {
            //inline-кнопки
            if (Payload.PayloadValue(message.Payload) != string.Empty)
            {
                switch (user.CurrentLevel())
                {
                    case Watched:
                        keyboard = Keyboards.FilmWatched();
                        SendMessage("Понравился фильм?");
                        keyboard = null;
                        break;

                    case WantToWatch:
                        SendMessage("Добавлено в список планируемых фильмов");
                        break;

                    case Soundtrack:
                        SendMessage("Саундтрек к кинофильму");
                        break;

                    case GenreFood:
                        SendMessage("Видео-инструкция приготовления");
                        break;

                    case BlackList:
                        SendMessage("Добавлено в список нежелаемых фильмов");
                        break;

                    case More:
                        keyboard = Keyboards.FilmSearch();
                        var f = Films[Payload.PayloadValue(message.Payload)];
                        SendMessage($"{f.Name}\nСтрана: {f.Countries}\nЖанр: {f.Genres}\nБюджет: {f.Budget}\nДата премьеры: {f.ReleaseDate}\nВозраст: {f.AgeLimit}\nВремя: {f.FilmLength}\nРейтинг: {f.Rate}\nОписание: {f.Description}");
                        keyboard = null;
                        break;

                    case AlreadyWatched:
                        SendMessage("Введите название просмотренного кинофильма");
                        break;

                    case Yes:
                        SendMessage("Круто! Будем советовать похожие");
                        break;

                    case No:
                        SendMessage("Жаль... Постараемся подобрать что-нибудь получше");
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
                        SendMessage("Введите название фильма");
                        break;

                    case Recommendations:
                        template = Keyboards.FilmMyRecomenation();                                   //ошибка в слове "Рекомендации" 
                        keyboard = null;
                        SendMessage("Рекомендуемые фильмы");
                        template = null;
                        break;

                    case PlanToWatch:
                        keyboard = Keyboards.FilmPlanToWatch();                                        
                        SendMessage($"Список планируемых к просмотру фильмов:\n{Films["84020"].Name}\n{Films["301"].Name}\n{Films["251733"].Name}");
                        keyboard = null;
                        break;

                    case Modes.Mode.Random:
                        keyboard = Keyboards.RandomFilm();
                        var f = new Movie();
                        var r = new Random();
                        int n = r.Next(0, 2);
                        int i = 0;
                        foreach (var item in Films.Values)
                        {
                            if (i == n)
                            {
                                f = item;
                                break;
                            }
                            i++;
                        }
                        
                        SendMessage($"{f.Name}\nСтрана: {f.Countries}\nЖанр: {f.Genres}\nБюджет: {f.Budget}\nДата премьеры: {f.ReleaseDate}\nВозраст: {f.AgeLimit}\nВремя: {f.FilmLength}\nРейтинг: {f.Rate}\nОписание: {f.Description}");
                        keyboard = null;
                        break;

                    case Back:
                        keyboard = Keyboards.Mainmenu();
                        user.RemoveLevel();
                        SendMessage("Жми любую кнопку");
                        keyboard = null;
                        break;
                    default:
                        SendMessage("Вероятно, ты ввел текстовую команду, не нажав кнопку. Используй кнопки");
                        break;
                }
            }

            if (user.CurrentLevel() != Search && user.CurrentLevel() != AlreadyWatched)
                user.RemoveLevel();
        }

        /// <summary>
        /// Второй уровень вложенности кнопки "Фильмы"
        /// </summary>
        public static void SecondLevel()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    keyboard = Keyboards.FilmSearch();
                    var f = new Movie();
                    foreach (var item in Films.Values)
                    {
                        if (item.Name == message.Text)
                        {
                            f = item;
                        }
                    }
                    SendMessage($"{f.Name}\nСтрана: {f.Countries}\nЖанр: {f.Genres}\nБюджет: {f.Budget}\nДата премьеры: {f.ReleaseDate}\nВозраст: {f.AgeLimit}\nВремя: {f.FilmLength}\nРейтинг: {f.Rate}\nОписание: {f.Description}");
                    keyboard = null;
                    break;
                case AlreadyWatched:
                    SendMessage("Фильм перенес в список просмотренных");
                    keyboard = Keyboards.FilmWatched();
                    SendMessage("Понравился фильм?");
                    keyboard = null;
                    break;
                default:
                    break;                     //Сделано:Подробная информация, Планируемые к просмотру фильмы , Рекомендуемые к просмотру
            }
            user.RemoveLevel();
        }

    }
}
