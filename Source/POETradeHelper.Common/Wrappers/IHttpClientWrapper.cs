using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.Common.Wrappers
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
    }
}