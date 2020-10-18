using System.ComponentModel.DataAnnotations;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum StatCategory
    {
        Unknown,

        [Display(Name = nameof(Resources.StatCategoryPseudo), ResourceType = typeof(Resources))]
        Pseudo,

        [Display(Name = nameof(Resources.StatCategoryExplicit), ResourceType = typeof(Resources))]
        Explicit,

        [Display(Name = nameof(Resources.StatCategoryImplicit), ResourceType = typeof(Resources))]
        Implicit,

        [Display(Name = nameof(Resources.StatCategoryEnchant), ResourceType = typeof(Resources))]
        Enchant,

        [Display(Name = nameof(Resources.StatCategoryCrafted), ResourceType = typeof(Resources))]
        Crafted,

        [Display(Name = nameof(Resources.StatCategoryFractured), ResourceType = typeof(Resources))]
        Fractured,

        [Display(Name = nameof(Resources.StatCategoryMonster), ResourceType = typeof(Resources))]
        Monster
    }
}