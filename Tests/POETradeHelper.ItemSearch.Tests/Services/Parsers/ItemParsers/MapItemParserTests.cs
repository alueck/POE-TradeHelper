using AwesomeAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class MapItemParserTests : ItemParserTestsBase
    {
        private readonly IItemStatsParser<ItemWithStats> itemStatsParserMock;
        private readonly MapItemStringBuilder mapItemStringBuilder;

        public MapItemParserTests()
        {
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<ItemWithStats>>();
            this.ItemParser = new MapItemParser(this.itemStatsParserMock);
            this.mapItemStringBuilder = new MapItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemContainsMapTier()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseMapRarity()
        {
            const ItemRarity expected = ItemRarity.Magic;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .WithRarity(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Rarity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseMapTier()
        {
            const int expected = 10;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Tier.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseItemQuantity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemQuantity(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.ItemQuantity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseItemRarity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemRarity(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.ItemRarity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseMonsterPackSize()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMonsterPackSize(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.MonsterPackSize.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Quality.Should().Be(expected);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ParseShouldParseIdentified(bool identified)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithIdentified(identified)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().Be(identified);
        }

        [TestCase(ItemRarity.Magic, true)]
        [TestCase(ItemRarity.Rare, true)]
        [TestCase(ItemRarity.Unique, true)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        public void ParseShouldNotSetType(ItemRarity itemRarity, bool isIdentified)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(itemRarity)
                .WithIdentified(isIdentified)
                .WithName("Dig Map")
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().BeEmpty();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ParseShouldParseCorruptedMap(bool isCorrupted)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithCorrupted(isCorrupted)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().Be(isCorrupted);
        }

        [Test]
        public void ParseShouldParseNormalRarityMap()
        {
            string expectedName = $"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} Dig Map";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName(expectedName)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expectedName);
        }

        [Test]
        public void ParseShouldCallParseOnMapItemStatsParserIfMapIsIdentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Desolate Cradle")
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, false);
        }

        [Test]
        public void ParseShouldNotCallParseOnMapItemStatsParserIfMapIsUnidentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Desolate Cradle")
                .WithUnidentified()
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .DidNotReceive()
                .Parse(Arg.Any<string[]>(), Arg.Any<bool>());
        }

        [Test]
        public void ParseShouldSetStatsOnMapItemFromStatsDataService()
        {
            ItemStats expected = new();
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Desolate Cradle")
                .BuildLines();

            this.itemStatsParserMock.Parse(Arg.Any<string[]>(), Arg.Any<bool>())
                .Returns(expected);

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.Stats.Should().BeSameAs(expected);
        }

        [Test]
        public void ParseShouldParseBlightedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Cemetery")
                .WithBlighted(MapBlightedStatus.Blighted)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.IsBlighted.Should().BeTrue();
            result.IsBlightRavaged.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseBlightRavagedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Cemetery")
                .WithBlighted(MapBlightedStatus.BlightRavaged)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            result.IsBlightRavaged.Should().BeTrue();
            result.IsBlighted.Should().BeFalse();
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName("Desolate Cradle")
                .WithUnidentified()
                .BuildLines();
        }
    }
}