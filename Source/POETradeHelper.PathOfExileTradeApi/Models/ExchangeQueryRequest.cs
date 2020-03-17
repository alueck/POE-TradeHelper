using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ExchangeQueryRequest : IQueryRequest
    {
        public Exchange Exchange { get; private set; } = new Exchange();

        public string Endpoint => Resources.PoeTradeApiExchangeEndpoint;

        public object Clone()
        {
            return new ExchangeQueryRequest
            {
                Exchange = (Exchange)this.Exchange.Clone()
            };
        }
    }
}