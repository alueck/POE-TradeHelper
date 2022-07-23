using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.ViewModels;

namespace POETradeHelper.ItemSearch.Views
{
    public class ItemResultsView : ReactiveUserControl<ItemResultsViewModel>
    {
        public ItemResultsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
