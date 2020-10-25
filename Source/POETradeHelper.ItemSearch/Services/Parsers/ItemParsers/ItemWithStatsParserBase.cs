using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class ItemWithStatsParserBase : ItemParserBase
    {
        private readonly IItemStatsParser<ItemWithStats> itemStatsParser;

        protected ItemWithStatsParserBase(IItemStatsParser<ItemWithStats> itemStatsParser)
        {
            this.itemStatsParser = itemStatsParser;
        }

        public override Item Parse(string[] itemStringLines)
        {
            ItemWithStats item = this.ParseItemWithoutStats(itemStringLines);
            item.Stats = this.ParseStats(item, itemStringLines);

            return item;
        }

        protected abstract ItemWithStats ParseItemWithoutStats(string[] itemStringLines);

        private ItemStats ParseStats(ItemWithStats item, string[] itemStringLines)
        {
            if (item is IIdentifiableItem identifiableItem && !identifiableItem.IsIdentified)
            {
                return null;
            }

            bool shouldPreferLocalStats = item is EquippableItem equippableItem
                                            && (equippableItem.Category == EquippableItemCategory.Armour
                                                || equippableItem.Category == EquippableItemCategory.Weapons);

            return this.itemStatsParser.Parse(itemStringLines, shouldPreferLocalStats);
        }
    }
}