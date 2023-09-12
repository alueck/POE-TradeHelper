using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class ItemResultsView : ReactiveUserControl<ItemResultsViewModel>
    {
        public ItemResultsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}