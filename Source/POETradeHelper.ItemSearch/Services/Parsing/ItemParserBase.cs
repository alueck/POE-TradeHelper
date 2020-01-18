using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Properties;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services
{
    public abstract class ItemParserBase : IItemParser
    {
        protected bool HasRarity(string[] itemStringLines, ItemRarity itemRarity)
        {
            return this.GetRarity(itemStringLines) == itemRarity;
        }

        protected ItemRarity GetRarity(string[] itemStringLines)
        {
            string rarityDescriptor = POETradeHelper.ItemSearch.Contract.Properties.Resources.RarityDescriptor;
            string rarityLine = itemStringLines.FirstOrDefault(line => line.Contains(rarityDescriptor));

            ItemRarity? rarity = rarityLine.Replace(rarityDescriptor, "").Trim().ParseToEnumByDisplayName<ItemRarity>();

            if (!rarity.HasValue)
            {
                throw new ParserException(itemStringLines, Resources.CouldNotParseRarityExceptionMessage);
            }

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
            return lines.Any(l => l == POETradeHelper.ItemSearch.Contract.Properties.Resources.CorruptedDescriptor);
        }

        protected bool IsIdentified(string[] itemStringLines)
        {
            return !itemStringLines.Any(l => l.Contains(POETradeHelper.ItemSearch.Contract.Properties.Resources.UnidentifiedDescriptor));
        }

        public abstract Item Parse(string[] itemStringLines);

        public abstract bool CanParse(string[] itemStringLines);
    }
}