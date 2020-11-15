using System;
using System.Linq;
using System.Text.RegularExpressions;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class ItemParserBase : IItemParser
    {
        protected bool HasRarity(string[] itemStringLines, ItemRarity itemRarity)
        {
            return this.GetRarity(itemStringLines) == itemRarity;
        }

        protected ItemRarity? GetRarity(string[] itemStringLines)
        {
            string rarityDescriptor = POETradeHelper.ItemSearch.Contract.Properties.Resources.RarityDescriptor;
            string rarityLine = itemStringLines.FirstOrDefault(line => line.Contains(rarityDescriptor));

            ItemRarity? rarity = rarityLine.Replace(rarityDescriptor, "").Trim().ParseToEnumByDisplayName<ItemRarity>();

            return rarity.Value;
        }

        protected int GetIntegerFromFirstStringContaining(string[] itemStringLines, string containsString)
        {
            int result = 0;
            string matchingLine = itemStringLines.FirstOrDefault(l => l.Contains(containsString));

            if (matchingLine != null)
            {
                Match match = Regex.Match(matchingLine, @"[\+\-]?\d+");

                if (match.Success)
                {
                    result = int.Parse(match.Value);
                }
            }

            return result;
        }

        protected bool IsCorrupted(string[] lines)
        {
            return lines.Any(l => l == POETradeHelper.ItemSearch.Contract.Properties.Resources.CorruptedKeyword);
        }

        protected bool IsIdentified(string[] itemStringLines)
        {
            return !itemStringLines.Any(l => l.Contains(POETradeHelper.ItemSearch.Contract.Properties.Resources.UnidentifiedKeyword));
        }

        public Item Parse(string[] itemStringLines)
        {
            Item item = this.ParseItem(itemStringLines);
            item.ItemText = string.Join(Environment.NewLine, itemStringLines);

            return item;
        }

        protected abstract Item ParseItem(string[] itemStringLines);

        public abstract bool CanParse(string[] itemStringLines);
    }
}