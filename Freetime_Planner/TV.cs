using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.Template;
using static Freetime_Planner.Bot;
using static Freetime_Planner.Modes.Mode;

namespace Freetime_Planner
{
    public static class TV
    {
        public class Country
        {
            public string country { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class Episode
        {
            public int seasonNumber { get; set; }
            public int episodeNumber { get; set; }
            public object nameRu { get; set; }
            public string nameEn { get; set; }
            public object synopsis { get; set; }
            public string releaseDate { get; set; }
        }

        public class Season
        {
            public int number { get; set; }
            public List<Episode> episodes { get; set; }
        }

        public class Data
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string webUrl { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
            public string year { get; set; }
            public string filmLength { get; set; }
            public object slogan { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public object ratingMpaa { get; set; }
            public object ratingAgeLimits { get; set; }
            public object premiereRu { get; set; }
            public object distributors { get; set; }
            public string premiereWorld { get; set; }
            public object premiereDigital { get; set; }
            public string premiereWorldCountry { get; set; }
            public object premiereDvd { get; set; }
            public object premiereBluRay { get; set; }
            public object distributorRelease { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public List<string> facts { get; set; }
            public List<Season> seasons { get; set; }
            public string VKPhotoID { get; set; }
        }

        public class ExternalId
        {
            public string imdbId { get; set; }
        }

        public class Budget
        {
            public object grossRu { get; set; }
            public object grossUsa { get; set; }
            public object grossWorld { get; set; }
            public object budget { get; set; }
            public object marketing { get; set; }
        }

        public class TVObject
        {
            public Data data { get; set; }
            public ExternalId externalId { get; set; }
            public Budget budget { get; set; }
            public int Priority { get; set; }
        }

        public static class Methods
        {
            public static string FullInfo(int TVID)
            {
                return null;
            }
            public static string FullInfo(TV.TVObject tv)
            {
                return null;
            }

            public static MessageTemplate Search(string TVName)
            {
                return null;
            }

            public static MessageTemplate Random()
            {
                return null;
            }

            public static List<Audio> Soundtrack(string TVName)
            {
                return null;
            }

            public static Video Food(string[] genres)
            {
                return null;
            }

            public static MessageKeyboard ServiceLinks(string TVName, string year)
            {
                return null;
            }
        }
    }



    public static class TVResults
    {
        public class Country
        {
            public string country { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class Film
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string nameEn { get; set; }
            public string type { get; set; }
            public string year { get; set; }
            public string description { get; set; }
            public string filmLength { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public string rating { get; set; }
            public int ratingVoteCount { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
        }

        public class Results
        {
            public string keyword { get; set; }
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
            public int searchFilmsCountResult { get; set; }
        }
    }




    public static class RandomTV
    {
        public class Country
        {
            public string country { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class Film
        {
            public int filmId { get; set; }
            public string nameRu { get; set; }
            public string type { get; set; }
            public string year { get; set; }
            public List<Country> countries { get; set; }
            public List<Genre> genres { get; set; }
            public string rating { get; set; }
            public int ratingVoteCount { get; set; }
            public string posterUrl { get; set; }
            public string posterUrlPreview { get; set; }
            public string nameEn { get; set; }
        }

        public class Results
        {
            public int pagesCount { get; set; }
            public List<Film> films { get; set; }
        }
    }
}