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

        public IList<Property> Properties { get; set; } = new List<Property>();

        public IList<Property> AdditionalProperties { get; set; } = new List<Property>();
    }
}