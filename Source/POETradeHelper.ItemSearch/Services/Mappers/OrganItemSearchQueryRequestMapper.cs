﻿using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class OrganItemSearchQueryRequestMapper : IItemSearchQueryRequestMapper
    {
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public OrganItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.itemSearchOptions = itemSearchOptions;
        }

        public bool CanMap(Item item) => item is OrganItem;

        public SearchQueryRequest MapToQueryRequest(Item item)
        {
            OrganItem organItem = (OrganItem)item;

            SearchQueryRequest result = new()
            {
                League = this.itemSearchOptions.CurrentValue.League!.Id,
            };
            MapItemName(result, organItem);

            return result;
        }

        private static void MapItemName(SearchQueryRequest result, OrganItem organItem) =>
            result.Query.Term = organItem.Name;
    }
}