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
using static Freetime_Planner.Film;
using static Freetime_Planner.TV;
using System.Threading;
using System.Threading.Tasks;

namespace Freetime_Planner
{
    public static class Keyboards
    {
        public static MessageKeyboard MainKeyboard { get; private set; }
        public static MessageKeyboard FilmKeyboard { get; private set; }
        public static MessageKeyboard TVKeyboard { get; private set; }
        public static MessageKeyboard FoodKeyboard { get; private set; }

        /// <summary>
        /// Клавиатура главного меню
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard MainMenu()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Фильмы", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Сериалы", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Еда под просмотр", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddLine();


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

            button.AddButton("Закуски", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Сладкое", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Коктейли", "Command", Primary, "text");
            button.AddLine();
            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Назад", "Command", Negative, "text");

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
            result.AddButton("Поиск по названию", "Command", Primary, "text");
            result.AddLine();
            //"Мои рекомендации"
            result.AddButton("Мои рекомендации", "Command", Primary, "text");
            result.AddLine();
            //"Планирую посмотреть"
            result.AddButton("Планирую посмотреть", "Command", Primary, "text");
            result.AddLine();
            //"Рандомный фильм"
            result.AddButton("Рандомный фильм", "Command", Primary, "text");
            result.AddLine();
            //"Назад"
            result.AddButton("Помощь", "Command", Positive, "text");
            result.AddButton("Назад", "Command", Negative, "text");

            return result.Build();
 
        }

        #region Film
        /// <summary>
        /// Создаёт клавиатуру в сообщении для кнопоки "Фильмы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>
        public static MessageKeyboard FilmSearch(string nameRu, string nameEn, string filmID, string date, string genres)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", $"{nameRu};{nameEn};{filmID};{date};{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", $"{nameRu};{nameEn};{filmID};{date};{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", $"{nameRu};{nameEn};{filmID};{date};{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", $"{nameRu};{nameEn};{filmID};{date};{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", $"{nameRu};{nameEn};{filmID};{date};{genres}", Negative, "text");

            button.SetInline();
            return button.Build();
        }
      
        //"Фильмы"->"Мои рекомендации"

        /// <summary>
        /// Возвращает карусель из нескольких фильмов из списка рекомендаций
        /// </summary>
        /// <param name="farray"></param>
        /// <returns></returns>
        public static MessageTemplate FilmMyRecommendations(IEnumerable<Film.FilmObject> farray)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(farray, (film) =>
            {
                arr.Add(CarouselFilm(film));
            });
            carousel.Elements = arr;

            return carousel;
        }

        /// <summary>
        /// Возвращает один элемент карусели фильмов-рекомендаций
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static CarouselElement CarouselFilm(Film.FilmObject film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.data.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{film.data.nameRu};{film.data.nameEn};{film.data.filmId};{film.data.premiereRu ?? film.data.premiereWorld};{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.data.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = film.data.VKPhotoID;
            return element;

        }

        /// <summary>
        /// Возвращает результаты поиска фильмов по названию в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate FilmResults(FilmResults.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(results.films, (film) =>
            {
                arr.Add(CarouselFilmResult(film));
            });
            carousel.Elements = arr;

            return carousel;
        }
        /// <summary>
        /// Возвращает один элемент карусели из результатов поиска фильма по названию
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static CarouselElement CarouselFilmResult(FilmResults.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{film.nameRu};{film.nameEn};{film.filmId};{film.year};{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = null;//нужно загрузить фотографию на сервер
            return element;

        }

        /// <summary>
        /// Возвращает результаты случайного поиска фильмов в виде карусели
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static MessageTemplate RandomFilmResults(RandomFilms.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(results.films, (film) =>
            {
                arr.Add(CarouselRandomFilmResult(film));
            });
            carousel.Elements = arr;

            return carousel;
        }
        /// <summary>
        /// Возвращает один элемент карусели рандомных фильмов
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static CarouselElement CarouselRandomFilmResult(RandomFilms.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{film.nameRu};{film.nameEn};{film.filmId};{film.year};{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = Attachments.RandomFilmPosterID(film);
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
            button.AddButton("Уже посмотрел", "Inline", Primary, "text");
            button.SetInline();
            return button.Build();
        }
        /// <summary>
        ///  "Фильмы"->"Рандомный фильм"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomFilm(string nameRu, string nameEn, string filmID, string date, string genres)
        {
            return FilmSearch(nameRu, nameEn, filmID, date, genres);
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

            button.AddButton("Поиск по названию", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Мои рекомендации", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Планирую посмотреть", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Рандомный сериал", "Command", Primary, "text");
            button.AddLine();

            button.AddButton("Помощь", "Command", Positive, "text");
            button.AddButton("Назад", "Command", Negative, "text");

            return button.Build();
        }

        #region TV

        /// <summary>
        ///Создаёт клавиатуру в сообщении для кнопоки "Сериалы"->"Поиск по названию"  
        /// </summary>
        /// <button></button>     
        public static MessageKeyboard TVSearch(string nameRu, string nameEn, string filmID, string genres)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);

            button.Clear();

            button.AddButton("Хочу посмотреть", $"{nameRu};{nameEn};{filmID};;{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Посмотрел", $"{nameRu};{nameEn};{filmID};;{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Саундтрек", $"{nameRu};{nameEn};{filmID};;{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Что поесть", $"{nameRu};{nameEn};{filmID};;{genres}", Primary, "text");
            button.AddLine();
            button.AddButton("Не показывать", $"{nameRu};{nameEn};{filmID};;{genres}", Negative, "text");

            button.SetInline();
            return button.Build();
        }

       

        //"Сериалы"->"Мои рекомендации"
        public static MessageTemplate TVMyRecommendations(TVObject[] tvs)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(tvs, (tv) =>
            {
                arr.Add(CarouselTV(tv));
            });
            carousel.Elements = arr;
            return carousel;
        }
        public static CarouselElement CarouselTV(TVObject tv)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', tv.data.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{tv.data.nameRu};{tv.data.nameEn};{tv.data.filmId};;{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = tv.data.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = tv.data.VKPhotoID;
            return element;
        }

        public static MessageTemplate TVResults(TVResults.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(results.films, (film) =>
            {
                arr.Add(CarouselTVResult(film));
            });
            carousel.Elements = arr;

            return carousel;
        }
        public static CarouselElement CarouselTVResult(TVResults.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{film.nameRu};{film.nameEn};{film.filmId};;{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = null;//нужно загрузить фотографию на сервер
            return element;

        }

        public static MessageTemplate RandomTVResults(RandomTV.Results results)
        {
            var carousel = new MessageTemplate();
            carousel.Type = Carousel;
            var arr = new List<CarouselElement>();
            Parallel.ForEach(results.films, (film) =>
            {
                arr.Add(CarouselRandomTVResult(film));
            });
            carousel.Elements = arr;

            return carousel;
        }
        public static CarouselElement CarouselRandomTVResult(RandomTV.Film film)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            var genres = string.Join('*', film.genres.Select(g => g.genre));
            button.AddButton("Подробнее", $"{film.nameRu};{film.nameEn};{film.filmId};;{genres}", Positive, "text");
            var element = new CarouselElement();
            element.Title = film.nameRu;
            element.Description = genres.Replace("*", ", ");
            element.Buttons = button.Build().Buttons.First();
            element.PhotoId = null;//нужно загрузить фотографию на сервер
            return element;

        }
        /// <summary>
        /// "Сериалы"->"Планирую посмотреть"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVPlanToWatch()
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Уже посмотрел", "Inline", Primary, "text");


            button.SetInline();
            return button.Build();
        }

        /// <summary>
        ///  "Сериалы"->"Рандомный сериал"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard RandomTV(string nameRu, string nameEn, string filmID, string genres)
        {
            return TVSearch(nameRu, nameEn, filmID, genres);
        }

        #endregion


        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard FilmWatched(string nameRu, string nameEn, string filmID, string date, string genres)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", $"{nameRu};{nameEn};{filmID};{date};{genres}", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", $"{nameRu};{nameEn};{filmID};{date};{genres}", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        /// <summary>
        /// Клавиатура в сообщении для кнопки "Просмотрел"
        /// </summary>
        /// <returns></returns>
        public static MessageKeyboard TVWatched(string nameRu, string nameEn, string filmID, string genres)
        {
            var button = new VkNet.Model.Keyboard.KeyboardBuilder(false);
            button.Clear();

            button.AddButton("Да", $"{nameRu};{nameEn};{filmID};;{genres}", Positive, "text");
            //button.AddLine();
            button.AddButton("Нет", $"{nameRu};{nameEn};{filmID};;{genres}", Negative, "text");

            button.SetInline();
            return button.Build();
        }

        public static void Init()
        {
            MainKeyboard = MainMenu();
            FilmKeyboard = Film();
            TVKeyboard = TV();
            FoodKeyboard = Food();
        }

    }
}
