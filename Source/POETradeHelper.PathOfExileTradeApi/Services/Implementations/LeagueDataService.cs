using System;
using System.Collections.Generic;
using System.Linq;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class LeagueDataService : DataServiceBase<LeagueData>, ILeagueDataService
    {
        public LeagueDataService(
            IHttpClientFactoryWrapper httpClientFactory,
            IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
            : base(Resources.PoeTradeApiLeaguesEndpoint, httpClientFactory, poeTradeApiJsonSerializer)
        {
        }

        public IList<LeagueData> GetLeaguesData() => this.Data;

        protected override IList<LeagueData> GetData(QueryResult<LeagueData> queryResult) =>
            queryResult.Result
                .Where(leagueData => string.Equals(leagueData.Realm, "PC", StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}