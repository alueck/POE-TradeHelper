using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.PricePrediction.UI.Avalonia.Views
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
