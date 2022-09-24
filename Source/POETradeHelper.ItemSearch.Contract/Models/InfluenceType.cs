using POETradeHelper.ItemSearch.Contract.Properties;

using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum InfluenceType
    {
        None,

        [Display(Name = nameof(Resources.ShaperItem), ResourceType = typeof(Resources))]
        Shaper,

        [Display(Name = nameof(Resources.ElderItem), ResourceType = typeof(Resources))]
        Elder,

        [Display(Name = nameof(Resources.CrusaderItem), ResourceType = typeof(Resources))]
        Crusader,

        [Display(Name = nameof(Resources.HunterItem), ResourceType = typeof(Resources))]
        Hunter,

        [Display(Name = nameof(Resources.RedeemerItem), ResourceType = typeof(Resources))]
        Redeemer,

        [Display(Name = nameof(Resources.WarlordItem), ResourceType = typeof(Resources))]
        Warlord
    };
}