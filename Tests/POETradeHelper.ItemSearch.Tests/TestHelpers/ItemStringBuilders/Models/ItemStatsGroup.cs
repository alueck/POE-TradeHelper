using POETradeHelper.ItemSearch.Contract.Properties;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class ItemStatsGroup : ItemStatsGroupBase
    {
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.QualityDescriptor} +{Quality}% {Resources.AugmentedDescriptor}", () => Quality > 0);

            return stringBuilder.ToString();
        }
    }
}