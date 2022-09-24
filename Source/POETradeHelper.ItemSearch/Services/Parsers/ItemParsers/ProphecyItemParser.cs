using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class ProphecyItemParser : SimpleItemParserBase<ProphecyItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Resources.ProphecyKeyword));
        }
    }
}