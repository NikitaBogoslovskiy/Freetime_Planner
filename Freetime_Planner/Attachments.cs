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

namespace Freetime_Planner
{
    /*
    public static class Attachments
    {
        /// <summary>
        /// Возвращает ID ВК загруженного постера фильма
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static string PosterID(Film movie)
        {
            if (movie.PhotoID == null)
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

        /// <summary>
        /// Вспомогательная приватная функция, обрезающая изображения в отношении 13:8
        /// </summary>
        /// <param name="imgPath"></param>
        private static void CropAndOverwrite(string imgPath)
        {
            //Load the original image
            Bitmap bMap = new Bitmap(imgPath);
            int h = bMap.Width / 13;
            var width = 13*h;
            var height = 8*h;
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
        }
    }
    */
}
