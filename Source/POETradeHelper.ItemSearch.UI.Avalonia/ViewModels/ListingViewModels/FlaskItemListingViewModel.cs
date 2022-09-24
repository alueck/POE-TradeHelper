using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Constants;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class FlaskItemListingViewModel : SimpleListingViewModel
    {
        [Display(ShortName = nameof(Resources.QualityColumn), ResourceType = typeof(Resources), Order = 3)]
        [StyleClasses(StyleClass.DataGridCellAlignRight)]
        public string Quality { get; set; } = string.Empty;
    }
}
