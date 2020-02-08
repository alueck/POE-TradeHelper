using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class ItemToQueryRequestMapperAggregator : IItemSearchQueryRequestMapperAggregator
    {
        private readonly IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers;

        public ItemToQueryRequestMapperAggregator(IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers)
        {
            this.itemToQueryRequestMappers = itemToQueryRequestMappers;
        }

        public SearchQueryRequest MapToQueryRequest(Item item)
        {
            var mapper = this.itemToQueryRequestMappers.FirstOrDefault(m => m.CanMap(item));

            return mapper.MapToQueryRequest(item);
        }
    }
}