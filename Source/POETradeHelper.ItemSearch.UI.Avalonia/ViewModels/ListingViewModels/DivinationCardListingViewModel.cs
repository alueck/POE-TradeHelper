using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Constants;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public sealed class DivinationCardListingViewModel : SimpleListingViewModel
{
    [Display(ShortName = nameof(Resources.StackSize), ResourceType = typeof(Resources), Order = 3)]
    [StyleClasses(StyleClass.DataGridCellAlignRight)]
    public int StackSize { get; set; }
}