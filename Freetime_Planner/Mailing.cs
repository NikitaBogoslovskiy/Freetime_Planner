﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VkNet.Model.Attachments;
using System.Net;

namespace Freetime_Planner
{
    public static class Mailing
    {
        public class MailObject
        {
            public string id { get; set; }
            public string Name { get; set; }
            public string Year { get; set; }
            public List<Photo> Posters { get; set; }
            public List<string> PostersIds { get; set; }
            public List<string> Facts { get; set; }
            public List<Audio> SoundTrack { get; set; }
            public Video Trailer { get; set; }
            public bool IsValid = false;
            public bool IsTrailer { get; set; }
            public MailObject() { }

            public void createPostersFacts(User user, string filmID)
            {
                IsTrailer = false;
                var client1 = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/" + filmID);
                var request1 = new RestRequest(Method.GET);
                request1.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response1 = client1.Execute(request1);
                Film.FilmObject film;
                try { film = JsonConvert.DeserializeObject<Film.FilmObject>(response1.Content); }
                catch(Exception) { film = null; }
                if (film != null)
                {
                    var name = film.data.nameRu ?? film.data.nameEn;
                    var year = film.data.year;

                    var client2 = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmID}/frames");
                    var request2 = new RestRequest(Method.GET);
                    request2.AddHeader("X-API-KEY", Bot._kp_key);
                    IRestResponse response2 = client2.Execute(request2);
                    Frames frames;
                    try { frames = JsonConvert.DeserializeObject<Frames>(response2.Content); }
                    catch(Exception) { frames = null; }
                    if (frames != null && frames.frames != null && frames.frames.Count != 0)
                    {
                        var posters = new List<Photo>();
                        IEnumerable<string> links = frames.frames.Select(f => f.image).Shuffle().Take(Math.Min(5, frames.frames.Count));
                        foreach (var link in links)
                        {
                            if (Attachments.PosterObject(user, link, filmID, out Photo photo))
                                posters.Add(photo);
                        }
                        if (posters.Count != 0)
                        {
                            Posters = posters;
                            IsValid = true;
                        }
                        else return;
                    }
                    else return;

                    if (film.data.facts.Count != 0)
                        Facts = film.data.facts.Shuffle().Take(Math.Min(3, film.data.facts.Count)).ToList();

                    var soundtrack = new List<Audio>();
                    if (film.data.type == "FILM")
                    {
                        string fname, addition;
                        if (film.data.nameEn != null && film.data.nameEn != string.Empty)
                        {
                            fname = film.data.nameEn;
                            addition = "ost";
                        }
                        else
                        {
                            fname = film.data.nameRu;
                            addition = "саундтрек";
                        }
                        //addition = year;
                        Film.Methods.DownloadSoundtrack(fname, addition, soundtrack, 2);
                    }
                    else if (film.data.type == "TV_SHOW")
                    {
                        string fname, addition;
                        if (film.data.nameEn != null && film.data.nameEn != string.Empty)
                        {
                            fname = film.data.nameEn;
                            addition = "ost";
                        }
                        else
                        {
                            fname = film.data.nameRu;
                            addition = "саундтрек";
                        }
                        //addition = year;
                        TV.Methods.DownloadSoundtrack(fname, addition, soundtrack, 2);
                    }
                    id = filmID;
                    Name = name;
                    Year = year;
                    SoundTrack = soundtrack;
                }
            }

            public void createPostersFacts(Film.FilmObject film)
            {
                IsTrailer = false;
                var name = film.data.nameRu ?? film.data.nameEn;
                var year = film.data.year;

                var client2 = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{film.data.filmId}/frames");
                var request2 = new RestRequest(Method.GET);
                request2.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response2 = client2.Execute(request2);
                Frames frames;
                try { frames = JsonConvert.DeserializeObject<Frames>(response2.Content); }
                catch (Exception) { frames = null; }
                if (frames != null && frames.frames != null && frames.frames.Count != 0)
                {
                    var posters = new List<string>();
                    IEnumerable<string> links = frames.frames.Select(f => f.image).Shuffle().Take(Math.Min(5, frames.frames.Count));
                    foreach (var link in links)
                    {

                        var id = Attachments.PosterObject(link, film.data.filmId.ToString());
                        if (id != null)
                            posters.Add(id);
                    }
                    if (posters.Count != 0)
                    {
                        PostersIds = posters;
                        IsValid = true;
                    }
                    else return;
                }
                else return;

                if (film.data.facts.Count != 0)
                    Facts = film.data.facts.Shuffle().Take(Math.Min(3, film.data.facts.Count)).ToList();

                /*var soundtrack = new List<Audio>();
                if (film.data.type == "FILM")
                {
                    string fname, addition;
                    if (film.data.nameEn != null)
                    {
                        fname = film.data.nameEn;
                        //addition = "ost";
                    }
                    else
                    {
                        fname = film.data.nameRu;
                        //addition = "саундтрек";
                    }
                    addition = year;
                    Film.Methods.DownloadSoundtrack(fname, addition, soundtrack, 2);
                }
                else if (film.data.type == "TV_SHOW")
                {
                    string fname, addition;
                    if (film.data.nameEn != null)
                    {
                        fname = film.data.nameEn;
                        addition = "series";
                    }
                    else
                    {
                        fname = film.data.nameRu;
                        addition = "сериал";
                    }
                    TV.Methods.DownloadSoundtrack(fname, addition, soundtrack, 2);
                }*/
                id = film.data.filmId.ToString();
                Name = name;
                Year = year;
                SoundTrack = null;
            }

            public void createTrailer(string filmid, string ruName, string engName, string year)
            {
                IsTrailer = true;
                var wc = new WebClient();
                var client = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmid}/videos");
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response = client.Execute(request);
                Trailer trailer;
                try { trailer = JsonConvert.DeserializeObject<MovieVideos>(response.Content).trailers.Where(t => t.site.ToLower() == "youtube").FirstOrDefault(); }
                catch (Exception)
                {
                    IsValid = false;
                    return;
                }
                if (trailer == null)
                {
                    IsValid = false;
                    return;
                }
                Trailer = Bot.private_vkapi.Video.Save(new VkNet.Model.RequestParams.VideoSaveParams
                {
                    Link = trailer.url
                });
                wc.DownloadString(Trailer.UploadUrl);
                IsValid = true;
                id = filmid;
                Name = ruName ?? engName;
                Year = year;
            }
        }

        public class Frame
        {
            public string image { get; set; }
            public string preview { get; set; }
        }

        public class Frames
        {
            public List<Frame> frames { get; set; }
        }
    }

    public class Trailer
    {
        public string url { get; set; }
        public string name { get; set; }
        public string site { get; set; }
        public object size { get; set; }
        public string type { get; set; }
    }

    public class MovieVideos
    {
        public List<Trailer> trailers { get; set; }
        public List<object> teasers { get; set; }
    }
}
