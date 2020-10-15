using System.ComponentModel.DataAnnotations;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public enum GemQualityType
    {
        Default = 0,

        [Display(Name = nameof(Resources.GemQualityType_Anomalous), ResourceType = typeof(Resources))]
        Anomalous = 1,

        [Display(Name = nameof(Resources.GemQualityType_Divergent), ResourceType = typeof(Resources))]
        Divergent = 2,

        [Display(Name = nameof(Resources.GemQualityType_Phantasmal), ResourceType = typeof(Resources))]
        Phantasmal = 3
    }
}