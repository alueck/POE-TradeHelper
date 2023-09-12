using System.ComponentModel.DataAnnotations;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum EquippableItemCategory
    {
        Unknown,

        [Display(Name = nameof(Resources.EquippableItemCategory_Accessories), ResourceType = typeof(Resources))]
        Accessories,

        [Display(Name = nameof(Resources.EquippableItemCategory_Armour), ResourceType = typeof(Resources))]
        Armour,

        [Display(Name = nameof(Resources.EquippableItemCategory_Weapons), ResourceType = typeof(Resources))]
        Weapons,
    }
}