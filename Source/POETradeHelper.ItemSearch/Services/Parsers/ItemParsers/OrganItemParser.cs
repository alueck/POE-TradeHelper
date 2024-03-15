using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class OrganItemParser : SimpleItemParserBase<OrganItem>
    {
        private readonly IItemStatsParser<OrganItem> organItemStatsParser;

        public OrganItemParser(IItemStatsParser<OrganItem> organItemStatsParser)
        {
            this.organItemStatsParser = organItemStatsParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines[^1].Contains(Contract.Properties.Resources.OrganItemDescriptor);
        }

        protected override Item ParseItem(string[] itemStringLines)
        {
            OrganItem organItem = (OrganItem)base.ParseItem(itemStringLines);

            organItem.Stats = this.organItemStatsParser.Parse(itemStringLines, false);

            return organItem;
        }
    }
}