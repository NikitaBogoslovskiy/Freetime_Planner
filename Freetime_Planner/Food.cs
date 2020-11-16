using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;


namespace Freetime_Planner
{
    public static class Food
    {
        public static void Menu()
        {
            switch (user.CurrentLevel())
            {
                case Snack:
                    SendMessage("Видео-инструкция по приготовлению закуски");
                    break;

                case Dessert:
                    SendMessage("Видео-инструкция по приготовлению десерта");
                    break;

                case Cocktails:
                    SendMessage("Видео-инструкция по приготовлению коктейля");
                    break;

                case Back:
                    keyboard = ;
                    user.RemoveLevel();
                    SendMessage("Выберите режим");
                    break;

            }
        }
    }
}
