using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public abstract class ItemWithStatsParserBase : ItemParserBase
    {
        private readonly IItemStatsParser<ItemWithStats> itemStatsParser;

        protected ItemWithStatsParserBase(IItemStatsParser<ItemWithStats> itemStatsParser)
        {
            this.itemStatsParser = itemStatsParser;
        }

        protected override Item ParseItem(string[] itemStringLines)
        {
            ItemWithStats item = this.ParseItemWithoutStats(itemStringLines);
            item.Stats = this.ParseStats(item, itemStringLines);

            return item;
        }

        protected abstract ItemWithStats ParseItemWithoutStats(string[] itemStringLines);

        private ItemStats? ParseStats(ItemWithStats item, string[] itemStringLines)
        {
            if (item is IIdentifiableItem { IsIdentified: false })
            {
                return null;
            }

            bool shouldPreferLocalStats = item is EquippableItem { Category: EquippableItemCategory.Armour or EquippableItemCategory.Weapons };

            return this.itemStatsParser.Parse(itemStringLines, shouldPreferLocalStats);
        }
    }
}