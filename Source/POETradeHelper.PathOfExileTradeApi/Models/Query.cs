using System;
using System.Collections.Generic;
using System.Linq;

using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Query : ICloneable
    {
        public string? Name { get; set; }

        public TypeFilter? Type { get; set; }

        public string? Term { get; set; }

        public QueryFilters Filters { get; private set; } = new();

        public IList<StatFilters> Stats { get; private set; } = [];

        public OptionFilter Status { get; private set; } = new() { Option = "online" };

        public object Clone() =>
            new Query
            {
                Name = this.Name,
                Type = this.Type,
                Term = this.Term,
                Filters = (QueryFilters)this.Filters.Clone(),
                Stats = this.Stats.Select(s => (StatFilters)s.Clone()).ToList(),
                Status = (OptionFilter)this.Status.Clone(),
            };
    }
}