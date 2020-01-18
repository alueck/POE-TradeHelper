using POETradeHelper.ItemSearch.Contract.Properties;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class MapItemStatsGroup : ItemStatsGroupBase
    {
        public int MapTier { get; set; } = 1;
        public int ItemQuantity { get; set; }
        public int ItemRarity { get; set; }
        public int MonsterPackSize { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.MapTierDescriptor} {this.MapTier}")
                .AppendLine($"{Resources.ItemQuantityDescriptor} +{this.ItemQuantity}% ({Resources.AugmentedDescriptor})", () => this.ItemQuantity > 0)
                .AppendLine($"{Resources.ItemRarityDescriptor} +{this.ItemRarity}% ({Resources.AugmentedDescriptor})", () => this.ItemRarity > 0)
                .AppendLine($"{Resources.MonsterPackSizeDescriptor} +{this.MonsterPackSize}% ({Resources.AugmentedDescriptor})", () => this.MonsterPackSize > 0)
                .AppendLine($"{Resources.QualityDescriptor} +{this.Quality}% ({Resources.AugmentedDescriptor})", () => this.Quality > 0);

            return stringBuilder.ToString();
        }
    }
}