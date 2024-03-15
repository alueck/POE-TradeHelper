using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class JewelItemParserTests : ItemParserTestsBase
    {
        private readonly IItemTypeParser itemTypeParserMock;
        private readonly IItemStatsParser<ItemWithStats> itemStatsParserMock;
        private readonly ItemStringBuilder itemStringBuilder;

        public JewelItemParserTests()
        {
            this.itemTypeParserMock = Substitute.For<IItemTypeParser>();
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<ItemWithStats>>();
            this.ItemParser = new JewelItemParser(this.itemTypeParserMock, this.itemStatsParserMock);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Magic)
                .WithType("Pyromantic Cobalt Jewel")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewelForRareJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Rare)
                .WithName("Armageddon Joy")
                .WithType("Cobalt Jewel")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewelForUnidentifiedRareJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Rare)
                .WithType("Cobalt Jewel")
                .WithUnidentified()
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseShouldParseRarity(ItemRarity expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(expected)
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.Rarity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithUnidentified()
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseNameOfMagicJewel()
        {
            const string expected = "Pyromantic Cobalt Jewel";

            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Magic)
                .WithName(expected)
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expected);
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
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(itemRarity)
                .WithIdentified(isIdentified)
                .WithName("Cobalt Jewel")
                .BuildLines();

            this.itemTypeParserMock.ParseType(itemStringLines, itemRarity, isIdentified)
                .Returns(expected);

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(expected);
        }

        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseShouldParseNameOfRareIdentifiedJewel(ItemRarity itemRarity)
        {
            const string expected = "Armageddon Joy";

            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(itemRarity)
                .WithName(expected)
                .WithType("Cobalt Jewel")
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithCorrupted()
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseNotCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            JewelItem result = (JewelItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeFalse();
        }

        [Test]
        public void ParseShouldCallParseOnItemStatsParserIfItemIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, false);
        }

        [Test]
        public void ParseShouldNotCallParseOnItemStatsParserIfItemIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithUnidentified()
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .DidNotReceive()
                .Parse(itemStringLines, false);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithRarity(ItemRarity.Rare)
                .WithName("Armageddon Joy")
                .WithType("Cobalt Jewel")
                .BuildLines();
        }
    }
}