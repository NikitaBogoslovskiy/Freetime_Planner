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
                switch (user.CurrentLevel())
                {
                    case Search:
                        keyboard = null;
                        SendMessage("Введите название сериала");
                        break;

                    case Recommendations:
                        keyboard = null;
                        template = Keyboards.FilmMyRecomenation();                                         //карусэл сдэлать
                        SendMessage("Рекомендуемые сериалы");
                        template = null;
                        break;

                    case PlanToWatch:
                        keyboard = Keyboards.FilmPlanToWatch();                                         //dodelat kebord
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

        /*public static void ButtonClicked()
        {
            switch (user.CurrentLevel())
            {
                case Search:
                    if (message.Payload == string.Empty)
                        SendMessage($"Подробная информация по сериалу {message.Text}");
                    else
                    {
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
                            SendMessage("Круто!");

                        else if (message.Text == "Нет")
                            SendMessage("Сочувствую");
                    }
                    break;

                case Back:
                    keyboard = Keyboards.Mainmenu();
                    user.RemoveLevel();
                    user.RemoveLevel();
                    SendMessage("Выберите режим");
                    break;

                case Recommendations:
                    if (message.Text == "Подробнее")
                        SendMessage("Подробная информация по выбраному сериалу");
                    else
                        SendMessage("Выберите кнопку 'Подробнее' или любую кнопку из выпадающего меню");
                    break;

                case PlanToWatch:
                    if (message.Payload == string.Empty)
                        SendMessage($"Сериал {message.Text} успешно удален из списка планируемых фильмов");
                    else
                    {
                        if (message.Text == "Уже посмотрел")
                            SendMessage("Введите название просмотренного сериала");
                        else
                            SendMessage($"Выберите кнопку 'Уже посмотрел' или любую кнопку из выпадающего меню");
                    }
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
                        SendMessage("Круто");

                    else if (message.Text == "Нет")
                        SendMessage("Сочувствую...");
                    break;
                default:
                    break;
            }
        }
        */
    }
}
