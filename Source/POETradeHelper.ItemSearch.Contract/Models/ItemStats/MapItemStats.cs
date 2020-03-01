using POETradeHelper.ItemSearch.Contract.Models.ItemStats;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MapItemStats : IHasExplicitStats
    {
        public IList<ExplicitItemStat> ExplicitStats { get; } = new List<ExplicitItemStat>();
    }
}