using System;
using System.Collections.Generic;
using System.Text;

namespace Freetime_Planner
{
    public static class Modes
    {
        public enum Mode { Default, Film, TV, Food, Search, Recommendations, PlanToWatch, Random, Dessert , Snack, Cocktails , Back } 
        public static Mode ConvertIntoMode(string message)
        {
            switch (message)
            {
                case "Фильмы": return Mode.Film;
                case "Сериалы": return Mode.TV;
                case "Еда под просмотр": return Mode.Food;
                case "По названию": return Mode.Search;
                case "Рекомендации": return Mode.Recommendations;
                case "Планирую смотреть": return Mode.PlanToWatch;
                case "Рандом": return Mode.Random;
                case "Сладкое": return Mode.Dessert;
                case "Закуски": return Mode.Snack;
                case "Коктейли": return Mode.Cocktails;
                case "Назад": return Mode.Back;
                default: return Mode.Default;
            }
        }
    }
}
