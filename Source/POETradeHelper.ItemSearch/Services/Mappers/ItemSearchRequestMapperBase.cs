﻿using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public abstract class ItemSearchRequestMapperBase : IItemSearchQueryRequestMapper
    {
        protected ItemSearchRequestMapperBase(IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.ItemSearchOptions = itemSearchOptions;
        }

        protected IOptionsMonitor<ItemSearchOptions> ItemSearchOptions { get; }

        public abstract bool CanMap(Item item);

        public virtual SearchQueryRequest MapToQueryRequest(Item item)
        {
            SearchQueryRequest result = new()
            {
                League = this.ItemSearchOptions.CurrentValue.League!.Id,
            };

            this.MapItemName(result, item);
            this.MapItemType(result, item);
            this.MapItemRarity(result, item);
            this.MapCorrupted(result, item);

            return result;
        }

        protected virtual void MapItemName(SearchQueryRequest result, Item item)
        {
            if (item.Rarity == ItemRarity.Unique
                && item is IIdentifiableItem identifiableItem
                && identifiableItem.IsIdentified)
            {
                result.Query.Name = item.Name;
            }
        }

        protected virtual void MapItemType(SearchQueryRequest result, Item item) =>
            result.Query.Type = new TypeFilter { Option = item.Type };

        protected virtual void MapItemRarity(SearchQueryRequest result, Item item) =>
            result.Query.Filters.TypeFilters.Rarity = new OptionFilter
            {
                Option = item.Rarity == ItemRarity.Unique
                    ? ItemRarityFilterOptions.Unique
                    : ItemRarityFilterOptions.NonUnique,
            };

        protected virtual void MapCorrupted(SearchQueryRequest result, Item item)
        {
            if (item is ICorruptableItem corruptableItem)
            {
                result.Query.Filters.MiscFilters.Corrupted = new BoolOptionFilter
                {
                    Option = corruptableItem.IsCorrupted,
                };
            }
        }
    }
}