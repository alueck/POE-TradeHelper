using System;
using System.Collections.ObjectModel;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class ItemListingsViewModel
    {
        public Uri? ListingsUri { get; set; }

        // object type is required for auto-generating columns
        public ObservableCollection<object> Listings { get; } = [];
    }
}
