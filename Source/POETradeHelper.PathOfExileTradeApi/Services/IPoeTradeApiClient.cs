using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPoeTradeApiClient
    {
        Task<ItemListingsQueryResult> GetListingsAsync(Item item, System.Threading.CancellationToken cancellationToken = default);

        Task<ItemListingsQueryResult> GetListingsAsync(IQueryRequest queryRequest, System.Threading.CancellationToken cancellationToken = default);
    }
}