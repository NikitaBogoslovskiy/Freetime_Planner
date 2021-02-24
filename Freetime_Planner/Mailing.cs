using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VkNet.Model.Attachments;

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
            public List<string> Facts { get; set; }
            public List<Audio> SoundTrack { get; set; }
            public bool IsValid = false;
            public MailObject(string filmID)
            {
                var client1 = new RestClient("https://kinopoiskapiunofficial.tech/api/v2.1/films/" + filmID);
                var request1 = new RestRequest(Method.GET);
                request1.AddHeader("X-API-KEY", Bot._kp_key);
                IRestResponse response1 = client1.Execute(request1);
                var film = JsonConvert.DeserializeObject<Film.FilmObject>(response1.Content);
                if (film != null)
                {
                    var name = film.data.nameRu ?? film.data.nameEn;
                    var year = film.data.year;

                    var client2 = new RestClient($"https://kinopoiskapiunofficial.tech/api/v2.1/films/{filmID}/frames");
                    var request2 = new RestRequest(Method.GET);
                    request2.AddHeader("X-API-KEY", Bot._kp_key);
                    IRestResponse response2 = client2.Execute(request2);
                    var frames = JsonConvert.DeserializeObject<Frames>(response2.Content);
                    if (frames != null && frames.frames != null && frames.frames.Count != 0)
                    {
                        var posters = new List<Photo>();
                        IEnumerable<string> links = frames.frames.Select(f => f.image).Shuffle().Take(Math.Min(5, frames.frames.Count));
                        foreach (var link in links)
                            posters.Add(Attachments.PosterObject(link, "0"));
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
                        Film.Methods.Soundtrack(film.data.nameEn ?? film.data.nameRu, year, soundtrack, 3);
                    else if (film.data.type == "TV_SHOW")
                        TV.Methods.Soundtrack(film.data.nameEn ?? film.data.nameRu, soundtrack, 3);
                    id = filmID;
                    Name = name;
                    Year = year;
                    SoundTrack = soundtrack;
                }
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
}
