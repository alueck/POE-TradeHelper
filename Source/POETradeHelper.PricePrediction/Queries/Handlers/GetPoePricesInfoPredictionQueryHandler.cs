using System.Threading;
using System.Threading.Tasks;

using Autofac.Extras.DynamicProxy;

using MediatR;

using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Queries.Handlers
{
    [Intercept(typeof(CacheResultAttributeInterceptor))]
    public class
        GetPoePricesInfoPredictionQueryHandler : IRequestHandler<GetPoePricesInfoPredictionQuery,
            PoePricesInfoPrediction>
    {
        private readonly IPoePricesInfoClient poePricesInfoClient;

        public GetPoePricesInfoPredictionQueryHandler(IPoePricesInfoClient poePricesInfoClient)
        {
            this.poePricesInfoClient = poePricesInfoClient;
        }

        [CacheResult(DurationSeconds = 30 * 60)]
        public async Task<PoePricesInfoPrediction> Handle(GetPoePricesInfoPredictionQuery request, CancellationToken cancellationToken)
        {
            PoePricesInfoPrediction prediction = null;

            if (request is { League: { }, Item: { Rarity: ItemRarity.Rare } }
                && !string.IsNullOrEmpty(request.Item.ItemText))
            {
                prediction = await this.poePricesInfoClient
                    .GetPricePredictionAsync(request.League.Id, request.Item.ItemText, cancellationToken)
                    .ConfigureAwait(false);
            }

            return prediction;
        }
    }
}