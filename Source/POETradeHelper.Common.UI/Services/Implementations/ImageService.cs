using Avalonia.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using POETradeHelper.Common.Wrappers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace POETradeHelper.Common.UI.Services
{
    public class ImageService : IImageService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IHttpClientWrapper httpClient;
        private readonly IBitmapFactory bitmapFactory;

        public ImageService(IMemoryCache memoryCache, IHttpClientFactoryWrapper httpClientFactory, IBitmapFactory bitmapFactory)
        {
            this.memoryCache = memoryCache;
            this.httpClient = httpClientFactory.CreateClient();
            this.bitmapFactory = bitmapFactory;
        }

        public async Task<IBitmap> GetImageAsync(Uri uri)
        {
            if (!this.memoryCache.TryGetValue(uri, out IBitmap image))
            {
                HttpResponseMessage response = await this.httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();

                    image = this.bitmapFactory.Create(responseStream);

                    ICacheEntry cacheEntry = this.memoryCache.CreateEntry(uri);
                    cacheEntry.SetValue(image);
                    cacheEntry.SetSize(responseStream.Length);
                }
            }

            return image;
        }
    }
}