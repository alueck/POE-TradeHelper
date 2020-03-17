using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.Constants;
using POETradeHelper.ItemSearch.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class FlaskItemListingViewModel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.QualityColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Quality { get; set; }
    }
}