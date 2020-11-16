using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public static class Film
    {
        public static void Menu()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    keyboard = null;
                    SendMessage("Введите название фильма");
                    break;

                case Recommendations:
                    template = ;                                         //карусэл сдэлать
                    SendMessage("Рекомендуемые фильмы");
                    break;

                case PlanToWatch:
                    keyboard = ;                                         //dodelat kebord
                    SendMessage("Список планируемых к просмотру фильмов");
                    break;

                case Modes.Mode.Random:
                    keyboard = ;
                    SendMessage("Подробная информация по случайному кинофильму");
                    break;

                case Back:
                    keyboard = ;
                    user.RemoveLevel();
                    SendMessage("Выберите режим");
                    break;

                default:
                    break;
            }
        }

        public static void ButtonClicked()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    if (message.Text == "Хочу посмотреть")
                        SendMessage("Кинофильм добавлен в список планируемых фильмов");

                    else if (message.Text == "Посмотрел")
                        SendMessage("Кинофильм добавлен в просмотренные");                   // реализовать понравилось или нет

                    else if (message.Text == "Саундтрек") 
                        SendMessage("Саундтрэк из фильма");                                  //реализовать список аудиозаписей
                    
                    else if (message.Text == "Что поесть") 
                        SendMessage("Видео-инструкция по приготовлению еды для просмотра");

                    else if (message.Text == "Не показывать")
                        SendMessage("Фильм добавлен в черный список");

                    else if (message.Text == "Да")
                        SendMessage("???");

                    else if (message.Text == "Нет")
                        SendMessage("???");
                    else
                      SendMessage($"Подробная информация по кинофильму {message.Text}");
                    break;

                case Back:
                    keyboard = ;
                    user.RemoveLevel();
                    SendMessage("Выберите режим");
                    break;

                case Recommendations:
                    if (message.Text == "Подробнее")
                        SendMessage("Подробная информация по выбраному кинофильму");
                    break;

                case PlanToWatch:
                    if (message.Text == "Уже посмотрел")
                        SendMessage("Введите название просмотренного кинофильма");
                    else SendMessage($"Кинофильм {message.Text} успешно удален из списка планируемых фильмов");
                    break;

                case Modes.Mode.Random:
                    if (message.Text == "Хочу посмотреть")
                        SendMessage("Кинофильм добавлен в список планируемых фильмов");

                    else if (message.Text == "Посмотрел")
                        SendMessage("Кинофильм добавлен в просмотренные");                   // реализовать понравилось или нет

                    else if (message.Text == "Саундтрек")
                        SendMessage("Саундтрэк из фильма");                                  //реализовать список аудиозаписей

                    else if (message.Text == "Что поесть")
                        SendMessage("Видео-инструкция по приготовлению еды для просмотра");

                    else if (message.Text == "Не показывать")
                        SendMessage("Фильм добавлен в черный список");

                    else if (message.Text == "Да")
                        SendMessage("???");

                    else if (message.Text == "Нет")
                        SendMessage("???");                                     
                    break;
                default:
                    break;
            }
        }

    }
}
