namespace POETradeHelper.ItemSearch.ViewModels
{
    public class MinMaxStatFilterViewModel : StatFilterViewModel, IMinMaxFilterViewModel
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string Current { get; set; }
    }
}