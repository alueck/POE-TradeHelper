using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Query
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public QueryFilters Filters { get; set; }

        public IDictionary<string, SortType> Sort { get; } = new Dictionary<string, SortType>
        {
            ["price"] = SortType.Asc
        };
    }
}