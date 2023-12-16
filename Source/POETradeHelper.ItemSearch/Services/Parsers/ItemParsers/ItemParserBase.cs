using System.Text.RegularExpressions;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public abstract class ItemParserBase : IItemParser
    {
        public Item Parse(string[] itemStringLines)
        {
            Item item = this.ParseItem(itemStringLines);
            item.ExtendedItemText = string.Join(Environment.NewLine, itemStringLines);

            return item;
        }

        public abstract bool CanParse(string[] itemStringLines);

        protected bool HasRarity(string[] itemStringLines, ItemRarity itemRarity) =>
            GetRarity(itemStringLines) == itemRarity;

        protected static ItemRarity? GetRarity(string[] itemStringLines)
        {
            string rarityDescriptor = Resources.RarityDescriptor;
            string? rarityLine = itemStringLines.FirstOrDefault(line => line.Contains(rarityDescriptor));

            return rarityLine?.Replace(rarityDescriptor, string.Empty).Trim().ParseToEnumByDisplayName<ItemRarity>();
        }

        protected static int GetIntegerFromFirstStringContaining(string[] itemStringLines, string containsString)
        {
            int result = 0;
            string? matchingLine = itemStringLines.FirstOrDefault(l => l.Contains(containsString));

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

        protected bool IsCorrupted(string[] lines) => lines.Any(l => l == Resources.CorruptedKeyword);

        protected bool IsIdentified(string[] itemStringLines) => !itemStringLines.Any(l => l.Contains(Resources.UnidentifiedKeyword));

        protected abstract Item ParseItem(string[] itemStringLines);
    }
}