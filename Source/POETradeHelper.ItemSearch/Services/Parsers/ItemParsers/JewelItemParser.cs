using System.Linq;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class JewelItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 1;
        private readonly IItemDataService itemDataService;

        public JewelItemParser(IItemDataService itemDataService, IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
            this.itemDataService = itemDataService;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Skip(1).Take(2).Any(line => line.Contains(Resources.JewelKeyword));
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            var item = new JewelItem(rarity.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines)
            };

            item.Type = GetType(itemStringLines, item.Rarity, item.IsIdentified);

            return item;
        }

        private string GetType(string[] itemStringLines, ItemRarity itemRarity, bool isIdentified)
        {
            string type;

            if (!isIdentified)
            {
                type = itemStringLines[NameLineIndex];
            }
            else if (itemRarity == ItemRarity.Magic)
            {
                type = this.itemDataService.GetType(itemStringLines[NameLineIndex]);
            }
            else
            {
                type = itemStringLines[2];
            }

            return type;
        }
    }
}