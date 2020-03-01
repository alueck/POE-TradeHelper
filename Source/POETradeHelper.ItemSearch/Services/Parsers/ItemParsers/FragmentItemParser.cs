using POETradeHelper.ItemSearch.Contract.Models;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FragmentItemParser : SimpleItemParserBase<FragmentItem>
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Normal)
                && !itemStringLines.Any(ContainsItemLevelOrProphecy);
        }

        private static bool ContainsItemLevelOrProphecy(string line)
        {
            return line.Contains(POETradeHelper.ItemSearch.Contract.Properties.Resources.ItemLevelDescriptor)
                || line.Contains(POETradeHelper.ItemSearch.Contract.Properties.Resources.ProphecyKeyword);
        }
    }
}