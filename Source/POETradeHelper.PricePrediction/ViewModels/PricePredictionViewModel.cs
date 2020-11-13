using Avalonia.Media.Imaging;

namespace POETradeHelper.PricePrediction.ViewModels
{
    public class PricePredictionViewModel
    {
        public string Prediction { get; set; }

        public string Currency { get; set; }

        public IBitmap CurrencyImage { get; set; }

        public string ConfidenceScore { get; set; }

        public bool HasValue => !string.IsNullOrEmpty(this.Prediction) && !string.IsNullOrEmpty(this.Currency);
    }
}