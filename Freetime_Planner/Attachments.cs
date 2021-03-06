﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VkNet.Model.RequestParams;
using System.Linq;
using System.IO;
using VkNet.Model.Attachments;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Console;
using VkNet.Model.Template;
using VkNet.Model.Keyboard;

namespace Freetime_Planner
{

    public static class Attachments
    {
        public static string DefaultPosterPath { get; set; }

        //Вложения для фильмов
        #region FilmPosters
        /// <summary>
        /// Возвращает ID постера фильма из списка PopularFilms
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static string PopularFilmPosterID(Film.FilmObject film,out string photoID2)
        {
            try 
            { 
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", film.data.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.data.posterUrl, path);
                if (!SizeIsWell(path))
                {
                    photoID2 = null;
                    return null; 
                }

                //загрузка полноценных фотографий - не импотентов
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_popular, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_popular,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                // обрезание
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_popular, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_popular,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null;
                return null;
            }
        }

        public static string ResultedFilmPosterID(FilmResults.Film film)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", film.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.posterUrl, path);
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_results, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_results,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return null;
            }
        }

        public static string RandomFilmPosterID(RandomFilms.Film film,out string photoID2)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", film.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.posterUrl, path);
                if (!SizeIsWell(path))
                {
                    photoID2 = null;
                    return null; }
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_random, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_random,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                if (!CropAndOverwrite(path))
                { photoID2 = null;
                    return null; }
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_random, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_random,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null;
                return null;
            }
        }

        public static string RecommendedFilmPosterID(Film.FilmObject film, out string photoID2)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", film.data.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.data.posterUrl, path);
                if (!SizeIsWell(path))
                {
                    photoID2 = null;
                    return null;
                }
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_random, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_random,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_recommended, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_recommended,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null;
                return null;
            }
        }

        /*public static string RecommendedFilmPosterID(RandomFilms.Film film)
        {
            try
            {
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", film.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.posterUrl, path);
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_recommended, Bot.group_id);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_recommended,
                    GroupId = Bot.group_id
                }).First();
                var vkid = $"-{Bot.group_id}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return null;
            }
        }*/
        #endregion

        //Вложения для сериалов
        #region TVPosters

        public static string PopularTVPosterID(TV.TVObject tv, out string photoID2)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/tv_{0}_{1}.jpg", tv.data.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(tv.data.posterUrl, path);
                //необрезаные фото
                if (!SizeIsWell(path))
                {   photoID2 = null;
                    return null; }
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_popular_tv, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_popular_tv,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                //обрезаные фото
                if (!CropAndOverwrite(path))//
               
                { photoID2 = null;  return null; }
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_popular_tv, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));//
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams//
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_popular_tv,//
                    GroupId = Bot.group_id_service//
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null; return null;
            }
        }

        public static string ResultedTVPosterID(TVResults.Film film)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/tv_{0}_{1}.jpg", film.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.posterUrl, path);
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_results_tv, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_results_tv,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return null;
            }
        }

        public static string RandomTVPosterID(RandomTV.Film film, out string photoID2)
        {
            try
            { 
                string path = String.Format(Bot.directory + "/tv_{0}_{1}.jpg", film.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(film.posterUrl, path);
                //не обрез
                if (!SizeIsWell(path))
                { photoID2 = null; return null; }
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_random_tv, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_random_tv,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                //обрез
                if (!CropAndOverwrite(path))
                { photoID2 = null; return null; }
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_random_tv, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_random_tv,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null;
                return null;
            }
        }

        public static string RecommendedTVPosterID(TV.TVObject tv, out string photoID2)
        {
            try
            {
                string path = String.Format(Bot.directory + "/tv_{0}_{1}.jpg", tv.data.filmId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(tv.data.posterUrl, path);
                //загрузка необрезаного постера
                if (!SizeIsWell(path))
                {
                    photoID2 = null;
                    return null;
                }
                var uploadServer2 = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_recommended_tv, Bot.group_id_service);
                var responseFile2 = Encoding.ASCII.GetString(wc.UploadFile(uploadServer2.UploadUrl, path));
                var photo2 = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile2,
                    AlbumId = Bot.album_id_recommended_tv,
                    GroupId = Bot.group_id_service
                }).First();
                photoID2 = $"-{Bot.group_id_service}_{photo2.Id}";
                //обрезаный постер
                if (!CropAndOverwrite(path))
                {
                    photoID2 = null;
                    return null;
                }
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_recommended_tv, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_recommended_tv,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                photoID2 = null;
                return null;
            }
        }

        #endregion

        public static string ActorPosterID(ActorResults.Actor actor)
        {
            try
            {
                string path = String.Format(Bot.directory + "/tv_{0}_{1}.jpg", actor.staffId, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(actor.posterUrl, path);
                if (!CropAndOverwrite(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_actors, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_actors,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return null;
            }
        }

        public static Photo PosterObject(User user, string url, string filmID)
        {
            string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", filmID, Guid.NewGuid());
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(url, path);
                var uploadServer = Bot.vkapi_main.Photo.GetMessagesUploadServer(user.ID);
                var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.vkapi_main.Photo.SaveMessagesPhoto(result).First();
                File.Delete(path);
                return photo;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                var uploadServer = Bot.vkapi_main.Photo.GetMessagesUploadServer(user.ID);
                var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, DefaultPosterPath));
                var photo = Bot.vkapi_main.Photo.SaveMessagesPhoto(result).First();
                return photo;
            }
        }
        public static string PosterObject(string url, string filmID)
        {
            try
            {
                string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", filmID, Guid.NewGuid());
                WebClient wc = new WebClient();
                wc.DownloadFile(url, path);
                if (!SizeIsWell(path))
                    return null;
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id_mailing, Bot.group_id_service);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id_mailing,
                    GroupId = Bot.group_id_service
                }).First();
                var vkid = $"-{Bot.group_id_service}_{photo.Id}";
                File.Delete(path);
                return vkid;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return null;
            }
        }

        public static bool PosterObject(User user, string url, string filmID, out Photo photo)
        {
            string path = String.Format(Bot.directory + "/film_{0}_{1}.jpg", filmID, Guid.NewGuid());
            WebClient wc = new WebClient();
            photo = null;
            try
            {
                wc.DownloadFile(url, path);
                if (!SizeIsWell(path))
                {
                    File.Delete(path);
                    return false;
                }
                var uploadServer = Bot.vkapi_main.Photo.GetMessagesUploadServer(user.ID);
                var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                photo = Bot.vkapi_main.Photo.SaveMessagesPhoto(result).First();
                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                WriteLine($"Исключение: {e.Message}\nСтектрейс: {e.StackTrace}");
                return false;
            }
        }

        /*
        /// <summary>
        /// Возвращает ID постера фильма
        /// </summary>
        /// <param name="film"></param>
        /// <returns></returns>
        public static string FilmObjectPosterID(Film.FilmObject film)
        {
            string path = String.Format("film_{0}_{1}.jpg", film.data.filmId, Guid.NewGuid());
            WebClient wc = new WebClient();
            wc.DownloadFile(film.data.posterUrl, path);
            if (!CropAndOverwrite(path))
                return null;
            var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id, Bot.group_id);
            var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
            var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
            {
                SaveFileResponse = responseFile,
                AlbumId = Bot.album_id,
                GroupId = Bot.group_id
            }).First();
            var vkid = $"-{Bot.group_id}_{photo.Id}";
            File.Delete(path);
            return vkid;
        }
        */
        /*
        /// <summary>
        /// Возвращает ID ВК загруженного постера фильма
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static string PosterID(Film.FilmObject movie)
        {
            if (movie.V == null)
            {
                string path = $"film_{movie.ID}.jpg";
                WebClient wc = new WebClient();
                wc.DownloadFile(movie.ImageURL, path);
                CropAndOverwrite(path);
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id, Bot.group_id);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id,
                    GroupId = Bot.group_id
                }).First();
                movie.PhotoID = $"-{Bot.group_id}_{photo.Id}";
                File.Delete(path);
            }
            return movie.PhotoID;
        }
        */
        /*
        /// <summary>
        /// Возвращает ID ВК загруженного постера сериала
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static string PosterID(TV tv)
        {
            if (tv.PhotoID == null)
            {
                string path = $"tv_{tv.ID}.jpg";
                WebClient wc = new WebClient();
                wc.DownloadFile(tv.ImageURL, path);
                CropAndOverwrite(path);
                var uploadServer = Bot.private_vkapi.Photo.GetUploadServer(Bot.album_id, Bot.group_id);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photo = Bot.private_vkapi.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = Bot.album_id,
                    GroupId = Bot.group_id
                }).First();
                tv.PhotoID = $"-{Bot.group_id}_{photo.Id}";
                File.Delete(path);
            }
            return tv.PhotoID;
        }
        */
        /*
        /// <summary>
        /// Возвращает объект Photo, представляющий постер фильма
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static Photo PosterObject(Film movie)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(movie.ImageURL, $"poster_{movie.ID}1.jpg");
            var uploadServer = Bot.vkapi.Photo.GetMessagesUploadServer(Bot.user.ID);
            var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, $"poster_{movie.ID}1.jpg"));
            var photo = Bot.vkapi.Photo.SaveMessagesPhoto(result).First();
            File.Delete($"poster_{movie.ID}1.jpg");
            return photo;
        }
        */
        /*
        /// <summary>
        /// Возвращает объект Photo, представляющий постер сериала
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static Photo PosterObject(TV tv)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(tv.ImageURL, $"tv_{tv.ID}.jpg");
            var uploadServer = Bot.vkapi.Photo.GetMessagesUploadServer(Bot.user.ID);
            var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, $"tv_{tv.ID}.jpg"));
            var photo = Bot.vkapi.Photo.SaveMessagesPhoto(result).First();
            File.Delete($"tv_{tv.ID}.jpg");
            return photo;
        }
        */



        //--------------------------------------------------Приватные методы по обработке фотографий-----------------------------------------

        public static bool SizeIsWell(string path)
        {
            Bitmap pic = new Bitmap(path);
            bool res = pic.Width >= 221 && pic.Height >= 136;
            pic.Dispose();
            return res;
        }

        /// <summary>
        /// Вспомогательная приватная функция, обрезающая изображения в отношении 13:8
        /// </summary>
        /// <param name="imgPath"></param>
        private static bool CropAndOverwrite(string imgPath)
        {
            //Load the original image
            Bitmap bMap = new Bitmap(imgPath);
            if (bMap.Width < 221 || bMap.Height < 136)
            {
                //221х136 - минимальный размер изображения в карусели
                bMap.Dispose();
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
                return false;
            }
            int h = bMap.Width / 13;
            var width = 13 * h;
            var height = 8 * h;
            var x = 0;
            var y = bMap.Height / 2 - height / 2;

            //Create a rectanagle to represent the cropping area
            Rectangle rect = new Rectangle(x, y, width, height);

            //The format of the target image which we will use as a parameter to the Save method
            var format = bMap.RawFormat;

            //Draw the cropped part to a new Bitmap
            var croppedImage = bMap.Clone(rect, bMap.PixelFormat);

            //Dispose the original image since we don't need it any more
            bMap.Dispose();

            //Remove the original image because the Save function will throw an exception and won't Overwrite by default
            if (System.IO.File.Exists(imgPath))
                System.IO.File.Delete(imgPath);

            //Save the result in the format of the original image
            croppedImage.Save(imgPath, format);
            //Dispose the result since we saved it
            croppedImage.Dispose();
            return true;
        }
    }

    public class FilmSountracks
    {
        public List<Audio> Tracks { get; set; }
        public DateTime DownloadTime { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsLoading { get; set; }
        public FilmSountracks()
        {
            DownloadTime = DateTime.Now;
            IsEmpty = true;
            IsLoading = true;
        }
        public void Update(List<Audio> _tracks)
        {
            Tracks = _tracks;
            DownloadTime = DateTime.Now;
            IsEmpty = false;
            IsLoading = false;
        }
    }

    public class ActorsTemplate
    {
        public List<(string, string, Photo,MessageKeyboard)> actors;
        public MessageTemplate Actors { get; set; }
        public DateTime DownloadTime { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsLoading { get; set; }
        public ActorsTemplate()
        {
            DownloadTime = DateTime.Now;
            IsEmpty = true;
            IsLoading = true;
        }
        public void Update(MessageTemplate _actors)
        {
            Actors = _actors;
            DownloadTime = DateTime.Now;
            IsEmpty = _actors.Elements.Count() == 0;
            IsLoading = false;
        }
         public void Update(List<(string, string, Photo,MessageKeyboard)> _actors)
        {
            actors = _actors;
            DownloadTime = DateTime.Now;
            IsEmpty = _actors.Count() == 0;
            IsLoading = false;
        }
    }

    public static class YouTube
    {
        public class PageInfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class Id
        {
            public string kind { get; set; }
            public string videoId { get; set; }
        }

        public class Default
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Medium
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class High
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Thumbnails
        {
            public Default @default { get; set; }
            public Medium medium { get; set; }
            public High high { get; set; }
        }

        public class Snippet
        {
            public DateTime publishedAt { get; set; }
            public string channelId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public Thumbnails thumbnails { get; set; }
            public string channelTitle { get; set; }
            public string liveBroadcastContent { get; set; }
            public DateTime publishTime { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public Id id { get; set; }
            public Snippet snippet { get; set; }
        }

        public class YouTubeResults
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string nextPageToken { get; set; }
            public string regionCode { get; set; }
            public PageInfo pageInfo { get; set; }
            public List<Item> items { get; set; }
        }
    }

}

