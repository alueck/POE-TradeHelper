using System;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class StatFilter : ICloneable
    {
        public string Id { get; set; }

        [JsonIgnore]
        public string Text { get; set; }

        public MinMaxFilter Value { get; set; }

        public object Clone()
        {
            return new StatFilter
            {
                Id = this.Id,
                Text = this.Text,
                Value = (MinMaxFilter)this.Value.Clone()
            };
        }
    }
}