using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public abstract class FilterViewModelBase : ReactiveObject
    {
        public string Text { get; set; }

        [Reactive]
        public bool? IsEnabled { get; set; }
    }
}