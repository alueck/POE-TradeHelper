using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services
{
    public class CurrencyItemParser : ItemParserBase
    {
        public override bool CanParse(string itemString)
        {
            return this.HasRarity(itemString, ItemRarity.Currency);
        }

        public override Item Parse(string itemString)
        {
            throw new System.NotImplementedException();
        }
    }
}