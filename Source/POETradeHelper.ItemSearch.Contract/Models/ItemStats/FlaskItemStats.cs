using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class FlaskItemStats
    {
        public IList<ExplicitItemStat> ExplicitStats { get; } = new List<ExplicitItemStat>();
    }
}