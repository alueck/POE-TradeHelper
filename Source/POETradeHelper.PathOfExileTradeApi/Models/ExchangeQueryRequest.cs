using System.Text.Json.Serialization;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ExchangeQueryRequest : IQueryRequest
    {
        public Exchange Exchange { get; private set; } = new Exchange();

        [JsonIgnore]
        public string Endpoint => Resources.PoeTradeApiExchangeEndpoint;

        [JsonIgnore]
        public string League { get; set; }

        public object Clone()
        {
            return new ExchangeQueryRequest
            {
                Exchange = (Exchange)this.Exchange.Clone(),
                League = this.League
            };
        }
    }
}