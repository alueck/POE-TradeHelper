using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class FlaskItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public override bool CanMap(Item item)
        {
            return item is FlaskItem;
        }
    }
}