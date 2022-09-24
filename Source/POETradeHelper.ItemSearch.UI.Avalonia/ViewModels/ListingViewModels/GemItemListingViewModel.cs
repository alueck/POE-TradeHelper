using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Constants;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class GemItemListingViewModel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.GemLevelColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Level { get; set; } = string.Empty;

        [Display(ShortName = nameof(Resources.GemExperiencePercentColumn), ResourceType = typeof(Resources), Order = 4)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public decimal GemExperiencePercent { get; set; }

        [Display(ShortName = nameof(Resources.QualityColumn), ResourceType = typeof(Resources), Order = 5)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Quality { get; set; } = string.Empty;
    }
}
