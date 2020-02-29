using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class StatFilter
    {
        public string Id { get; set; }

        [JsonIgnore]
        public string Text { get; set; }

        public MinMaxFilter Value { get; set; }
    }
}