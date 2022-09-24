using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class ExchangeResultsView : ReactiveUserControl<ExchangeResultsViewModel>
    {
        public ExchangeResultsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
