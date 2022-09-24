using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class ItemListingsViewModel
    {
        public Uri ListingsUri { get; set; }

        public ICollection<SimpleListingViewModel> Listings { get; } = new ObservableCollection<SimpleListingViewModel>();
    }
}