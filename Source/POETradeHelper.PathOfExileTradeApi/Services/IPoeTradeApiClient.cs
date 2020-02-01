using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPoeTradeApiClient
    {
        Task<ItemListingsQueryResult> GetListingsAsync(Item item);

        Task<IList<League>> GetLeaguesAsync();
    }
}