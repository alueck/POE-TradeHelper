using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.PricePrediction.Views
{
    public class PricePredictionControl : UserControl
    {
        public PricePredictionControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
