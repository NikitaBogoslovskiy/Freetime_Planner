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
        public static string users_path;

        /// <summary>
        /// Словарь пользователей, где ключ - это ID, а значение - объект класса User
        /// </summary>
        public static Dictionary<long, User> Users_Dict;

        public static bool FileIsUsed = false;

        /// <summary>
        /// Загружает json-файл со списком известных на данный момент пользователей по пути, указанному в поле users_path
        /// </summary>
        public static void Upload()
        {
            while (true)
            {
                if (FileIsUsed)
                    continue;
                FileIsUsed = true;
                var dict = JsonConvert.DeserializeObject<Dictionary<long, User>>(File.ReadAllText(users_path));
                FileIsUsed = false;
                Users_Dict = dict ?? new Dictionary<long, User>();
                break;
            }
            foreach (var user in Users_Dict.Values)
            {
                user.Level.RemoveFirst();
                user.RandomFilmsIsUpdating = false;
                user.RandomTVIsUpdating = false;
            }//костыль: при десериализации возникает лишний первый уровень Default   
        }

        /// <summary>
        /// Выгружает актуальный список пользователей в json-файл по пути, который указан в поле users_path
        /// </summary>
        public static void Unload()
        {
            try
            {
                while (true)
                {
                    if (FileIsUsed)
                        continue;
                    FileIsUsed = true;
                    File.WriteAllText(users_path, JsonConvert.SerializeObject(Users_Dict));
                    FileIsUsed = false;
                    return;
                }
            }
            catch(Exception) { }
        }

        /// <summary>
        /// Получение объекта нашего класса User из объекта класса User пространства имен VkNet
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns> 
        public static User GetUser(VkNet.Model.User sender, out bool IsOld)
        {
            if (IsOld = Users_Dict.TryGetValue(sender.Id, out User user))
                return user;
            else
            {
                var new_user = new User(sender.FirstName, sender.LastName, sender.Id);
                Users_Dict.Add(sender.Id, new_user);
                Unload();
                return new_user;
            }
        }
    }
}
