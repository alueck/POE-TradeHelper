using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Services.Factories
{
    public interface IPricePredictionViewModelFactory
    {
        Task<PricePredictionViewModel> CreateAsync(PoePricesInfoItem poePricesInfoItem, CancellationToken cancellationToken = default);
    }
}