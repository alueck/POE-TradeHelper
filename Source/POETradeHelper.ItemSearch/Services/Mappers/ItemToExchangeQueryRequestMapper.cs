using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class ItemToExchangeQueryRequestMapper : IItemSearchQueryRequestMapper
    {
        private IStaticDataService staticItemDataService;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public ItemToExchangeQueryRequestMapper(IStaticDataService staticDataService, IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.staticItemDataService = staticDataService;
            this.itemSearchOptions = itemSearchOptions;
        }

        public bool CanMap(Item item)
        {
            return item is CurrencyItem
                || item is FragmentItem
                || item is DivinationCardItem;
        }

        public IQueryRequest MapToQueryRequest(Item item)
        {
            var result = new ExchangeQueryRequest
            {
                League = this.itemSearchOptions.CurrentValue.League.Id
            };
            string itemId = this.staticItemDataService.GetId(item.Name);

            result.Exchange.Have.Add("chaos");
            result.Exchange.Want.Add(itemId);

            return result;
        }
    }
}