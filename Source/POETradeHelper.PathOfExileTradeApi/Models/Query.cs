using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Query
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Term { get; set; }

        public QueryFilters Filters { get; } = new QueryFilters();

        public IList<StatFilters> Stats { get; } = new List<StatFilters>();

        public OptionFilter Status { get; } = new OptionFilter { Option = "online" };
    }
}