using System.Collections.Generic;

using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IPseudoItemStatsParser
    {
        IEnumerable<ItemStat> Parse(IEnumerable<ItemStat> itemStats);
    }
}