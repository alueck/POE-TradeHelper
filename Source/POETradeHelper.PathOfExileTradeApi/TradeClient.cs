using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi
{
    public class TradeClient : ITradeClient
    {
        public async Task<ListingResult> GetListingAsync(Item item)
        {
            await Task.Delay(2000);
            return null;
        }
    }
}