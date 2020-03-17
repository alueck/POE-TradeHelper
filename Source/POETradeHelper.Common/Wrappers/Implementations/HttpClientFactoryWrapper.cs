using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace POETradeHelper.Common.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class HttpClientFactoryWrapper : IHttpClientFactoryWrapper
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpClientFactoryWrapper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public IHttpClientWrapper CreateClient()
        {
            return new HttpClientWrapper(this.httpClientFactory.CreateClient());
        }
    }
}