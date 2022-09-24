using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface IPriceViewModelFactory
    {
        Task<PriceViewModel?> CreateAsync(Price? price, CancellationToken cancellationToken = default);
    }
}
