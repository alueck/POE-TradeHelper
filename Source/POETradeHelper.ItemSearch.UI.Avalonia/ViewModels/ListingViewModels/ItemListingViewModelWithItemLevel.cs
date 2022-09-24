using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Constants;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class ItemListingViewModelWithItemLevel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.ItemLevelColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public int ItemLevel { get; set; }
    }
}