using Avalonia;

using POETradeHelper.Common.UI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public interface IItemSearchResultOverlayView : IHideable, IDataContextProvider
    {
        bool IsVisible { get; set; }
    }
}