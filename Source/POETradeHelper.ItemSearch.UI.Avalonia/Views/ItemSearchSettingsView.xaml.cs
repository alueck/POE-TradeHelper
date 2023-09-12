using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class ItemSearchSettingsView : UserControl
    {
        public ItemSearchSettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}