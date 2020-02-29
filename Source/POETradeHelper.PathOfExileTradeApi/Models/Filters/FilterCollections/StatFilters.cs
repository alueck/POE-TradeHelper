using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class StatFilters
    {
        public IList<StatFilter> Filters { get; } = new List<StatFilter>();

        public string Type => "and";
    }
}