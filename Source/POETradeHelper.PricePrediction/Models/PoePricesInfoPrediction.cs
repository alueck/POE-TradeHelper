using System.Text.Json.Serialization;

namespace POETradeHelper.PricePrediction.Models
{
    public record PoePricesInfoPrediction
    {
        public decimal Min { get; init; }

        public decimal Max { get; init; }

        public string Currency { get; init; } = string.Empty;

        [JsonPropertyName("error")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("pred_confidence_score")]
        public decimal ConfidenceScore { get; init; }
    }
}