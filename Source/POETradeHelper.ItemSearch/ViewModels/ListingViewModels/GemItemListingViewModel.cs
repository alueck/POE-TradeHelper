using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.Constants;
using POETradeHelper.ItemSearch.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class GemItemListingViewModel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.GemLevelColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Level { get; set; }

        [Display(ShortName = nameof(Resources.GemExperiencePercentColumn), ResourceType = typeof(Resources), Order = 4)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public decimal GemExperiencePercent { get; set; }

        [Display(ShortName = nameof(Resources.QualityColumn), ResourceType = typeof(Resources), Order = 5)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Quality { get; set; }
    }
}