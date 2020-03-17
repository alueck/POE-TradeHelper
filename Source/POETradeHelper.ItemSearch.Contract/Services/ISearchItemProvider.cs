using POETradeHelper.ItemSearch.Contract.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Contract.Services
{
    public interface ISearchItemProvider
    {
        Task<Item> GetItemFromUnderCursorAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}