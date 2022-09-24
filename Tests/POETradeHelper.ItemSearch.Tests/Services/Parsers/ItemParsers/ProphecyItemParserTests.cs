using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class ProphecyItemParserTests : ItemParserTestsBase
    {
        private const string Prophecy = "The Unbreathing Queen I";
        private readonly ItemStringBuilder itemStringBuilder;

        public ProphecyItemParserTests()
        {
            this.ItemParser = new ProphecyItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemStringContainsProphecyKeyword()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription($"Right-click to add this {Resources.ProphecyKeyword} to your character.")
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemStringDoesNotContainProphecyKeyword()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithDescription("Random description")
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            ProphecyItem result = (ProphecyItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Name, Is.EqualTo(Prophecy));
        }

        [Test]
        public void ParseShouldParseType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            ProphecyItem result = (ProphecyItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Type, Is.EqualTo(Prophecy));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                        .WithName(Prophecy)
                        .WithDescription("Random description")
                        .BuildLines();
        }
    }
}