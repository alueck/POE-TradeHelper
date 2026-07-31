using System.Collections.Generic;

using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPseudoStatDataMappingService
    {
        IEnumerable<IStatData> GetPseudoStatData(string itemStatId);
    }
}