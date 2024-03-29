﻿using System.Collections.Generic;

using POETradeHelper.Common.Contract;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface ILeagueDataService : IInitializable
    {
        IList<LeagueData> GetLeaguesData();
    }
}