using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public partial class AdvancedFiltersView : UserControl
    {
        public AdvancedFiltersView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}