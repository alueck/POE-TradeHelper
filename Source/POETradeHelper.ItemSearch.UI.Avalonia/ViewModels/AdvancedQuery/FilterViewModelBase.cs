using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public abstract class FilterViewModelBase : ReactiveObject, IFilterViewModel
    {
        public string Text { get; set; } = string.Empty;

        [Reactive]
        public bool? IsEnabled { get; set; }
    }
}
