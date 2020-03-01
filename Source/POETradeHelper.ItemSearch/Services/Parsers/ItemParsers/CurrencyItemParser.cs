using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class CurrencyItemParser : SimpleItemParserBase<CurrencyItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Currency);
        }
    }
}