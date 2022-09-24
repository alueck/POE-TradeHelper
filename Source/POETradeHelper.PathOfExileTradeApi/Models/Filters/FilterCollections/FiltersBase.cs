using POETradeHelper.Common;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public abstract class FiltersBase<TType> : ICloneable
        where TType : FiltersBase<TType>, new()
    {
        private static readonly JsonSnakeCaseNamingPolicy snakeCaseNamingPolicy = new();

        protected void SetFilter(IFilter? filter, [CallerMemberName] string? filterName = null)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException(nameof(filterName));
            }

            if (filter == null)
            {
                this.filters.Remove(snakeCaseNamingPolicy.ConvertName(filterName));
            }
            else
            {
                this.filters[snakeCaseNamingPolicy.ConvertName(filterName)] = filter;
            }
        }

        protected TFilter? GetFilter<TFilter>([CallerMemberName] string? filterName = null)
            where TFilter : IFilter
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException(nameof(filterName));
            }

            return this.filters.TryGetValue(snakeCaseNamingPolicy.ConvertName(filterName), out object? filter) ? (TFilter)filter : default;
        }

        public object Clone()
        {
            var result = new TType();

            foreach (var entry in this.filters)
            {
                result.filters.Add(entry.Key, ((ICloneable)entry.Value).Clone());
            }

            return result;
        }

        private readonly Dictionary<string, object> filters = new();

        /// <summary>
        /// This should only be used for JSON serialization and not accessed otherwise.
        /// System.Text.Json can only access public properties at the moment. The type needs to be object,
        /// so the JSON serializer from System.Text.Json handles each object as the derived and not the base type.
        /// </summary>
        public IReadOnlyDictionary<string, object>? Filters => this.filters.Count > 0 ? this.filters : null;
    }
}