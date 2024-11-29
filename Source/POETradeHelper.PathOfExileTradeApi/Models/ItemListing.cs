using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ItemListing
    {
        public bool Identified { get; set; }

        public bool Corrupted { get; set; }

        [JsonPropertyName("ilvl")]
        public byte ItemLevel { get; set; }

        public int? StackSize { get; set; }

        public IList<Property> Properties { get; set; } = [];

        public IList<Property> AdditionalProperties { get; set; } = [];
    }
}