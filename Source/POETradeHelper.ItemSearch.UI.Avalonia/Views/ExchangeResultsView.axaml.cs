using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
     public partial class ExchangeResultsView : ReactiveUserControl<ExchangeResultsViewModel>
    {
        public ExchangeResultsView()
        {
            this.InitializeComponent();
        }
    }
}