using POETradeHelper.ItemSearch.Contract.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum ItemRarity
    {
        [Display(Name = nameof(Resources.ItemRarityNormal), ResourceType = typeof(Resources))]
        Normal,

        [Display(Name = nameof(Resources.ItemRarityMagic), ResourceType = typeof(Resources))]
        Magic,

        [Display(Name = nameof(Resources.ItemRarityRare), ResourceType = typeof(Resources))]
        Rare,

        [Display(Name = nameof(Resources.ItemRarityUnique), ResourceType = typeof(Resources))]
        Unique,

        [Display(Name = nameof(Resources.ItemRarityGem), ResourceType = typeof(Resources))]
        Gem,

        [Display(Name = nameof(Resources.ItemRarityCurrency), ResourceType = typeof(Resources))]
        Currency,

        [Display(Name = nameof(Resources.ItemRarityDiviniationCard), ResourceType = typeof(Resources))]
        DivinationCard
    }
}