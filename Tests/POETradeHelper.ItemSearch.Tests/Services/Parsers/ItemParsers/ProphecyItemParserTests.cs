using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class ProphecyItemParserTests
    {
        private ProphecyItemParser prophecyItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.prophecyItemParser = new ProphecyItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemStringContainsProphecyKeyword()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription($"Right-click to add this {Resources.ProphecyKeyword} to your character.")
                                            .BuildLines();

            bool result = this.prophecyItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemStringDoesNotContainProphecyKeyword()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription("Random description")
                                            .BuildLines();

            bool result = this.prophecyItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "The Unbreathing Queen I";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithName(expected)
                                            .BuildLines();

            ProphecyItem result = this.prophecyItemParser.Parse(itemStringLines) as ProphecyItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "The Unbreathing Queen I";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithName(expected)
                                            .BuildLines();

            ProphecyItem result = this.prophecyItemParser.Parse(itemStringLines) as ProphecyItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }
    }
}