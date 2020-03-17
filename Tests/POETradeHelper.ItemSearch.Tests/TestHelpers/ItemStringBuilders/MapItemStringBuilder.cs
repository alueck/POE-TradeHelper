using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class MapItemStringBuilder : ItemStringBuilderBase<MapItemStringBuilder>
    {
        public MapItemStatsGroup ItemStatsGroup { get; } = new MapItemStatsGroup();

        public bool IsBlighted { get; private set; }

        public MapItemStringBuilder WithMapTier(int mapTier)
        {
            this.ItemStatsGroup.MapTier = mapTier;
            return this;
        }

        public MapItemStringBuilder WithItemQuantity(int itemQuantity)
        {
            this.ItemStatsGroup.ItemQuantity = itemQuantity;
            return this;
        }

        public MapItemStringBuilder WithItemRarity(int itemRarity)
        {
            this.ItemStatsGroup.ItemRarity = itemRarity;
            return this;
        }

        public MapItemStringBuilder WithMonsterPackSize(int monsterPackSize)
        {
            this.ItemStatsGroup.MonsterPackSize = monsterPackSize;
            return this;
        }

        public MapItemStringBuilder WithQuality(int quality)
        {
            this.ItemStatsGroup.Quality = quality;
            return this;
        }

        public override string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(this.NameAndRarityGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator)
                .Append(ItemStatsGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.IsCorrupted)
                .AppendLine(Resources.CorruptedKeyword, () => this.IsCorrupted);

            return stringBuilder.ToString();
        }
    }
}