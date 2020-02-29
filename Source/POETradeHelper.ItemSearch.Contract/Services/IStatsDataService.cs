using POETradeHelper.ItemSearch.Contract.Models;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract
{
    public interface IStatsDataService
    {
        string GetId(ItemStat itemStat);
    }
}