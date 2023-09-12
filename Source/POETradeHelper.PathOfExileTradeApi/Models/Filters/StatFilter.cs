using System;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class StatFilter : ICloneable
    {
        public string Id { get; set; } = string.Empty;

        [JsonIgnore]
        public string Text { get; set; } = string.Empty;

        public MinMaxFilter Value { get; set; } = new();

        public object Clone() =>
            new StatFilter
            {
                Id = this.Id,
                Text = this.Text,
                Value = (MinMaxFilter)this.Value.Clone(),
            };
    }
}