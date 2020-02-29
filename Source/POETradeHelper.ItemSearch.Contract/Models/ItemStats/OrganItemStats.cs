using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class OrganItemStats
    {
        public IList<MonsterItemStat> Stats { get; } = new List<MonsterItemStat>();
    }
}