using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class ItemToExchangeQueryRequestMapper : IItemSearchQueryRequestMapper
    {
        private IStaticItemDataService staticItemDataService;

        public ItemToExchangeQueryRequestMapper(IStaticItemDataService staticItemDataService)
        {
            this.staticItemDataService = staticItemDataService;
        }

        public bool CanMap(Item item)
        {
            return item is CurrencyItem
                || item is FragmentItem
                || item is DivinationCardItem;
        }

        public IQueryRequest MapToQueryRequest(Item item)
        {
            var result = new ExchangeQueryRequest();
            string itemId = this.staticItemDataService.GetId(item);

            result.Exchange.Have.Add("chaos");
            result.Exchange.Want.Add(itemId);

            return result;
        }
    }
}