using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class JewelItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public override bool CanMap(Item item)
        {
            return item is JewelItem;
        }
    }
}