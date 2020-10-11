using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class OrganItemParserTests : ItemParserTestsBase
    {
        private Mock<IItemStatsParser<OrganItem>> itemStatsParserMock;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.itemStatsParserMock = new Mock<IItemStatsParser<OrganItem>>();
            this.ItemParser = new OrganItemParser(this.itemStatsParserMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemDescriptionContainsOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription(Resources.OrganItemDescriptor)
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemDescriptionDoesNotContainOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithType(expected)
                                            .BuildLines();

            ItemWithStats result = this.ItemParser.Parse(itemStringLines) as ItemWithStats;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithType(expected)
                                            .BuildLines();

            ItemWithStats result = this.ItemParser.Parse(itemStringLines) as ItemWithStats;

            Assert.That(result.Type, Is.EqualTo(expected));
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

            ItemWithStats result = this.ItemParser.Parse(itemStringLines) as ItemWithStats;

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines, false));
        }

        [Test]
        public void ParseShouldReturnOrganItemWithStatsSetFromOrganItemStatsParser()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .BuildLines();

            var expectedOrganItemStats = new ItemStats();

            this.itemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>(), It.IsAny<bool>()))
                .Returns(expectedOrganItemStats);

            ItemWithStats result = this.ItemParser.Parse(itemStringLines) as ItemWithStats;

            Assert.That(result.Stats, Is.SameAs(expectedOrganItemStats));
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