using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public abstract class FiltersBase
    {
        protected void SetFilter(IFilter filter, [CallerMemberName] string filterName = null)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException(nameof(filterName));
            }

            if (filter == null)
            {
                this.filters.Remove(filterName);
            }
            else
            {
                this.filters[filterName.ToLower()] = filter;
            }
        }

        protected TFilter GetFilter<TFilter>([CallerMemberName] string filterName = null)
            where TFilter : IFilter
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException(nameof(filterName));
            }

            return this.Filters.TryGetValue(filterName.ToLower(), out object filter) ? (TFilter)filter : default;
        }

        private readonly Dictionary<string, object> filters = new Dictionary<string, object>();

        // has to be type object for System.Text.JSON serialization
        public IReadOnlyDictionary<string, object> Filters => this.filters;
    }
}