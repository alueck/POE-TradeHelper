using POETradeHelper.PathOfExileTradeApi.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryRequest : IQueryRequest
    {
        public Query Query { get; private set; } = new Query();

        public IDictionary<string, SortType> Sort { get; set; } = new Dictionary<string, SortType>
        {
            ["price"] = SortType.Asc
        };

        [JsonIgnore]
        public string Endpoint => Resources.PoeTradeApiSearchEndpoint;

        public object Clone()
        {
            return new SearchQueryRequest
            {
                Query = (Query)this.Query.Clone(),
                Sort = this.Sort.ToDictionary(entry => entry.Key, entry => entry.Value)
            };
        }
    }
}