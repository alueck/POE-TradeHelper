using POETradeHelper.ItemSearch.Contract.Properties;
using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.ItemSearch.Contract
{
    public enum StatCategory
    {
        [Display(Name = nameof(Resources.StatCategoryPseudo), ResourceType = typeof(Resources))]
        Pseudo,

        [Display(Name = nameof(Resources.StatCategoryExplicit), ResourceType = typeof(Resources))]
        Explicit,

        [Display(Name = nameof(Resources.StatCategoryImplicit), ResourceType = typeof(Resources))]
        Implicit,

        [Display(Name = nameof(Resources.StatCategoryFractured), ResourceType = typeof(Resources))]
        Fractured,

        [Display(Name = nameof(Resources.StatCategoryEnchant), ResourceType = typeof(Resources))]
        Enchant,

        [Display(Name = nameof(Resources.StatCategoryCrafted), ResourceType = typeof(Resources))]
        Crafted,

        [Display(Name = nameof(Resources.StatCategoryVeiled), ResourceType = typeof(Resources))]
        Veiled,

        [Display(Name = nameof(Resources.StatCategoryMonster), ResourceType = typeof(Resources))]
        Monster,

        [Display(Name = nameof(Resources.StatCategoryDelve), ResourceType = typeof(Resources))]
        Delve
    }
}