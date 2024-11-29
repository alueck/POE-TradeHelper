using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class StatFilters : ICloneable
    {
        public IList<StatFilter> Filters { get; private set; } = [];

        public string Type => "and";

        public object Clone()
        {
            return new StatFilters
            {
                Filters = this.Filters.Select(f => (StatFilter)f.Clone()).ToList(),
            };
        }
    }
}