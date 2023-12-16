using FluentAssertions;
using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class OrganItemParserTests : ItemParserTestsBase
    {
        private readonly IItemStatsParser<OrganItem> itemStatsParserMock;
        private readonly ItemStringBuilder itemStringBuilder;

        public OrganItemParserTests()
        {
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<OrganItem>>();
            this.ItemParser = new OrganItemParser(this.itemStatsParserMock);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemDescriptionContainsOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithDescription(Resources.OrganItemDescriptor)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemDescriptionDoesNotContainOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                .WithType(expected)
                .BuildLines();

            ItemWithStats result = (ItemWithStats)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                .WithType(expected)
                .BuildLines();

            ItemWithStats result = (ItemWithStats)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(expected);
        }

        [Test]
        public void ParseShouldCallParseOnOrganItemStatsParser()
        {
            const string expected = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Oriath's Virtue's Eye")
                .WithItemLevel(73)
                .WithItemStat(expected, StatCategory.Monster)
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, false);
        }

        [Test]
        public void ParseShouldReturnOrganItemWithStatsSetFromOrganItemStatsParser()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Oriath's Virtue's Eye")
                .BuildLines();

            var expectedOrganItemStats = new ItemStats();

            this.itemStatsParserMock.Parse(Arg.Any<string[]>(), Arg.Any<bool>())
                .Returns(expectedOrganItemStats);

            ItemWithStats result = (ItemWithStats)this.ItemParser.Parse(itemStringLines);

            result.Stats.Should().BeSameAs(expectedOrganItemStats);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithType("Oriath's Virtue's Eye")
                .WithItemLevel(73)
                .WithItemStat("Drops additional currency", StatCategory.Monster)
                .BuildLines();
        }
    }
}