namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public interface IFilterViewModel
{
    string Text { get; set; }

    bool? IsEnabled { get; set; }
}