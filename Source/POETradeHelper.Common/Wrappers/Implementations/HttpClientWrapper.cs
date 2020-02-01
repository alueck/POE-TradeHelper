using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace POETradeHelper.Common.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return this.httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return this.httpClient.PostAsync(requestUri, content);
        }
    }
}