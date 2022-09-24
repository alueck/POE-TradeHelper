using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Constants;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.AccountNameColumn), ResourceType = typeof(Resources), Order = 0)]
        public string AccountName { get; set; }

        [Display(ShortName = nameof(Resources.PriceColumn), ResourceType = typeof(Resources), Order = 1)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public PriceViewModel Price { get; set; }

        [Display(ShortName = nameof(Resources.AgeColumn), ResourceType = typeof(Resources), Order = 99)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Age { get; set; }
    }
}