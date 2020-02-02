using POETradeHelper.PathOfExileTradeApi.JsonConverters;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class BoolOptionFilter : IFilter
    {
        [JsonConverter(typeof(JsonBoolStringConverter))]
        public bool Option { get; set; }
    }
}