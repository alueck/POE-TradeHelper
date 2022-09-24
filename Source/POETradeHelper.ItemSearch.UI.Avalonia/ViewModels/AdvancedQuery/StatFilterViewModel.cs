namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class StatFilterViewModel : FilterViewModelBase
    {
        public StatFilterViewModel()
        {
            this.IsEnabled = false;
        }

        public string Id { get; set; } = string.Empty;
    }
}
