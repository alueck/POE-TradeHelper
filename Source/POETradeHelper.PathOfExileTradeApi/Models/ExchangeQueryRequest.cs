using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ExchangeQueryRequest : IQueryRequest
    {
        public Exchange Exchange { get; } = new Exchange();

        public string Endpoint { get; } = Resources.PoeTradeApiExchangeEndpoint;
    }
}