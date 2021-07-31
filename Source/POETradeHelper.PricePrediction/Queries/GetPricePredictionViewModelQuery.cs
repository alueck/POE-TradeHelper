using MediatR;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Queries
{
    public class GetPricePredictionViewModelQuery : IRequest<PricePredictionViewModel>
    {
        public GetPricePredictionViewModelQuery(Item item)
        {
            Item = item;
        }

        public Item Item { get; }
    }
}