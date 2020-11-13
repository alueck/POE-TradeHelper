using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemListingsViewModel
    {
        public string ItemDescription { get; set; }

        public ItemRarity ItemRarity { get; set; }

        public Uri ListingsUri { get; set; }

        public ICollection<object> Listings { get; } = new ObservableCollection<object>();
    }
}