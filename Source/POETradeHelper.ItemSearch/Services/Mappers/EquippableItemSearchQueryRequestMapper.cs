using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class EquippableItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        private static readonly IDictionary<InfluenceType, Action<MiscFilters>?> InfluenceMappings =
            new Dictionary<InfluenceType, Action<MiscFilters>?>
            {
                [InfluenceType.None] = null,
                [InfluenceType.Crusader] = miscFilters => miscFilters.CrusaderItem = new BoolOptionFilter { Option = true },
                [InfluenceType.Elder] = miscFilters => miscFilters.ElderItem = new BoolOptionFilter { Option = true },
                [InfluenceType.Hunter] = miscFilters => miscFilters.HunterItem = new BoolOptionFilter { Option = true },
                [InfluenceType.Redeemer] = miscFilters => miscFilters.RedeemerItem = new BoolOptionFilter { Option = true },
                [InfluenceType.Shaper] = miscFilters => miscFilters.ShaperItem = new BoolOptionFilter { Option = true },
                [InfluenceType.Warlord] = miscFilters => miscFilters.WarlordItem = new BoolOptionFilter { Option = true },
            };

        public EquippableItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
            : base(itemSearchOptions)
        {
        }

        public override bool CanMap(Item item) => item is EquippableItem;

        public override SearchQueryRequest MapToQueryRequest(Item item)
        {
            SearchQueryRequest result = base.MapToQueryRequest(item);

            EquippableItem equippableItem = (EquippableItem)item;
            MapItemLinks(result, equippableItem);
            MapInfluence(result, equippableItem);
            this.MapItemLevel(result, equippableItem);

            return result;
        }

        private void MapItemLevel(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (equippableItem.ItemLevel >= this.ItemSearchOptions.CurrentValue.ItemLevelThreshold)
            {
                result.Query.Filters.MiscFilters.ItemLevel = new MinMaxFilter
                {
                    Min = equippableItem.ItemLevel,
                };
            }
        }

        private static void MapItemLinks(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (equippableItem.Sockets?.Count == 0)
            {
                return;
            }

            int? maxLinks = equippableItem.Sockets?.SocketGroups.Max(socketGroup => socketGroup.Links);

            if (maxLinks >= 5)
            {
                result.Query.Filters.SocketFilters.Links = new SocketsFilter
                {
                    Min = maxLinks,
                    Max = maxLinks,
                };
            }
        }

        private static void MapInfluence(SearchQueryRequest result, EquippableItem equippableItem)
        {
            if (InfluenceMappings.TryGetValue(equippableItem.Influence, out Action<MiscFilters>? setInfluenceAction))
            {
                setInfluenceAction?.Invoke(result.Query.Filters.MiscFilters);
            }
        }
    }
}