using POETradeHelper.ItemSearch.Contract.Models;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ProphecyItemParser : SimpleItemParserBase<ProphecyItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(POETradeHelper.ItemSearch.Contract.Properties.Resources.ProphecyKeyword));
        }
    }
}