using System;
using System.Collections.Generic;
using System.Text;

namespace Freetime_Planner
{
    public static class Modes
    {
        public enum Mode { Default, Film, TV, Food, Search, Recommendations, PlanToWatch, Random, Dessert,
            Snack, Cocktails, WantToWatch, Watched, Soundtrack, GenreFood, BlackList, More, AlreadyWatched, Yes, No, Back, Help, WhereToWatch } 

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
                case "Поиск по названию": return Mode.Search;
                case "Мои рекомендации": return Mode.Recommendations;
                case "Планирую посмотреть": return Mode.PlanToWatch;
                case "Рандомный фильм": return Mode.Random;
                case "Рандомный сериал": return Mode.Random;
                case "Сладкое": return Mode.Dessert;
                case "Закуски": return Mode.Snack;
                case "Коктейли": return Mode.Cocktails;
                case "Хочу посмотреть": return Mode.WantToWatch;
                case "Посмотрел": return Mode.Watched;
                case "Саундтрек": return Mode.Soundtrack;
                case "Еда": return Mode.GenreFood;
                case "Не показывать": return Mode.BlackList;
                case "Подробнее": return Mode.More;
                case "Уже посмотрел": return Mode.AlreadyWatched;
                case "Да": return Mode.Yes;
                case "Нет": return Mode.No;
                case "Помощь": return Mode.Help;
                case "Назад": return Mode.Back;
                case "Где посмотреть": return Mode.WhereToWatch;
                default: throw new ArgumentException("Нажимай кнопки в меню");
            }
        }
    }
}
