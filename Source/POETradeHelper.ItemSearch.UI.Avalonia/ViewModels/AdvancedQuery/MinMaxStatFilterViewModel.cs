namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class MinMaxStatFilterViewModel : StatFilterViewModel, IMinMaxFilterViewModel
    {
        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public string Current { get; set; } = string.Empty;
    }
}