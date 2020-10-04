using System.Text;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class GemItemStatsGroup : ItemStatsGroupBase
    {
        public int GemLevel { get; set; }
        public string Tags { get; set; } = "Tag, Tag1, Tag2";
        public string Experience { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine(Tags)
                .AppendLine($"{Resources.LevelDescriptor} {GemLevel}", () => GemLevel > 0)
                .AppendLine($"{Resources.QualityDescriptor} +{Quality}% {Resources.AugmentedDescriptor}", () => Quality > 0)
                .AppendLine($"{Resources.ExperienceDescriptor} {this.Experience}", () => !string.IsNullOrWhiteSpace(this.Experience));

            return stringBuilder.ToString();
        }
    }
}