using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Model.Keyboard;

namespace Freetime_Planner
{
    public static class Modes
    {
        public enum Mode
        {
            Default, Film, TV, Food, Search, Recommendations, PlanToWatch, Random, Dessert,
            Snack, Cocktails, WantToWatch, Watched, Soundtrack, GenreFood, BlackList, More, AlreadyWatched, Yes, No, Back, Help, WhereToWatch, Options,
            MailFrequency, Everyday, ThreeDays, FiveDays, EveryWeek, NoMail, DietMode, NoLimit, HealthyFood, Actors, MoreAboutActor, SearchGenre,
            GenreFiction, GenreThriller, GenreComedy, GenreAnime, GenreDrama, GenreMilitary, GenreFamily, GenreHoror, GenreDetective, GenreCriminal, 
            GenreFantasy, GenreBoevik

        }

        /// <summary>
        /// Конвертер текстовой команды из главного меню в объект типа Mode
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Mode MainMenu(string message)
        {
            switch (message)
            {
                case "Фильмы": return Mode.Film;
                case "Сериалы": return Mode.TV;
                case "Еда под просмотр": return Mode.Food;
                case "Помощь": return Mode.Help;
                case "Настройки": return Mode.Options;
                default: throw new ArgumentException("Нажимай кнопки в меню");
            }
        }

        /// <summary>
        /// Конвертер текстовой команды из вложенного меню в объект типа Mode
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Mode SecondMenu(string message)
        {
            switch (message)
            {
                case "По названию": return Mode.Search;
                case "Рекомендовано": return Mode.Recommendations;
                case "Запланировано": return Mode.PlanToWatch;
                case "Случайный фильм": return Mode.Random;
                case "Случайный сериал": return Mode.Random;
                case "Сладкое": return Mode.Dessert;
                case "Закуски": return Mode.Snack;
                case "Коктейли": return Mode.Cocktails;
                case "Хочу посмотреть": return Mode.WantToWatch;
                case "Посмотрел": return Mode.Watched;
                case "Саундтрек": return Mode.Soundtrack;
                case "Актеры": return Mode.Actors;
                case "Узнать больше": return Mode.MoreAboutActor;
                case "Еда": return Mode.GenreFood;
                case "Не показывать": return Mode.BlackList;
                case "Подробнее": return Mode.More;
                case "Уже посмотрел": return Mode.AlreadyWatched;
                case "Да": return Mode.Yes;
                case "Нет": return Mode.No;
                case "Помощь": return Mode.Help;
                case "Назад": return Mode.Back;
                case "Смотреть": return Mode.WhereToWatch;
                case "Частота рассылки": return Mode.MailFrequency;
                case "Ежедневно": return Mode.Everyday;
                case "Раз в три дня": return Mode.ThreeDays;
                case "Раз в пять дней": return Mode.FiveDays;
                case "Раз в неделю": return Mode.EveryWeek;
                case "Без рассылки": return Mode.NoMail;
                case "Режим питания": return Mode.DietMode;
                case "Без ограничений": return Mode.NoLimit;
                case "Здоровое питание": return Mode.HealthyFood;
                case "По жанру": return Mode.SearchGenre;
                default: throw new ArgumentException("Нажимай кнопки в меню");
            }
        }

        public static Mode ThirdMenu(string message)
        {
            switch (message)
            {
                case "Триллер": return Mode.GenreThriller;
                case "Фэнтези": return Mode.GenreFantasy;
                case "Криминал": return Mode.GenreCriminal;
                case "Детектив": return Mode.GenreDetective;
                case "Фантастика": return Mode.GenreFiction;
                case "Боевик": return Mode.GenreBoevik;
                case "Комедия": return Mode.GenreComedy;
                case "Аниме": return Mode.GenreAnime;
                case "Драма": return Mode.GenreDrama;
                case "Военный": return Mode.GenreMilitary;
                case "Семейный": return Mode.GenreFamily;
                case "Ужасы": return Mode.GenreHoror;
                case "Назад": return Mode.Back;
                default: throw new ArgumentException("Нажимай кнопки в меню");
            }
        }

        public static MessageKeyboard LevelKeyboard(Mode m)
        {
            switch(m)
            {
                case Mode.Default: return Keyboards.MainKeyboard;
                case Mode.Film: return Keyboards.FilmKeyboard;
                case Mode.TV: return Keyboards.TVKeyboard;
                case Mode.Food: return Keyboards.FoodKeyboard;
                case Mode.Options: return Keyboards.Options();
                case Mode.SearchGenre: return Keyboards.GenresKeyboard;
                default: return null;
            }
        }
    }
}
