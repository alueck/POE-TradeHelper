namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IMinMaxFilterViewModel
    {
        string Text { get; set; }
        bool? IsEnabled { get; set; }
        decimal? Min { get; set; }
        decimal? Max { get; set; }
        string Current { get; set; }
    }
}