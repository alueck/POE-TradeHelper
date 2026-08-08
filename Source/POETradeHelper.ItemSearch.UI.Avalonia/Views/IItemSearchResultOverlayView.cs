using System.ComponentModel;

using Avalonia;

using POETradeHelper.Common.UI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public interface IItemSearchResultOverlayView : IHideable, IDataContextProvider, INotifyPropertyChanged
    {
        bool IsVisible { get; set; }
    }
}