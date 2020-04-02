using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract
{
    public interface IStatsDataService
    {
        /// <summary>
        /// Searches the given <paramref name="statCategoriesToSearch"/> for stat data for the given <paramref name="itemStat"/>.
        /// If <paramref name="statCategoriesToSearch"/> is empty or contains <see cref="StatCategory.Unknown"/> all stat categories are searched.
        /// </summary>
        /// <param name="itemStat">the <see cref="ItemStat"/> for which to retrieve stat data</param>
        /// <param name="statCategoriesToSearch">a list of <see cref="StatCategory"/> to search, if it is empty or contains <see cref="StatCategory.Unknown"/> all stat categories are searched</param>
        /// <returns></returns>
        StatData GetStatData(ItemStat itemStat, params StatCategory[] statCategoriesToSearch);

        StatData GetStatData(string itemStatId);
    }
}