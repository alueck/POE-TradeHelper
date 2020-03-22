using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.Constants;
using POETradeHelper.ItemSearch.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.ViewModels
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