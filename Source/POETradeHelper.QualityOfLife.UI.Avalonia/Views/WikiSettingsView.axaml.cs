using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.QualityOfLife.UI.Avalonia.Views
{
    public partial class WikiSettingsView : UserControl
    {
        public WikiSettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}