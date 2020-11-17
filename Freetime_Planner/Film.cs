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
                        SendMessage("Подробная информация по кинофильму");
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
                switch (user.CurrentLevel())
                {
                    case Search:
                        keyboard = null;
                        SendMessage("Введите название фильма");
                        break;

                    case Recommendations:
                        template = Keyboards.FilmMyRecomenation();
                        keyboard = null;
                        SendMessage("Рекомендуемые фильмы");
                        template = null;
                        break;

                    case PlanToWatch:
                        keyboard = Keyboards.FilmPlanToWatch();                                         //dodelat kebord
                        SendMessage("Список планируемых к просмотру фильмов");
                        keyboard = null;
                        break;

                    case Modes.Mode.Random:
                        keyboard = Keyboards.RandomFilm();
                        SendMessage("Подробная информация по случайному кинофильму");
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
                    SendMessage("Подробная информация по кинофильму");
                    keyboard = null;
                    break;
                case AlreadyWatched:
                    SendMessage("Фильм перенес в список просмотренных");
                    keyboard = Keyboards.FilmWatched();
                    SendMessage("Понравился фильм?");
                    keyboard = null;
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
                    if (message.Payload == null)
                        SendMessage($"Подробная информация по кинофильму {message.Text}");
                    else
                    {
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
                            SendMessage("Круто!");

                        else if (message.Text == "Нет")
                            SendMessage("Сочувствую...");
                    }
                    break;

                case Back:
                    keyboard = Keyboards.Mainmenu();
                    user.RemoveLevel();
                    user.RemoveLevel();
                    SendMessage("Жми любую кнопку");
                    break;

                case Recommendations:
                    if (message.Text == "Подробнее")
                        SendMessage("Подробная информация по выбраному кинофильму");
                    else
                        SendMessage("Выберите кнопку 'Подробнее' или любую кнопку из выпадающего меню");
                    break;

                case PlanToWatch:
                    if (message.Payload == string.Empty)
                        SendMessage($"Кинофильм {message.Text} успешно удален из списка планируемых фильмов");
                    else
                    {
                        if (message.Text == "Уже посмотрел")
                            SendMessage("Введите название просмотренного кинофильма");
                        else 
                            SendMessage($"Выберите кнопку 'Уже посмотрел' или любую кнопку из выпадающего меню");
                    }
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
                        SendMessage("Круто");

                    else if (message.Text == "Нет")
                        SendMessage("Сочувствую");                                     
                    break;
                default:
                    break;
            }
        }*/

    }
}
