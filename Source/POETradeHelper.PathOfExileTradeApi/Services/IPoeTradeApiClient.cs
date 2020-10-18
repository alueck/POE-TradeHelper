using System.Threading.Tasks;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPoeTradeApiClient
    {
        Task<ItemListingsQueryResult> GetListingsAsync(IQueryRequest queryRequest, System.Threading.CancellationToken cancellationToken = default);
    }
}