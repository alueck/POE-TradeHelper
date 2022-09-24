using POETradeHelper.ItemSearch.Contract.Models;

using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IPseudoItemStatsParser
    {
        IEnumerable<ItemStat> Parse(IEnumerable<ItemStat> itemStats);
    }
}