using System;
using System.Collections.Generic;
using System.Text;
using static Freetime_Planner.User;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Freetime_Planner
{
    public static class Users
    {
        /// <summary>
        /// Путь к json-файлу со списком пользователей
        /// </summary>
        private static string users_path = @"Users_Dict\Users_Dict.json";

        /// <summary>
        /// Словарь пользователей, где ключ - это ID, а значение - объект класса User
        /// </summary>
        public static Dictionary<long, User> Users_Dict { get; set; }

        /// <summary>
        /// Загружает json-файл со списком известных на данный момент пользователей по пути, указанному в поле users_path
        /// </summary>
        public static void Upload()
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<long, User>>(File.ReadAllText(users_path));
            Users_Dict = dict ?? new Dictionary<long, User>();
        }

        /// <summary>
        /// Выгружает актуальный список пользователей в json-файл по пути, который указан в поле users_path
        /// </summary>
        public static void Unload()
        {
            File.WriteAllText(users_path, JsonConvert.SerializeObject(Users_Dict));
        }

        /// <summary>
        /// Получение объекта нашего класса User из объекта класса User пространства имен VkNet
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static User GetUser(VkNet.Model.User sender, ref bool IsNew)
        {
            if (Users_Dict.Select(user => user.Key).Contains(sender.Id))
            {
                IsNew = false;
                return Users_Dict[sender.Id];
            }
            else
            {
                IsNew = true;
                var new_user = new User(sender.FirstName, sender.LastName, sender.Id);
                Users_Dict.Add(sender.Id, new_user);
                Unload();
                Bot.keyboard = Keyboards.Mainmenu();
                Bot.SendMessage("Привет! Жми кнопку из меню");
                return new_user;
            }
        }
    }
}
