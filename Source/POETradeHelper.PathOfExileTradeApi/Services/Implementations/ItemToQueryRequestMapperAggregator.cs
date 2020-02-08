using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class ItemToQueryRequestMapperAggregator : IItemSearchQueryRequestMapperAggregator
    {
        private readonly IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers;

        public ItemToQueryRequestMapperAggregator(IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers)
        {
            this.itemToQueryRequestMappers = itemToQueryRequestMappers;
        }

        public bool CanMap(Item item)
        {
            throw new NotImplementedException();
        }

        public SearchQueryRequest MapToQueryRequest(Item item)
        {
            var mapper = this.itemToQueryRequestMappers.FirstOrDefault(i => i.CanMap(item));

            return mapper.MapToQueryRequest(item);
        }
    }
}