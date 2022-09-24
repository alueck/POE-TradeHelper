using POETradeHelper.Common.UI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public interface IItemSearchResultOverlayView : IHideable
    {
        bool IsVisible { get; set; }
    }
}