using System;
using System.Text.Json.Serialization;

using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ExchangeQueryRequest : ICloneable
    {
        public string Engine => "new";

        public Exchange Query { get; private set; } = new();

        [JsonIgnore]
        public string Endpoint => Resources.PoeTradeApiExchangeEndpoint;

        [JsonIgnore]
        public string League { get; set; } = string.Empty;

        public object Clone()
        {
            return new ExchangeQueryRequest
            {
                Query = (Exchange)this.Query.Clone(),
                League = this.League
            };
        }
    }
}