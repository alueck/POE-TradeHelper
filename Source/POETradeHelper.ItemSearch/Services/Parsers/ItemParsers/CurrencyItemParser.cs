using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class CurrencyItemParser : SimpleItemParserBase<CurrencyItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Currency);
        }
    }
}