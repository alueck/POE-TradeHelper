using Avalonia.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using POETradeHelper.Common.Wrappers;
using System;
using System.Threading.Tasks;

namespace POETradeHelper.Common.UI
{
    public class ImageService : IImageService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IHttpClientWrapper httpClient;

        public ImageService(IMemoryCache memoryCache, IHttpClientFactoryWrapper httpClientFactory)
        {
            this.memoryCache = memoryCache;
            this.httpClient = httpClientFactory.CreateClient();
        }

        public Task<IBitmap> GetImageAsync(Uri uri)
        {
            return this.memoryCache.GetOrCreateAsync(uri, async cacheEntry =>
            {
                var response = await this.httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    cacheEntry.SetSize(responseStream.Length);

                    return (IBitmap)new Bitmap(responseStream);
                }

                return null;
            });
        }
    }
}