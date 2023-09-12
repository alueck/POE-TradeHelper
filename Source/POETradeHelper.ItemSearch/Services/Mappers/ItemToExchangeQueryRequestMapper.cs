using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class ItemToExchangeQueryRequestMapper : IItemToExchangeQueryRequestMapper
    {
        private readonly IStaticDataService staticItemDataService;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public ItemToExchangeQueryRequestMapper(
            IStaticDataService staticDataService,
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.staticItemDataService = staticDataService;
            this.itemSearchOptions = itemSearchOptions;
        }

        public ExchangeQueryRequest MapToQueryRequest(Item item)
        {
            if (item is not CurrencyItem or FragmentItem or DivinationCardItem)
            {
                throw new ArgumentException("Item must be currency, fragment or divination card.", nameof(item));
            }

            ExchangeQueryRequest result = new ExchangeQueryRequest
            {
                League = this.itemSearchOptions.CurrentValue.League.Id,
            };
            string? itemId = this.staticItemDataService.GetId(item.Name);

            result.Query.Want.Add("chaos");
            result.Query.Have.Add(itemId!);

            return result;
        }
    }
}