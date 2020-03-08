using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.Constants;
using POETradeHelper.ItemSearch.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemListingViewModelWithItemLevel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.ItemLevelColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public int ItemLevel { get; set; }
    }
}