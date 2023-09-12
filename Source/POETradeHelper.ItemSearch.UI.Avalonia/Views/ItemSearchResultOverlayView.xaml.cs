using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class ItemSearchResultOverlayView : Window, IItemSearchResultOverlayView
    {
        public ItemSearchResultOverlayView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
            base.OnPointerPressed(e);
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}