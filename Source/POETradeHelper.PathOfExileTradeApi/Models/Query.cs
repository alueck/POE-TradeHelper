using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Query : ICloneable
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Term { get; set; }

        public QueryFilters Filters { get; private set; } = new QueryFilters();

        public IList<StatFilters> Stats { get; private set; } = new List<StatFilters>();

        public OptionFilter Status { get; private set; } = new OptionFilter { Option = "online" };

        public object Clone()
        {
            return new Query
            {
                Name = this.Name,
                Type = this.Type,
                Term = this.Term,
                Filters = (QueryFilters)this.Filters.Clone(),
                Stats = this.Stats.Select(s => (StatFilters)s.Clone()).ToList(),
                Status = (OptionFilter)this.Status.Clone()
            };
        }
    }
}