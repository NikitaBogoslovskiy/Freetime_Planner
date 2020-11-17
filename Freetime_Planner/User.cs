using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.Modes;
using Newtonsoft.Json;

namespace Freetime_Planner
{
    public class User
    {
        /// <summary>
        /// Имя пользовтеля
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; private set; }

        /// <summary>
        /// ID пользователя
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Уровень, на котором находится пользователь
        /// </summary>
        public LinkedList<Mode> Level { get; private set; }

        /// <summary>
        /// Последнее время, когда пользователь находился на каком-то нетривиальном уровне (т.е. не Default)
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// Конструктор пользователя
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="id"></param>
        public User(string name, string surname, long id)
        {
            Name = name;
            Surname = surname;
            ID = id;
            ResetLevel();
        }

        /// <summary>
        /// Функция, добавляющая новый уровень
        /// </summary>
        /// <param name="m"></param>
        public void AddLevel(Mode m)
        {
            Level.AddLast(m);
            LastTime = DateTime.Now;
            Users.Unload();
        }

        /// <summary>
        /// Функция, удаляющая последний уровень
        /// </summary>
        public void RemoveLevel()
        {
            if (Level.Last.Value == Mode.Default)
                return;
            Level.RemoveLast();
            if (CurrentLevel() != Mode.Default)
                LastTime = DateTime.Now;
            Users.Unload();
        }

        /// <summary>
        /// Возвращает текущий уровень
        /// </summary>
        /// <returns></returns>
        public Mode CurrentLevel() => Level.Last.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Mode PreviousLevel() => Level.Last.Previous.Value;

        /// <summary>
        /// Устанавливает первый уровень по умолчанию Default
        /// </summary>
        public void ResetLevel()
        {
            Level = new LinkedList<Mode>();
            Level.AddLast(Mode.Default);
            Users.Unload();
            //Bot.keyboard = Keyboards.Mainmenu();
            //Bot.SendMessage("Жми любую кнопку");
        }
    }

    public class Payload
    {
        public string text { get; set; }

        public static string PayloadValue(string payload) => payload == null ? "" : JsonConvert.DeserializeObject<Payload>(payload).text;
    }
}
