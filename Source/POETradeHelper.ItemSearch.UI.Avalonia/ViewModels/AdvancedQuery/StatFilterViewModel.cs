using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class StatFilterViewModel : FilterViewModelBase
    {
        public StatFilterViewModel()
        {
            this.IsEnabled = false;
        }

        public string Id { get; set; } = string.Empty;

        public int? Tier { get; set; }

        public StatCategory Category { get; set; }
    }
}
