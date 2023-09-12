using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryRequest : ICloneable
    {
        public Query Query { get; private set; } = new();

        public IDictionary<string, SortType> Sort { get; set; } = new Dictionary<string, SortType>
        {
            ["price"] = SortType.Asc,
        };

        [JsonIgnore]
        public string Endpoint => Resources.PoeTradeApiSearchEndpoint;

        [JsonIgnore]
        public string League { get; set; } = string.Empty;

        public object Clone() =>
            new SearchQueryRequest
            {
                Query = (Query)this.Query.Clone(),
                League = this.League,
                Sort = this.Sort.ToDictionary(entry => entry.Key, entry => entry.Value),
            };
    }
}