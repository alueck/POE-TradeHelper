using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryRequest
    {
        public Query Query { get; } = new Query();

        public IDictionary<string, SortType> Sort { get; } = new Dictionary<string, SortType>
        {
            ["price"] = SortType.Asc
        };
    }
}