using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class ItemListingsViewModel
    {
        public Uri? ListingsUri { get; set; }

        // object type is required for auto-generating columns
        public ICollection<object> Listings { get; } = new ObservableCollection<object>();
    }
}
