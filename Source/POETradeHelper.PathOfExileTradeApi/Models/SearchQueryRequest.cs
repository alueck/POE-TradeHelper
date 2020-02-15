using POETradeHelper.PathOfExileTradeApi.Properties;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryRequest : IQueryRequest
    {
        public Query Query { get; } = new Query();

        public IDictionary<string, SortType> Sort { get; } = new Dictionary<string, SortType>
        {
            ["price"] = SortType.Asc
        };

        [JsonIgnore]
        public string Endpoint => Resources.PoeTradeApiSearchEndpoint;
    }
}