using Avalonia.Media.Imaging;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class PriceViewModel
    {
        public string Amount { get; set; }

        public string Currency { get; set; }

        public IBitmap Image { get; set; }
    }
}