using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class DivinationCardItemParser : SimpleItemParserBase<DivinationCardItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.DivinationCard);
        }
    }
}