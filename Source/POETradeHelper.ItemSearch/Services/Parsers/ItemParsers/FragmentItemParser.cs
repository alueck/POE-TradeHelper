using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
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
            return line.Contains(Resources.ItemLevelDescriptor)
                   || line.Contains(Resources.ProphecyKeyword);
        }
    }
}