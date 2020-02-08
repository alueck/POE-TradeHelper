using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public abstract class ItemSearchRequestMapperBase : IItemSearchQueryRequestMapper
    {
        public abstract bool CanMap(Item item);

        public virtual SearchQueryRequest MapToQueryRequest(Item item)
        {
            var result = new SearchQueryRequest();

            this.MapItemName(result, item);
            this.MapItemType(result, item);
            this.MapItemRarity(result, item);

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

        protected virtual void MapItemType(SearchQueryRequest result, Item item)
        {
            result.Query.Type = item.Type;
        }

        protected virtual void MapItemRarity(SearchQueryRequest result, Item item)
        {
            result.Query.Filters.TypeFilters.Rarity = new OptionFilter
            {
                Option = item.Rarity == ItemRarity.Unique
                                ? ItemRarityFilterOptions.Unique
                                : ItemRarityFilterOptions.NonUnique
            };
        }
    }
}