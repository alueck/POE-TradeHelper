using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services
{
    public class CurrencyItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;

        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Currency);
        }

        public override Item Parse(string[] itemStringLines)
        {
            return new CurrencyItem
            {
                Name = itemStringLines[NameLineIndex],
                Type = itemStringLines[NameLineIndex]
            };
        }
    }
}