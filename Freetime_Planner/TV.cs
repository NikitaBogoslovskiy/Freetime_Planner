using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public static class TV
    {
        public static void Menu()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    keyboard = null;
                    SendMessage("Введите название сериала");
                    break;

                case Recommendations:
                    template = ;                                         //карусэл сдэлать
                    SendMessage("Рекомендуемые сериалы");
                    break;

                case PlanToWatch:
                    keyboard = ;                                         //dodelat kebord
                    SendMessage("Список планируемых к просмотру сериалов");
                    break;

                case Modes.Mode.Random:
                    keyboard = ;
                    SendMessage("Подробная информация по сериалу");
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
                        SendMessage("Сериал добавлен в список планируемых фильмов");

                    else if (message.Text == "Посмотрел")
                        SendMessage("Сериал добавлен в просмотренные");                   // реализовать понравилось или нет

                    else if (message.Text == "Саундтрек")
                        SendMessage("Саундтрэк из сериала");                                  //реализовать список аудиозаписей

                    else if (message.Text == "Что поесть")
                        SendMessage("Видео-инструкция по приготовлению еды для просмотра");

                    else if (message.Text == "Не показывать")
                        SendMessage("Сериал добавлен в черный список");

                    else if (message.Text == "Да")
                        SendMessage("???");

                    else if (message.Text == "Нет")
                        SendMessage("???");
                    else
                        SendMessage($"Подробная информация по сериалу {message.Text}");
                    break;

                case Back:
                    keyboard = ;
                    user.RemoveLevel();
                    SendMessage("Выберите режим");
                    break;

                case Recommendations:
                    if (message.Text == "Подробнее")
                        SendMessage("Подробная информация по выбраному сериалу");
                    break;

                case PlanToWatch:
                    if (message.Text == "Уже посмотрел")
                        SendMessage("Введите название просмотренного сериала");
                    else SendMessage($"Сериал {message.Text} успешно удален из списка планируемых сериалов");
                    break;

                case Modes.Mode.Random:
                    if (message.Text == "Хочу посмотреть")
                        SendMessage("Сериал добавлен в список планируемых сериалов");

                    else if (message.Text == "Посмотрел")
                        SendMessage("Сериал добавлен в просмотренные");                   // реализовать понравилось или нет

                    else if (message.Text == "Саундтрек")
                        SendMessage("Саундтрэк из сериала");                                  //реализовать список аудиозаписей

                    else if (message.Text == "Что поесть")
                        SendMessage("Видео-инструкция по приготовлению еды для просмотра");

                    else if (message.Text == "Не показывать")
                        SendMessage("Сериал добавлен в черный список");

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
