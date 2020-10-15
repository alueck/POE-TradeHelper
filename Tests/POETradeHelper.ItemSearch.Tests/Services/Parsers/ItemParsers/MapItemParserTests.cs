using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class MapItemParserTests
    {
        private Mock<IItemTypeParser> itemTypeParserMock;
        private Mock<IItemStatsParser<ItemWithStats>> itemStatsParserMock;
        private MapItemParser mapItemParser;
        private MapItemStringBuilder mapItemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.itemTypeParserMock = new Mock<IItemTypeParser>();
            this.itemStatsParserMock = new Mock<IItemStatsParser<ItemWithStats>>();
            this.mapItemParser = new MapItemParser(this.itemTypeParserMock.Object, this.itemStatsParserMock.Object);
            this.mapItemStringBuilder = new MapItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemContainsMapTier()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .BuildLines();

            bool result = this.mapItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void ParseShouldParseMapRarity()
        {
            const ItemRarity expected = ItemRarity.Magic;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .WithRarity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMapTier()
        {
            const int expected = 10;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Tier, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemQuantity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemQuantity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.ItemQuantity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemRarity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemRarity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.ItemRarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMonsterPackSize()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMonsterPackSize(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.MonsterPackSize, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [TestCase(ItemRarity.Magic, true)]
        [TestCase(ItemRarity.Rare, true)]
        [TestCase(ItemRarity.Unique, true)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        public void ParseShouldSetTypeFromItemTypeParser(ItemRarity itemRarity, bool isIdentified)
        {
            const string expected = "Result from ItemTypeParser";
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(itemRarity)
                .WithIdentified(isIdentified)
                .WithName("Dig Map")
                .BuildLines();

            this.itemTypeParserMock.Setup(x => x.ParseType(itemStringLines, itemRarity, isIdentified))
                .Returns(expected);

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseBlightedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithType($"{Resources.BlightedPrefix} Dig Map")
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsBlighted);
        }

        [Test]
        public void ParseShouldParseSuperiorBlightedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithType($"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} Dig Map")
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsBlighted);
        }

        [Test]
        public void ParseShouldParseCorruptedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithCorrupted()
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseNormalRarityMap()
        {
            string expectedName = $"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} Dig Map";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName(expectedName)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void ParseShouldCallParseOnMapItemStatsParserIfMapIsIdentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                                        .WithRarity(ItemRarity.Normal)
                                        .WithType("Thicket Map")
                                        .BuildLines();

            this.mapItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines));
        }

        [Test]
        public void ParseShouldNotCallParseOnMapItemStatsParserIfMapIsUnidentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                                        .WithRarity(ItemRarity.Normal)
                                        .WithType("Thicket Map")
                                        .WithUnidentified()
                                        .BuildLines();

            this.mapItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines), Times.Never);
        }

        [Test]
        public void ParseShouldSetStatsOnMapItemFromStatsDataService()
        {
            ItemStats expected = new ItemStats();
            string[] itemStringLines = this.mapItemStringBuilder
                            .WithRarity(ItemRarity.Normal)
                            .WithType("Thicket Map")
                            .BuildLines();

            this.itemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>()))
                .Returns(expected);

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Stats, Is.SameAs(expected));
        }
    }
}