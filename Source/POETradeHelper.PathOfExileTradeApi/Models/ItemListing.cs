using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ItemListing
    {
        [JsonPropertyName("ilvl")]
        public byte ItemLevel { get; set; }

        public int? StackSize { get; set; }

        public IList<Property> Properties { get; set; } = [];

        public IList<Property> AdditionalProperties { get; set; } = [];

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; } = [];
    }
}