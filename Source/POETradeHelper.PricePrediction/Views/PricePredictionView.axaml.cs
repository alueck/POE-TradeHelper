using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.PricePrediction.Views
{
    public class PricePredictionView : UserControl
    {
        public PricePredictionView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
