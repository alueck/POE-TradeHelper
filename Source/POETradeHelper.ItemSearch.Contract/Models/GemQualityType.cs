using System.ComponentModel.DataAnnotations;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum GemQualityType
    {
        Default,

        [Display(Name = nameof(Resources.GemQualityType_Anomalous), ResourceType = typeof(Resources))]
        Anomalous,

        [Display(Name = nameof(Resources.GemQualityType_Divergent), ResourceType = typeof(Resources))]
        Divergent,

        [Display(Name = nameof(Resources.GemQualityType_Phantasmal), ResourceType = typeof(Resources))]
        Phantasmal
    }
}