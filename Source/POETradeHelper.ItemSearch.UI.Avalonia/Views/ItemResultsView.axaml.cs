using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public partial class ItemResultsView : ReactiveUserControl<ItemResultsViewModel>
    {
        public ItemResultsView()
        {
            this.InitializeComponent();
        }
    }
}