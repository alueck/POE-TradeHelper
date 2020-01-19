using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class OrganItemParserTests
    {
        private OrganItemParser organItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.organItemParser = new OrganItemParser();
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
    }
}