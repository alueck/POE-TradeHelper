using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Query
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Term { get; set; }

        public QueryFilters Filters { get; } = new QueryFilters();
    }
}