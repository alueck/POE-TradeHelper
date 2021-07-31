using MediatR;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;

namespace POETradeHelper.PricePrediction.Queries
{
    public class GetPoePricesInfoPredictionQuery : IRequest<PoePricesInfoPrediction>
    {
        public GetPoePricesInfoPredictionQuery(Item item, League league)
        {
            Item = item;
            League = league;
        }

        public Item Item { get; }
        public League League { get; }
    }
}