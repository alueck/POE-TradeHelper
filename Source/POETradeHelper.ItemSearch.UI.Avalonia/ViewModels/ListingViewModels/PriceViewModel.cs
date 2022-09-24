using Avalonia.Media.Imaging;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class PriceViewModel
    {
        public string Amount { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public IBitmap? Image { get; set; }
    }
}
