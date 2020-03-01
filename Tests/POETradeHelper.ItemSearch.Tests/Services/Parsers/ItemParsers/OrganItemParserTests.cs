using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class OrganItemParserTests
    {
        private Mock<IOrganItemStatsParser> organItemStatsParserMock;
        private OrganItemParser organItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.organItemStatsParserMock = new Mock<IOrganItemStatsParser>();
            this.organItemParser = new OrganItemParser(this.organItemStatsParserMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemDescriptionContainsOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription(Resources.OrganItemDescriptor)
                                            .BuildLines();

            bool result = this.organItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemDescriptionDoesNotContainOrganText()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            bool result = this.organItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithType(expected)
                                            .BuildLines();

            OrganItem result = this.organItemParser.Parse(itemStringLines) as OrganItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "Oriath's Virtue's Eye";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithType(expected)
                                            .BuildLines();

            OrganItem result = this.organItemParser.Parse(itemStringLines) as OrganItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallParseOnOrganItemStatsParser()
        {
            const string expected = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat(expected)
                                .BuildLines();

            OrganItem result = this.organItemParser.Parse(itemStringLines) as OrganItem;

            this.organItemStatsParserMock.Verify(x => x.Parse(itemStringLines));
        }

        [Test]
        public void ParseShouldReturnOrganItemWithStatsSetFromOrganItemStatsParser()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .BuildLines();

            var expectedOrganItemStats = new OrganItemStats();

            this.organItemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>()))
                .Returns(expectedOrganItemStats);

            OrganItem result = this.organItemParser.Parse(itemStringLines) as OrganItem;

            Assert.That(result.Stats, Is.SameAs(expectedOrganItemStats));
        }
    }
}