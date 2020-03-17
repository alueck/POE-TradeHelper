namespace POETradeHelper.ItemSearch.ViewModels
{
    public class MinMaxFilterViewModel : FilterViewModel, IMinMaxFilterViewModel
    {
        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Current { get; set; }
    }
}