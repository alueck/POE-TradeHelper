using POETradeHelper.ItemSearch.Contract.Models;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class OrganItemParser : SimpleItemParserBase<OrganItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Last().Contains(Contract.Properties.Resources.OrganItemDescriptor);
        }
    }
}