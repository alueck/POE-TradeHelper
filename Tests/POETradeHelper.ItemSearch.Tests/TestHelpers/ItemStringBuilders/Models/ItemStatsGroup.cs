using System.Text;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models
{
    public class ItemStatsGroup : ItemStatsGroupBase
    {
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.QualityDescriptor} +{this.Quality}% {Resources.AugmentedDescriptor}", () => this.Quality > 0);

            return stringBuilder.ToString();
        }
    }
}