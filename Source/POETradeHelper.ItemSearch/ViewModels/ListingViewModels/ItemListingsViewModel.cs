using POETradeHelper.ItemSearch.Contract.Models;
using System;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemListingsViewModel
    {
        public string ItemDescription { get; set; }

        public ItemRarity ItemRarity { get; set; }

        public Uri ListingsUri { get; set; }

        public IList<object> Listings { get; } = new List<object>();
    }
}