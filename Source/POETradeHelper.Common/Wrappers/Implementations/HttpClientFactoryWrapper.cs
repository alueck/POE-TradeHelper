using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace POETradeHelper.Common.Wrappers.Implementations
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

        public IHttpClientWrapper CreateClient(string name)
        {
            return new HttpClientWrapper(this.httpClientFactory.CreateClient(name));
        }
    }
}