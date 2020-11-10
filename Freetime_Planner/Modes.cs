using System;
using System.Collections.Generic;
using System.Text;

namespace Freetime_Planner
{
    public static class Modes
    {
        public enum Mode { Default, Film, TV, Food, Search, Genres, Recommendations, PlanToWatch, Random, Soundtrack, Back } 
        public static Mode ConvertIntoMode(string message)
        {
            switch (message)
            {
                case "Фильмы": return Mode.Film;
                case "Сериалы": return Mode.TV;
                case "Еда под просмотр": return Mode.Food;
                case "По названию": return Mode.Search;
                case "По жанру": return Mode.Genres;
                case "Рекомендации": return Mode.Recommendations;
                case "Планирую смотреть": return Mode.PlanToWatch;
                case "Рандом": return Mode.Random;
                case "Саундтрек": return Mode.Soundtrack;
                case "Назад": return Mode.Back;
                default: return Mode.Default;
            }
        }
    }
}
