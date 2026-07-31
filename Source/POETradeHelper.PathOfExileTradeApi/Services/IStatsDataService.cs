using System.Collections.Generic;

using POETradeHelper.Common.Contract;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IStatsDataService : IInitializable
    {
        /// <summary>
        /// Searches the given <paramref name="statCategoriesToSearch"/> for stat data for the given <paramref name="itemStatLines"/>.
        /// If <paramref name="statCategoriesToSearch"/> is empty all stat categories are searched.
        /// </summary>
        /// <param name="itemStatLines">
        ///     The stat text lines for which to retrieve stat data. Some stats span multiple lines. Only the first n lines matching
        ///     the number of lines of a stat are considered.
        /// </param>
        /// <param name="preferLocalStat">defines if a local stat should be preferred if there is one with the same <paramref name="itemStatLines"/></param>
        /// <param name="statCategoriesToSearch">a list of stat categories to search, if it is empty all stat categories are searched</param>
        /// <returns>the stat data if found; otherwise null</returns>
        IStatData? TryGetStatData(IReadOnlyCollection<string> itemStatLines, bool preferLocalStat, params string[] statCategoriesToSearch);

        IStatData? GetStatDataById(string itemStatId);
    }
}