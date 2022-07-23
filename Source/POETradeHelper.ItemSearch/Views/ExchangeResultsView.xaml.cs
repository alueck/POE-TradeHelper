using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using POETradeHelper.ItemSearch.ViewModels;

namespace POETradeHelper.ItemSearch.Views
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
