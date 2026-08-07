using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public abstract partial class FilterViewModelBase : ReactiveObject, IFilterViewModel
    {
        public string Text { get; set; } = string.Empty;

        [Reactive]
        public partial bool? IsEnabled { get; set; }
    }
}
