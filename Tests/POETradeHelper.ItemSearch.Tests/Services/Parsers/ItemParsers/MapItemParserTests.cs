using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class MapItemParserTests : ItemParserTestsBase
    {
        private readonly IItemTypeParser itemTypeParserMock;
        private readonly IItemStatsParser<ItemWithStats> itemStatsParserMock;
        private readonly MapItemStringBuilder mapItemStringBuilder;

        public MapItemParserTests()
        {
            this.itemTypeParserMock = Substitute.For<IItemTypeParser>();
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<ItemWithStats>>();
            this.ItemParser = new MapItemParser(this.itemTypeParserMock, this.itemStatsParserMock);
            this.mapItemStringBuilder = new MapItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemContainsMapTier()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

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

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMapTier()
        {
            const int expected = 10;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Tier, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemQuantity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemQuantity(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.ItemQuantity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemRarity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemRarity(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.ItemRarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMonsterPackSize()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMonsterPackSize(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.MonsterPackSize, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ParseShouldParseIdentified(bool identified)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithIdentified(identified)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.IsIdentified, Is.EqualTo(identified));
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

            this.itemTypeParserMock.ParseType(itemStringLines, itemRarity, isIdentified)
                .Returns(expected);

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [TestCase("Blighted Dig Map", true)]
        [TestCase("Dig Map", false)]
        [TestCase("Blight-ravaged Dig Map", false)]
        public void ParseShouldParseBlightedMap(string type, bool expected)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithType(type)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.IsBlighted, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseSuperiorBlightedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithType($"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} Dig Map")
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.IsTrue(result.IsBlighted);
        }

        [TestCase("Blight-ravaged Dig Map", true)]
        [TestCase("Dig Map", false)]
        [TestCase("Blighted Dig Map", false)]
        public void ParseShouldParseBlightRavagedMap(string type, bool expected)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithType(type)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.IsBlightRavaged, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ParseShouldParseCorruptedMap(bool isCorrupted)
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithCorrupted(isCorrupted)
                .BuildLines();

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.IsCorrupted, Is.EqualTo(isCorrupted));
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

            Assert.That(result.Name, Is.EqualTo(expectedName));
        }

        [Test]
        public void ParseShouldCallParseOnMapItemStatsParserIfMapIsIdentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithType("Thicket Map")
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
                .WithType("Thicket Map")
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
                .WithType("Thicket Map")
                .BuildLines();

            this.itemStatsParserMock.Parse(Arg.Any<string[]>(), Arg.Any<bool>())
                .Returns(expected);

            MapItem result = (MapItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Stats, Is.SameAs(expected));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithType("Thicket Map")
                .WithUnidentified()
                .BuildLines();
        }
    }
}