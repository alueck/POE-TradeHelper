using POETradeHelper.ItemSearch.Contract.Models;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPseudoStatDataMappingService
    {
        IEnumerable<StatData> GetPseudoStatDatas(string itemStatId);
    }
}