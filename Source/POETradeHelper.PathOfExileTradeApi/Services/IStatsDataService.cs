using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IStatsDataService
    {
        /// <summary>
        /// Searches the given <paramref name="statCategoriesToSearch"/> for stat data for the given <paramref name="itemStatText"/>.
        /// If <paramref name="statCategoriesToSearch"/> is empty all stat categories are searched.
        /// </summary>
        /// <param name="itemStatText">the stat text for which to retrieve stat data</param>
        /// <param name="preferLocalStat">defines if a local stat should be preferred if there is one with the same <paramref name="itemStatText"/></param>
        /// <param name="statCategoriesToSearch">a list of <see cref="StatCategory"/> to search, if it is empty all stat categories are searched</param>
        /// <returns></returns>
        StatData? GetStatData(string itemStatText, bool preferLocalStat, params string[] statCategoriesToSearch);

        StatData? GetStatDataById(string itemStatId);
    }
}