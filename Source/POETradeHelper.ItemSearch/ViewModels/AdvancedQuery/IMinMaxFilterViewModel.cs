namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IMinMaxFilterViewModel
    {
        string Text { get; set; }
        bool IsEnabled { get; set; }
        int? Min { get; set; }
        int? Max { get; set; }
        string Current { get; set; }
    }
}