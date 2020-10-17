using System.Collections.Generic;
using POETradeHelper.Common;
using POETradeHelper.PathOfExileTradeApi.Models.Data;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface ILeagueDataService : IInitializable
    {
        IList<LeagueData> GetLeaguesData();
    }
}