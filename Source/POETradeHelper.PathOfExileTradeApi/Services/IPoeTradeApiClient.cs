using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPoeTradeApiClient
    {
        Task<ItemListingsQueryResult> GetListingsAsync(SearchQueryRequest request, CancellationToken cancellationToken = default);

        Task<ExchangeQueryResult> GetListingsAsync(ExchangeQueryRequest request, CancellationToken cancellationToken = default);
    }
}