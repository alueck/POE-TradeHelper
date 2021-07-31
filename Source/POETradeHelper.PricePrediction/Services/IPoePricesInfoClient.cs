using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.PricePrediction.Models;

namespace POETradeHelper.PricePrediction.Services
{
    public interface IPoePricesInfoClient
    {
        Task<PoePricesInfoPrediction> GetPricePredictionAsync(string league, string itemText, CancellationToken cancellationToken = default);
    }
}