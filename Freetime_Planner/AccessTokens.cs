using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Freetime_Planner
{
    public class AccessTokens
    {
        public string _access_token { get; set; }
        public string _private_access_token { get; set; }
        public string _mdb_key { get; set; }
        public string _kp_key { get; set; }
        public string _yandex_login { get; set; }
        public string _yandex_password { get; set; }
        public string _vk_login { get; set; }
        public string _vk_password { get; set; }
        public string _youtube_key { get; set; }
        public string _google_key { get; set; }
        public string _google_sid { get; set; } 
        public string _google_sid_series { get; set; }

        public static void Upload()
        {
            var directory = GetCurrentDirectory();
            var token_object = JsonConvert.DeserializeObject<AccessTokens>(File.ReadAllText(directory + "/Access_Tokens.json"));
            Bot._access_token = token_object._access_token;
            Bot._private_access_token = token_object._private_access_token;
            Bot._mdb_key = token_object._mdb_key;
            Bot._kp_key = token_object._kp_key;
            Bot._yandex_login = token_object._yandex_login;
            Bot._yandex_password = token_object._yandex_password;
            Bot._vk_login = token_object._vk_login;
            Bot._vk_password = token_object._vk_password;
            Bot._youtube_key = token_object._youtube_key;
            Bot._google_key = token_object._google_key;
            Bot._google_sid = token_object._google_sid;
            Bot._google_sid_series = token_object._google_sid_series;
            Users.users_path = directory + "/Users_Dict.json";
            Film.PopularFilmsPath = directory + "/PopularFilms.json";
            TV.PopularTVPath = directory + "/PopularTV.json";
            ServiceClass.service_path = directory + "/ServiceData.json";
            Food.GenreFoodPath = directory + "/GenreFood.txt";
            Attachments.DefaultPosterPath = directory + "/DefaultPoster.jpg";
        }

        public static string GetCurrentDirectory()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(exePath).Replace("\\", "/");
        }
    }

    public static class ServiceClass
    {
        public static ServiceData service_data;
        public static string service_path;

        public static void UploadServiceData()
        {
            if (!File.Exists(service_path))
            {
                service_data = new ServiceData();
                File.WriteAllText(service_path, JsonConvert.SerializeObject(service_data));
            }
            else
                service_data = JsonConvert.DeserializeObject<ServiceData>(File.ReadAllText(service_path));
        }
        public static void UnloadServiceData() => File.WriteAllText(service_path, JsonConvert.SerializeObject(service_data));
    }

    public class ServiceData
    {
        public int google_requests;
        public DateTime last_update;
        public ServiceData()
        {
            google_requests = 0;
            last_update = DateTime.Now;
        }
        public void ResetGoogleRequests()
        {
            google_requests = 0;
            last_update = DateTime.Now;
            ServiceClass.UnloadServiceData();
        }
        public void IncGoogleRequests()
        {
            google_requests++;
            ServiceClass.UnloadServiceData();
        }
    }
}
