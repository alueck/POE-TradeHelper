using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class EquippableItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        private static readonly IDictionary<InfluenceType, Action<MiscFilters>> InfluenceMappings = new Dictionary<InfluenceType, Action<MiscFilters>>
        {
            [InfluenceType.None] = null,
            [InfluenceType.Crusader] = (miscFilters) => miscFilters.CrusaderItem = new BoolOptionFilter { Option = true },
            [InfluenceType.Elder] = (miscFilters) => miscFilters.ElderItem = new BoolOptionFilter { Option = true },
            [InfluenceType.Hunter] = (miscFilters) => miscFilters.HunterItem = new BoolOptionFilter { Option = true },
            [InfluenceType.Redeemer] = (miscFilters) => miscFilters.RedeemerItem = new BoolOptionFilter { Option = true },
            [InfluenceType.Shaper] = (miscFilters) => miscFilters.ShaperItem = new BoolOptionFilter { Option = true },
            [InfluenceType.Warlord] = (miscFilters) => miscFilters.WarlordItem = new BoolOptionFilter { Option = true }
        };

        private IOptions<ItemSearchOptions> itemSearchOptions;

        public EquippableItemSearchQueryRequestMapper(IOptions<ItemSearchOptions> itemSearchOptions)
        {
            this.itemSearchOptions = itemSearchOptions;
        }

        public override bool CanMap(Item item)
        {
            return item is EquippableItem;
        }

        public override SearchQueryRequest MapToQueryRequest(Item item)
        {
            var result = base.MapToQueryRequest(item);

            var equippableItem = (EquippableItem)item;
            MapItemLinks(result, equippableItem);
            MapInfluence(result, equippableItem);
            this.MapItemLevel(result, equippableItem);

            return result;
        }

        private void MapItemLevel(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (equippableItem.ItemLevel >= itemSearchOptions.Value.ItemLevelThreshold)
            {
                result.Query.Filters.MiscFilters.ItemLevel = new MinMaxFilter
                {
                    Min = equippableItem.ItemLevel
                };
            }
        }

        private void MapItemLinks(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (equippableItem.Sockets?.Count == 0)
            {
                return;
            }

            decimal? maxLinks = equippableItem.Sockets?.SocketGroups.Max(socketGroup => socketGroup.Links);

            if (maxLinks >= 5)
            {
                result.Query.Filters.SocketFilters.Links = new SocketsFilter
                {
                    Min = equippableItem.Sockets.Count,
                    Max = equippableItem.Sockets.Count
                };
            }
        }

        private void MapInfluence(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (InfluenceMappings.TryGetValue(equippableItem.Influence, out var setInfluenceAction))
            {
                setInfluenceAction?.Invoke(result.Query.Filters.MiscFilters);
            }
        }
    }
}