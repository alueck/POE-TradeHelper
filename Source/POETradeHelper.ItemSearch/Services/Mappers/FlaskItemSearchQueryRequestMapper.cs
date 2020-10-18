using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class FlaskItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public FlaskItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override bool CanMap(Item item)
        {
            return item is FlaskItem;
        }
    }
}