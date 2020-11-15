using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Services
{
    public interface IPricePredictionService
    {
        Task<PricePredictionViewModel> GetPricePredictionAsync(Item item, CancellationToken cancellationToken = default);
    }
}