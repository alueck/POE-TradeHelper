using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract
{
    public interface IStatsDataService
    {
        StatData GetStatData(ItemStat itemStat);
    }
}