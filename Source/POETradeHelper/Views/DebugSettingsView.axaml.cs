using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.Views
{
    public partial class DebugSettingsView : UserControl
    {
        public DebugSettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}