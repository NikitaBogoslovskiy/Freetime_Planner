using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Model.Attachments;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Freetime_Planner
{
    public static class Food
    {
        public static string[] Snacks = new string[] {"бутерброды", "сэндвич рецепт", "гренки", "тарталетки с начинкой", "сырные палочки", "луковые кольца", "закуски с яйцом",
            "закуски с лососем", "кальмары рецепт", "наггетсы", "мини-пицца"};
        public static string[] Cocktails = new string[] {"с мороженым", "клубничный", "молочный", "банановый", "ванильный", "кофейный безалкогольный",
            "мятный безалкогольный", "сливочный безалкогольный"};
        public static string[] Desserts = new string[] {"шоколадный десерт", "фруктовый десерт", "молочный десерт", "мармеладный десерт", 
            "десерт с мороженым", "десерт с яблоком", "ванильный десерт", "десерт пудинг рецепт", "десерт нутелла рецепт", "шоколадный брауни рецепт"};

        public static string[] HealthySnacks = new string[] {"Тост с фруктами и сливочным сыром", "Цветная капуста с медовым соусом", "Помидоры с сыром",
            "Запеченная морковь", "Полезный сэндвич" };
        public static string[] HealthyCocktails = new string[] {"Овсяноблин", "Фруктовое мороженое"," Фитнес кекс","Яблочные чипсы",
            "Диетическое овсяное печенье","Полезное цельнозерновое печенье" };
        public static string[] HealthyDesserts = new string[] {"Cмузи из банана и шпината","смузи из сельдерея","смузи из замороженных ягод",
            "жиросжигающий смузи","смузи из свеклы" };

        public static Dictionary<string, string[]> GenreFood = new Dictionary<string, string[]>();
        public static string GenreFoodPath;
        public static Random r = new Random();
        public static WebClient wc = new WebClient();

        public static Video Snack(User user)
        {
            int ind;
            do { ind = r.Next(0, Snacks.Length - 1); } while (ind == user.LastFood["Snack"]);
            user.LastFood["Snack"] = ind;
            var client = new RestSharp.RestClient("https://www.googleapis.com/youtube/v3/search");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("key", Bot._youtube_key);
            request.AddQueryParameter("part", "snippet");
            request.AddQueryParameter("q", Snacks[ind]);
            request.AddQueryParameter("videoDuration", "short");
            request.AddQueryParameter("type", "video");
            IRestResponse response = client.Execute(request);
            YouTube.YouTubeResults results;
            try { results = JsonConvert.DeserializeObject<YouTube.YouTubeResults>(response.Content); }
            catch(Exception) { return null; }
            if (results == null || results.items.Count == 0)
                return null;

            var video = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
            {
                Link = $"https://www.youtube.com/watch?v={results.items[r.Next(0, 5)].id.videoId}"
            });
            wc.DownloadString(video.UploadUrl);
            return video;
        }

        public static Video Cocktail(User user)
        {
            int ind;
            do { ind = r.Next(0, Cocktails.Length - 1); } while (ind == user.LastFood["Cocktail"]);
            user.LastFood["Cocktail"] = ind;
            var client = new RestSharp.RestClient("https://www.googleapis.com/youtube/v3/search");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("key", Bot._youtube_key);
            request.AddQueryParameter("part", "snippet");
            request.AddQueryParameter("q", "коктейль " + Cocktails[ind]);
            request.AddQueryParameter("videoDuration", "short");
            request.AddQueryParameter("type", "video");
            IRestResponse response = client.Execute(request);
            YouTube.YouTubeResults results;
            try { results = JsonConvert.DeserializeObject<YouTube.YouTubeResults>(response.Content); }
            catch (Exception) { return null; }
            if (results == null || results.items.Count == 0)
                return null;

            var video = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
            {
                Link = $"https://www.youtube.com/watch?v={results.items[r.Next(0, 5)].id.videoId}"
            });
            wc.DownloadString(video.UploadUrl);
            return video;
        }

        public static Video Dessert(User user)
        {
            int ind;
            do { ind = r.Next(0, Desserts.Length - 1); } while (ind == user.LastFood["Dessert"]);
            user.LastFood["Dessert"] = ind;
            var client = new RestSharp.RestClient("https://www.googleapis.com/youtube/v3/search");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("key", Bot._youtube_key);
            request.AddQueryParameter("part", "snippet");
            request.AddQueryParameter("q", Desserts[ind]);
            request.AddQueryParameter("videoDuration", "short");
            request.AddQueryParameter("type", "video");
            IRestResponse response = client.Execute(request);
            YouTube.YouTubeResults results;
            try { results = JsonConvert.DeserializeObject<YouTube.YouTubeResults>(response.Content); }
            catch (Exception) { return null; }
            if (results == null || results.items.Count == 0)
                return null;

            var video = private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
            {
                Link = $"https://www.youtube.com/watch?v={results.items[r.Next(0, 5)].id.videoId}"
            });
            wc.DownloadString(video.UploadUrl);
            return video;
        }

        public static void UploadGenreFood()
        {
            foreach(var line in File.ReadLines(GenreFoodPath))
            {
                var pair = line.Split("*");
                GenreFood[pair[0]] = pair[1].Split(",").Select(g => g.Trim()).ToArray();
            }
        }

        /*
        private static void UnloadFood()
        {
            var l1 = new List<string>();
            l1.Add("снеки: ");
            l1.AddRange(Snacks.Select(g => $"{g}, "));
            var l2 = new List<string>();
            l2.Add("коктейли: ");
            l2.AddRange(Cocktails.Select(c => $"коктейль {c}, "));
            var l3 = new List<string>();
            l3.Add("десерты: ");
            l3.AddRange(Desserts.Select(d => $"{d}, "));
            File.WriteAllLines("FoodTypes.txt", new List<string> { string.Join("", l1), string.Join("", l2), string.Join("", l3) });
        }*/
    }
}
