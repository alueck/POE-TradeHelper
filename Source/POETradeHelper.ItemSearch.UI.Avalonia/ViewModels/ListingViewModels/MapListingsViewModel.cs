using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public class MapListingsViewModel : SimpleListingViewModel
{
    [Display(ShortName = nameof(Resources.MapAreaColumn), ResourceType = typeof(Resources), Order = 2)]
    [DataGridStarWidth(1)]
    public string MapArea { get; set; } = string.Empty;
}