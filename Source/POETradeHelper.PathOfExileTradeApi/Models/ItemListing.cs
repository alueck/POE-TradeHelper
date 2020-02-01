using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ItemListing
    {
        public bool Verified { get; set; }

        [JsonPropertyName("icon")]
        public string IconUri { get; set; }

        public string Name { get; set; }

        public string TypeLine { get; set; }

        public bool Identified { get; set; }

        [JsonPropertyName("ilvl")]
        public byte ItemLevel { get; set; }
    }
}