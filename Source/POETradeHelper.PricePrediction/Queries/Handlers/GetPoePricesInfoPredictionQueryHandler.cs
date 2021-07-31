using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Queries.Handlers
{
    public class GetPoePricesInfoPredictionQueryHandler : IRequestHandler<GetPoePricesInfoPredictionQuery, PoePricesInfoPrediction>
    {
        private readonly IPoePricesInfoClient poePricesInfoClient;
        private readonly IMemoryCache memoryCache;

        public GetPoePricesInfoPredictionQueryHandler(
            IPoePricesInfoClient poePricesInfoClient,
            IMemoryCache memoryCache)
        {
            this.poePricesInfoClient = poePricesInfoClient;
            this.memoryCache = memoryCache;
        }

        public async Task<PoePricesInfoPrediction> Handle(GetPoePricesInfoPredictionQuery request, CancellationToken cancellationToken)
        {
            PoePricesInfoPrediction prediction = null;

            if (request.League != null
                && !string.IsNullOrEmpty(request.Item?.ItemText)
                && request.Item.Rarity == ItemRarity.Rare)
            {
                prediction = await this.GetPoePricesInfoItemAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return prediction;
        }

        private async Task<PoePricesInfoPrediction> GetPoePricesInfoItemAsync(GetPoePricesInfoPredictionQuery request, CancellationToken cancellationToken)
        {
            if (!this.memoryCache.TryGetValue(request.Item.ItemText, out PoePricesInfoPrediction prediction))
            {
                prediction = await this.poePricesInfoClient.GetPricePredictionAsync(request.League.Id, request.Item.ItemText, cancellationToken).ConfigureAwait(false);

                if (!cancellationToken.IsCancellationRequested && prediction != null)
                {
                    using var cacheEntry = this.memoryCache.CreateEntry(request.Item.ItemText);
                    cacheEntry.SetValue(prediction);
                    cacheEntry.SetSize(70);
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                }
            }

            return prediction;
        }
    }
}