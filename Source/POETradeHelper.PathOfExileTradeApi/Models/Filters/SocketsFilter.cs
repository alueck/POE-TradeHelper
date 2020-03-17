using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class SocketsFilter : MinMaxFilter
    {
        [JsonPropertyName("r")]
        public int? Red { get; set; }

        [JsonPropertyName("g")]
        public int? Green { get; set; }

        [JsonPropertyName("b")]
        public int? Blue { get; set; }

        [JsonPropertyName("w")]
        public int? White { get; set; }
    }
}