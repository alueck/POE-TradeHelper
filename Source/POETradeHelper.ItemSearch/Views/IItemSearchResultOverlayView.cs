using POETradeHelper.Common.UI;

namespace POETradeHelper.ItemSearch.Views
{
    public interface IItemSearchResultOverlayView : IHideable
    {
        bool IsVisible { get; set; }
    }
}