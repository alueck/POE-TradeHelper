using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.Views
{
    public partial class DebugSettingsView : UserControl
    {
        public DebugSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
