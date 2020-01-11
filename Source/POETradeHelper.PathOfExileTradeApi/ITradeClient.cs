using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi
{
    public interface ITradeClient
    {
        Task<ListingResult> GetListingAsync(Item item);
    }
}