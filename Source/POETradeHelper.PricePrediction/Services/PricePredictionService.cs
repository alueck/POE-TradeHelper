using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services.Factories;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Services
{
    public class PricePredictionService : IPricePredictionService
    {
        private readonly IPoePricesInfoClient poePricesInfoClient;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;
        private readonly IMemoryCache memoryCache;
        private readonly IPricePredictionViewModelFactory pricePredictionViewModelFactory;

        public PricePredictionService(
            IPoePricesInfoClient poePricesInfoClient,
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions,
            IMemoryCache memoryCache,
            IPricePredictionViewModelFactory pricePredictionViewModelFactory)
        {
            this.poePricesInfoClient = poePricesInfoClient;
            this.itemSearchOptions = itemSearchOptions;
            this.memoryCache = memoryCache;
            this.pricePredictionViewModelFactory = pricePredictionViewModelFactory;
        }

        public async Task<PricePredictionViewModel> GetPricePredictionAsync(Item item, CancellationToken cancellationToken = default)
        {
            PoePricesInfoItem prediction = null;

            if (!string.IsNullOrEmpty(item?.ItemText) && item.Rarity == ItemRarity.Rare)
            {
                prediction = await this.GetPoePricesInfoItemAsync(item.ItemText, cancellationToken).ConfigureAwait(false);
            }

            return await this.pricePredictionViewModelFactory.CreateAsync(prediction, cancellationToken).ConfigureAwait(false);
        }

        private async Task<PoePricesInfoItem> GetPoePricesInfoItemAsync(string itemText, CancellationToken cancellationToken)
        {
            if (!this.memoryCache.TryGetValue(itemText, out PoePricesInfoItem prediction))
            {
                prediction = await this.poePricesInfoClient.GetPricePredictionAsync(this.itemSearchOptions.CurrentValue.League.Id, itemText, cancellationToken).ConfigureAwait(false);

                if (!cancellationToken.IsCancellationRequested && prediction != null)
                {
                    using (var cacheEntry = this.memoryCache.CreateEntry(itemText))
                    {
                        cacheEntry.SetValue(prediction);
                        cacheEntry.SetSize(70);
                        cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                    }
                }
            }

            return prediction;
        }
    }
}