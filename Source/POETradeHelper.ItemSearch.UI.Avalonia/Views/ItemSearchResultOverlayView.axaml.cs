using Avalonia.Controls;
using Avalonia.Input;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public partial class ItemSearchResultOverlayView : Window, IItemSearchResultOverlayView
    {
        public ItemSearchResultOverlayView()
        {
            this.InitializeComponent();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
            base.OnPointerPressed(e);
        }
    }
}