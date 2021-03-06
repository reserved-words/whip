﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Whip.Common.Enums;
using Whip.Services.Interfaces;

namespace Whip.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IAsyncMethodInterceptor _interceptor;

        public ImageProcessingService(IAsyncMethodInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public byte[] GetImageBytesFromFile(string fileLocation)
        {
            if (string.IsNullOrEmpty(fileLocation))
                return null;

            var img = Image.FromFile(fileLocation);

            if (img == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public async Task<byte[]> GetImageBytesFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            using (var webClient = new WebClient())
            {
                return await _interceptor.TryMethod(webClient.DownloadDataTaskAsync(new Uri(url)), null, WebServiceType.Web, "Getting image " + url);
            }
        }

        public BitmapImage GetImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(bytes))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public async Task<BitmapImage> GetImageFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var bytes = await GetImageBytesFromUrl(url);
            return GetImageFromBytes(bytes);
        }
    }
}
