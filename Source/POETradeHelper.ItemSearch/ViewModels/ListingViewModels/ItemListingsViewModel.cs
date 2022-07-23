using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemListingsViewModel
    {
        public Uri ListingsUri { get; set; }

        public ICollection<SimpleListingViewModel> Listings { get; } = new ObservableCollection<SimpleListingViewModel>();
    }
}