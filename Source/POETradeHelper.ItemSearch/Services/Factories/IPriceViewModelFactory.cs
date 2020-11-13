using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IPriceViewModelFactory
    {
        Task<PriceViewModel> CreateAsync(Price price, CancellationToken cancellationToken = default);
    }
}