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
    public class FlaskItemParserTests : ItemParserTestsBase
    {
        private readonly IItemTypeParser itemTypeParserMock;
        private readonly IItemStatsParser<ItemWithStats> itemStatsParserMock;
        private readonly ItemStringBuilder itemStringBuilder;

        public FlaskItemParserTests()
        {
            this.itemTypeParserMock = Substitute.For<IItemTypeParser>();
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<ItemWithStats>>();
            this.ItemParser = new FlaskItemParser(this.itemTypeParserMock, this.itemStatsParserMock);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase("Bubbling Divine Life Flask of Staunching")]
        [TestCase("Flask")]
        public void CanParseShouldReturnTrueIfNameContainsFlask(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName(name)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeOfUniqueFlaskContainsFlask()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Unique)
                .WithName("Cinderswallow Urn")
                .WithType("Silver Flask")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnFalseIfNameDoesNotContainFlask()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Scroll of Wisdom")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void ParseShoudParseFlaskRarity()
        {
            const ItemRarity expected = ItemRarity.Unique;
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(expected)
                .BuildLines();

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.Rarity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseFlaskName()
        {
            const string expected = "Bubbling Divine Life Flask of Staunching";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

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
                .WithName("Cinderswallow Urn")
                .BuildLines();

            this.itemTypeParserMock.ParseType(itemStringLines, itemRarity, isIdentified)
                .Returns(expected);

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseFlaskQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Bubbling Divine Life Flask of Staunching")
                .WithQuality(expected)
                .BuildLines();

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.Quality.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseFlaskIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Bubbling Divine Life Flask of Staunching")
                .BuildLines();

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseFlaskUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Bubbling Divine Life Flask of Staunching")
                .WithUnidentified()
                .BuildLines();

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeFalse();
        }

        [Test]
        public void ParseShouldCallParseOnFlaskItemStatsParserIfFlaskIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Divine Life Flask")
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, false);
        }

        [Test]
        public void ParseShouldNotCallParseOnFlaskItemStatsParserIfFlaskIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Divine Life Flask")
                .WithUnidentified()
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .DidNotReceive()
                .Parse(Arg.Any<string[]>(), Arg.Any<bool>());
        }

        [Test]
        public void ParseShouldSetStatsToStatsFromStatsDataService()
        {
            ItemStats expected = new();
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Divine Life Flask")
                .BuildLines();

            this.itemStatsParserMock.Parse(Arg.Any<string[]>(), false)
                .Returns(expected);

            FlaskItem result = (FlaskItem)this.ItemParser.Parse(itemStringLines);

            result.Stats.Should().BeSameAs(expected);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithRarity(ItemRarity.Unique)
                .WithName("Cinderswallow Urn")
                .WithType("Silver Flask")
                .BuildLines();
        }
    }
}