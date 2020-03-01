using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class OrganItemParser : SimpleItemParserBase<OrganItem>
    {
        private readonly IOrganItemStatsParser organItemStatsParser;

        public OrganItemParser(IOrganItemStatsParser organItemStatsParser)
        {
            this.organItemStatsParser = organItemStatsParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Last().Contains(Contract.Properties.Resources.OrganItemDescriptor);
        }

        public override Item Parse(string[] itemStringLines)
        {
            OrganItem organItem = base.Parse(itemStringLines) as OrganItem;

            organItem.Stats = this.organItemStatsParser.Parse(itemStringLines);

            return organItem;
        }
    }
}