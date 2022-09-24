using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemTypeParser : IItemTypeParser
    {
        private const int NamedItemTypeIndex = 3;

        private readonly IItemDataService itemDataService;

        public ItemTypeParser(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        public string? ParseType(string[] itemStringLines, ItemRarity itemRarity, bool isIdentified)
        {
            if (itemRarity != ItemRarity.Normal && itemRarity != ItemRarity.Magic && itemRarity != ItemRarity.Rare && itemRarity != ItemRarity.Unique)
            {
                throw new ArgumentException($"Item rarity {itemRarity} is not supported.", nameof(itemRarity));
            }

            bool itemHasName = isIdentified && itemRarity >= ItemRarity.Rare;

            return itemHasName
                ? itemStringLines[NamedItemTypeIndex]
                : this.itemDataService.GetType(itemStringLines[NamedItemTypeIndex - 1]);
        }
    }
}
