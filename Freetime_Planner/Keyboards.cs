using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;
using static VkNet.Enums.SafetyEnums.KeyboardButtonColor;
using  VkNet.Model.Template;
using static VkNet.Model.Template.MessageTemplate;
using static VkNet.Enums.SafetyEnums.TemplateType;
using VkNet.Model.Template.Carousel;
using System.Linq;


namespace Freetime_Planner
{
    public static class Keyboards
    {
        public static MessageKeyboard Mainmenu()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Фильмы", "", Primary, "text");
            button.AddButton("Сериалы", "", Primary, "text");
            button.AddLine();

            button.AddButton("Еда под просмотр", "", Primary, "text");


            return button.Build();
        }
       
        

        

        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Еда под просмотр" (Build MessageKeyboard for button "Еда под просмотр")
        /// </summary>
        /// <button></button>
        public static MessageKeyboard Food()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Закуски", "", Primary, "text");
            button.AddLine();
            button.AddButton("Сладкое", "", Primary, "text");
            button.AddLine();
            button.AddButton("Коктейли", "", Primary, "text");
            button.AddLine();
            button.AddButton("Назад", "", Default, "text");

            return button.Build();
        }

        //Фильмы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Фильмы" (Build MessageKeyboard for button "Фильмы")
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard Film()
        {

            var result = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            result.Clear();
            //"Поиск по названию"
            result.AddButton("Поиск по названию", "", Primary, "text");
            result.AddLine();
            //"Мои рекомендации"
            result.AddButton("Мои рекомендации", "", Primary, "text");
            result.AddLine();
            //"Планирую посмотреть"
            result.AddButton("Планирую посмотреть", "", Primary, "text");
            result.AddLine();
            //"Рандомный фильм"
            result.AddButton("Рандомный фильм", "", Primary, "text");
            result.AddLine();
            //"Назад"
            result.AddButton("Назад", "", Default, "text");

            return result.Build();
 
        }

        #region Film
        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Фильмы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>
        public static MessageKeyboard FilmSearch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", "filmID", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", "filmID", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", "filmID", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", "filmID", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", "filmID", Negative, "text");

            button.SetInline();
            return button.Build();
        }
      
        //"Фильмы"->"Мои рекомендации"
        public static MessageTemplate FilmMyRecomenation()
        {
           var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            // carousel.Elements
            var arr = new List<CarouselElement>();
            for (var i=0;i<5;i++)
            {
                arr.Add(CarouselElem("Название Фильма", "Жанры"));
            }
            carousel.Elements = arr;


            return carousel;
        }
        public static CarouselElement CarouselElem(string Title, string Description,string ID ="ID")
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", ID, Positive, "text");
            var element = new CarouselElement();
            element.Title = Title;
            element.Description = Description;
            element.Buttons = button.Build().Buttons.First();
            return element;

        }

        /// <summary>
        /// "Фильмы"->"Планирую посмотреть"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard FilmPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "payload", Primary, "text");


            button.SetInline();
            return button.Build();
        }
        /// <summary>
        ///  "Фильмы"->"Рандомный фильм"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomFilm()
        {
            return FilmSearch();
        }



        #endregion


        //Сериалы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Сериалы" (Build MessageKeyboard for button "Сериалы")
        /// </summary>
        /// <button></button>
        public static MessageKeyboard TV()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Поиск по названию", "", Primary, "text");
            button.AddLine();

            button.AddButton("Мои рекомендации", "", Primary, "text");
            button.AddLine();

            button.AddButton("Планирую посмотреть", "", Primary, "text");
            button.AddLine();

            button.AddButton("Рандомный сериал", "", Primary, "text");
            button.AddLine();

            button.AddButton("Назад", "", Default, "text");

            return button.Build();
        }

        #region TV

        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Сериалы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>     
        public static MessageKeyboard TVSearch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", "TVID", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", "TVID", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", "TVID", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", "TVID", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", "TVID", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        //"Сериалы"->"Мои рекомендации"
        public static MessageTemplate TVMyRecomenation()
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            carousel.Elements = new List<CarouselElement>();
            for (var i = 0; i < 5; i++)
            {
                carousel.Elements.Append(CarouselElem("Название Сериала", "Жанры"));
            }



            return carousel;
        }

        /// <summary>
        /// "Сериалы"->"Планирую посмотреть"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "TVID", Primary, "text");


            button.SetInline();
            return button.Build();
        }

        /// <summary>
        ///  "Сериалы"->"Рандомный сериал"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomTV()
        {
            return TVSearch();
        }

        #endregion


        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard FilmWatched()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", "filmID", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", "filmID", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVWatched()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", "TVID", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", "TVID", Negative, "text");

            button.SetInline();
            return button.Build();
        }

    }
}
