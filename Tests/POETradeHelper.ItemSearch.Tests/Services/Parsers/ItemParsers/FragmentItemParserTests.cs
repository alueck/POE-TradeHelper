using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class FragmentItemParserTests
    {
        private FragmentItemParser prophecyItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.prophecyItemParser = new FragmentItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemIsNormalRarityAndHasNoItemLevelOrProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .BuildLines();

            bool result = this.prophecyItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButHasItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithItemLevel(10)
                                            .BuildLines();

            bool result = this.prophecyItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithDescription($"Right-click to add this {Resources.ProphecyKeyword} to your character.")
                                            .BuildLines();

            bool result = this.prophecyItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "Offering to the Goddess";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithName(expected)
                                            .BuildLines();

            FragmentItem result = this.prophecyItemParser.Parse(itemStringLines) as FragmentItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "Offering to the Goddess";
            string[] itemStringLines = this.itemStringBuilder
                                            .WithName(expected)
                                            .BuildLines();

            FragmentItem result = this.prophecyItemParser.Parse(itemStringLines) as FragmentItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }
    }
}