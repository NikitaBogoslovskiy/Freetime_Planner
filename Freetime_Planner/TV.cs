﻿using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public static class TV
    {
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