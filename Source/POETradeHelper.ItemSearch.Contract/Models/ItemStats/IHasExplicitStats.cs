using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models.ItemStats
{
    public interface IHasExplicitStats
    {
        IList<ExplicitItemStat> ExplicitStats { get; }
    }
}