using System.Text;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

public sealed class MapItemNameAndRarityGroup : NameAndRarityGroupBase
{
    public int MapTier { get; set; } = 1;

    public MapBlightedStatus BlightedStatus { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());

        if (this.BlightedStatus == MapBlightedStatus.Blighted)
        {
            sb.Append($"{Resources.BlightedPrefix} ");
        }
        else if (this.BlightedStatus == MapBlightedStatus.BlightRavaged)
        {
            sb.Append($"{Resources.BlightRavagedPrefix} ");
        }

        sb.AppendLine($"{Resources.MapTierDescriptor} {this.MapTier})");

        return sb.ToString();
    }
}