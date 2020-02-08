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

            return this.filters.TryGetValue(filterName.ToLower(), out object filter) ? (TFilter)filter : default;
        }

        private readonly Dictionary<string, object> filters = new Dictionary<string, object>();

        /// <summary>
        /// This should only be used for JSON serialization and not accessed otherwise.
        /// System.Text.Json can only access public properties at the moment. The type needs to be object,
        /// so the JSON serializer from System.Text.Json handles each object as the derived and not the base type.
        /// </summary>
        public IReadOnlyDictionary<string, object> Filters => this.filters.Count > 0 ? this.filters : null;
    }
}