using System.Text;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models
{
    public class GemItemStatsGroup : ItemStatsGroupBase
    {
        public int GemLevel { get; set; }
        public string Tags { get; set; } = "Tag, Tag1, Tag2";
        public string Experience { get; set; } = string.Empty;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine(this.Tags)
                .AppendLine($"{Resources.LevelDescriptor} {this.GemLevel}", () => this.GemLevel > 0)
                .AppendLine($"{Resources.QualityDescriptor} +{this.Quality}% {Resources.AugmentedDescriptor}", () => this.Quality > 0)
                .AppendLine($"{Resources.ExperienceDescriptor} {this.Experience}", () => !string.IsNullOrWhiteSpace(this.Experience));

            return stringBuilder.ToString();
        }
    }
}