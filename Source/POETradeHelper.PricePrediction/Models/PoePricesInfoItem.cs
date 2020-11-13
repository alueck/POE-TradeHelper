using System.Text.Json.Serialization;

namespace POETradeHelper.PricePrediction.Models
{
    public class PoePricesInfoItem
    {
        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public string Currency { get; set; }

        [JsonPropertyName("error")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("pred_confidence_score")]
        public decimal ConfidenceScore { get; set; }
    }
}