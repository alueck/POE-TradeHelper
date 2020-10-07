using ReactiveUI;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public abstract class FilterViewModelBase : ReactiveObject
    {
        private bool isEnabled;

        public string Text { get; set; }

        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }
    }
}