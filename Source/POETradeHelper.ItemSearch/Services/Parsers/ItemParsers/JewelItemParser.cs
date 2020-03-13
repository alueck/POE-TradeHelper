using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class JewelItemParser : ItemWithStatsParserBase
    {
        public JewelItemParser(IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
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
                Name = itemStringLines[1],
                Type = GetType(itemStringLines, rarity.Value),
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines)
            };

            return item;
        }

        private string GetType(string[] itemStringLines, ItemRarity itemRarity)
        {
            int typeLineIndex = itemRarity >= ItemRarity.Rare ? 2 : 1;

            return Regex.Match(itemStringLines[typeLineIndex], $@"\w+\s{Resources.JewelKeyword}").Value;
        }
    }
}