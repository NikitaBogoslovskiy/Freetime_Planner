using System.Collections.Generic;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public class TV
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genres { get; set; }
        public string DateRelease { get; set; }
        public double Rate { get; set; }
        public string ImageURL { get; set; }


        public string SeriaLength { get; set; }

        //public string AgeLimit { get; set; }

        // public string Budget { get; set; }

        public string Countries { get; set; }

        public int Siasons { get; set; }

         TV(string _ID, string _Name, string _Description, string _Genres, string _DateRelease, double _Rate, string _ImageURL, string _SeriaLength, string _Countries, int _Siasons)
        {
            ID = _ID;//1
            Name = _Name;//2
            Description = _Description;//3
            Genres = _Genres;//4
            DateRelease = _DateRelease;//5
            Rate = _Rate;//6
            ImageURL = _ImageURL;//7
            SeriaLength = _SeriaLength;//8
            Countries = _Countries;//9
            Siasons = _Siasons;//10
        }

        public static Dictionary<string, TV> Series { get; set; }

        public static void InitSeries()
        {
            Series = new Dictionary<string, TV>
            {
                ["77044"] = new TV("77044", "Друзья", "Главные герои - шестеро друзей - Рейчел, Моника, Фиби, Джоуи, Чендлер и Росс." + " Три девушки и три парня, которые дружат, живут по соседству, вместе убивают время" +
                " и противостоят жестокой реальности, делятся своими секретами и иногда очень сильно влюбляются.", "комедия,мелодрама",
                "22.09.1994", 9.3, "https://www.kinopoisk.ru/series/77044/", "22 мин.", "США", 10),
                ["1032606"] = new TV("1032606", "Тьма ", "История четырёх семей, живущих спокойной и размеренной жизнью в маленьком немецком городке." +
                " Видимая идиллия рушится, когда бесследно исчезают двое детей и воскресают тёмные тайны прошлого.",
                "триллер, фантастика, драма, криминал, детектив", "9 сентября 2017", 8.2, "https://www.kinopoisk.ru/series/1032606/", "60 мин.", "Германия, США", 3),
                ["492613"] = new TV("492613", "Оборотень",  "Сериал крутится вокруг молодого игрока в лакросс по имени Скотт МакКол. Однажды ночью он блуждал по лесу в поисках трупа и подвергся нападению оборотня." +
                " Ему удалось убежать, получив лишь небольшой укус. Но чуть позднее, он начал замечать некоторые изменения в себе.",
                "фэнтези, боевик, триллер, драма, мелодрама", "7.12.2011", 8.0, "https://www.kinopoisk.ru/series/492613/", "43 мин.", "США", 6),
                ["859908"] = new TV("859908", "Мистер Робот", "История молодого программиста Эллиота, страдающего социофобией и решившего, что единственный приемлемый для него" +
                " способ взаимодействия с людьми — это профессия хакера. Таким образом, он быстро оказывается в том самом месте, где пересекаются интересы его работодателя — фирмы, занимающейся кибербезопасностью," +
                " — и подпольных организаций, которые пытаются его завербовать с целью обрушения самых могучих американских корпораций.",
                "триллер, драма, криминал", "6.02.2017", 7.8, "https://www.kinopoisk.ru/series/859908/", "49 мин.", "США", 4),
            };

        }

        /// <summary>
        /// Основное меню кнопки "Сериалы"
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
                        SendMessage("Понравился сериал?");
                        keyboard = null;
                        break;

                    case WantToWatch:
                        SendMessage("Добавлено в список планируемых сериалов");
                        break;

                    case Soundtrack:
                        SendMessage("Саундтрек к сериалу");
                        break;

                    case GenreFood:
                        SendMessage("Видео-инструкция приготовления");
                        break;

                    case BlackList:
                        SendMessage("Добавлено в список нежелаемых сериалов");
                        break;

                    case More:
                        keyboard = Keyboards.FilmSearch();
                        SendMessage("Подробная информация по сериалу");
                        keyboard = null;
                        break;

                    case AlreadyWatched:
                        SendMessage("Введите название просмотренного сериала");
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
                        SendMessage("Введите название сериала");
                        break;

                    case Recommendations:
                        keyboard = null;
                        template = Keyboards.FilmMyRecomenation();                                        
                        SendMessage("Рекомендуемые сериалы");
                        template = null;
                        break;

                    case PlanToWatch:
                        keyboard = Keyboards.FilmPlanToWatch();                                         
                        SendMessage("Список планируемых к просмотру сериалов");
                        keyboard = null;
                        break;

                    case Modes.Mode.Random:
                        keyboard = Keyboards.RandomFilm();
                        SendMessage("Подробная информация по случайному сериалу");
                        keyboard = null;
                        break;
                    case Back:
                        keyboard = Keyboards.Mainmenu();
                        user.RemoveLevel();
                        SendMessage("Жми любую кнопку");
                        keyboard = null;
                        break;
                    default:
                        SendMessage("Вероятно, ты ввел словесно команду, не нажав кнопку. Используй кнопки");
                        break;
                }
            }

            if (user.CurrentLevel() != Search && user.CurrentLevel() != AlreadyWatched)
                user.RemoveLevel();
        }

        /// <summary>
        /// Второй уровень вложенности кнопки "Сериалы"
        /// </summary>
        public static void SecondLevel()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    keyboard = Keyboards.FilmSearch();
                    SendMessage("Подробная информация по сериалу");
                    keyboard = null;
                    break;
                case AlreadyWatched:
                    SendMessage("Сериал перенес в список просмотренных");
                    keyboard = Keyboards.FilmWatched();
                    SendMessage("Понравился сериал?");
                    break;
                default:
                    break;
            }
            user.RemoveLevel();
        }
    }
}
