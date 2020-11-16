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
        static MessageKeyboard Mainmenu()
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
        static MessageKeyboard Food()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Закуски", "", Primary, "text");
            button.AddLine();
            button.AddButton("Сладкое", "", Primary, "text");
            button.AddLine();
            button.AddButton("Коктели", "", Primary, "text");
            button.AddLine();
            button.AddButton("Назад", "", Default, "text");

            return button.Build();
        }

        //Фильмы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Фильмы" (Build MessageKeyboard for button "Фильмы")
        /// </summary>
        /// <returns></returns>
        static MessageKeyboard Film()
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
        static MessageKeyboard FilmSearch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", "", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", "", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", "", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", "", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", "", Negative, "text");

            button.SetInline();
            return button.Build();
        }
      
        //"Фильмы"->"Мои рекомендации"
         static MessageTemplate FilmMyRecomenation()
        {
           var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            // carousel.Elements
            carousel.Elements = new List<CarouselElement>();
            for (var i=0;i<5;i++)
            {
                carousel.Elements.Append(CarouselElem("Название Фильма", "Жанры"));
            }



            return carousel;
        }
        static CarouselElement CarouselElem(string Title, string Description,string FilmID ="")
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.AddButton("Подробнее", FilmID, Positive, "text");
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
        static MessageKeyboard FilmPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "", Primary, "text");


            button.SetInline();
            return button.Build();
        }
        /// <summary>
        ///  "Фильмы"->"Рандомный фильм"
        /// </summary>
        /// <returns></returns>
        static MessageKeyboard RandomFilm()
        {
            return FilmSearch();
        }



        #endregion


        //Сериалы
        /// <summary>
        /// Создаёт Клавиатуру для кнопоки "Сериалы" (Build MessageKeyboard for button "Сериалы")
        /// </summary>
        /// <button></button>
        static MessageKeyboard TV()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Поиск по названию", "", Primary, "text");
            button.AddLine();

            button.AddButton("Мои рекомендации", "", Primary, "text");
            button.AddLine();

            button.AddButton("Планирую посмотреть", "", Primary, "text");
            button.AddLine();

            button.AddButton("Рандомный фильм", "", Primary, "text");
            button.AddLine();

            button.AddButton("Назад", "", Default, "text");

            return button.Build();
        }

        #region TV

        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Сериалы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>     
        static MessageKeyboard TVSearch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", "", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", "", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", "", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", "", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", "", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        //"Сериалы"->"Мои рекомендации"
        static MessageTemplate TVMyRecomenation()
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
        static MessageKeyboard TVPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "", Primary, "text");


            button.SetInline();
            return button.Build();
        }

        /// <summary>
        ///  "Сериалы"->"Рандомный сериал"
        /// </summary>
        /// <returns></returns>
        static MessageKeyboard RandomTV()
        {
            return TVSearch();
        }

        #endregion


        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        static MessageKeyboard Watched()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Понравилось", "", Primary, "text");
            //button.AddLine();
            button.AddButton("Не понравилось", "", Primary, "text");

            button.SetInline();
            return button.Build();
        }

       
    }
}
