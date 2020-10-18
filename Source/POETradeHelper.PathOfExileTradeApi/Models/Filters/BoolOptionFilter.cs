using System.Text.Json.Serialization;
using POETradeHelper.PathOfExileTradeApi.JsonConverters;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class BoolOptionFilter : IFilter
    {
        [JsonConverter(typeof(JsonBoolStringConverter))]
        public bool Option { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}