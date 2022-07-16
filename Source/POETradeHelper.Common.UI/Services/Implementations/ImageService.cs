using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Autofac.Extras.DynamicProxy;

using Avalonia.Media.Imaging;

using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.Common.UI.Services
{
    [Intercept(typeof(CacheResultAttributeInterceptor))]
    public class ImageService : IImageService
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IBitmapFactory bitmapFactory;

        public ImageService(IHttpClientFactoryWrapper httpClientFactory, IBitmapFactory bitmapFactory)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.bitmapFactory = bitmapFactory;
        }

        [CacheResult]
        public async Task<IBitmap> GetImageAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            IBitmap image = null;
            HttpResponseMessage response = await this.httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                image = this.bitmapFactory.Create(responseStream);
            }

            return image;
        }
    }
}
