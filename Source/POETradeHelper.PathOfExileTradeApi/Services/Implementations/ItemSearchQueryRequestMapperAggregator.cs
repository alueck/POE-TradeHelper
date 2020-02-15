using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class ItemSearchQueryRequestMapperAggregator : IItemSearchQueryRequestMapperAggregator
    {
        private readonly IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers;

        public ItemSearchQueryRequestMapperAggregator(IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers)
        {
            this.itemToQueryRequestMappers = itemToQueryRequestMappers;
        }

        public IQueryRequest MapToQueryRequest(Item item)
        {
            var mapper = this.itemToQueryRequestMappers.FirstOrDefault(m => m.CanMap(item));

            return mapper?.MapToQueryRequest(item);
        }
    }
}