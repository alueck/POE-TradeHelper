using System.Collections.Generic;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class LeagueDataService : DataServiceBase<LeagueData>, ILeagueDataService
    {
        public LeagueDataService(IHttpClientFactoryWrapper httpClientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
            : base(Resources.PoeTradeApiLeaguesEndpoint, httpClientFactory, poeTradeApiJsonSerializer)
        {
        }

        public IList<LeagueData> GetLeaguesData()
        {
            return this.Data;
        }
    }
}