using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class FragmentItemParserTests : ItemParserTestsBase
    {
        private const string Fragment = "Offering to the Goddess";
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.ItemParser = new FragmentItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemIsNormalRarityAndHasNoItemLevelOrProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButHasItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithItemLevel(10)
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithDescription($"Right-click to add this {Resources.ProphecyKeyword} to your character.")
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldParseName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            FragmentItem result = this.ItemParser.Parse(itemStringLines) as FragmentItem;

            Assert.That(result.Name, Is.EqualTo(Fragment));
        }

        [Test]
        public void ParseShouldParseType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            FragmentItem result = this.ItemParser.Parse(itemStringLines) as FragmentItem;

            Assert.That(result.Type, Is.EqualTo(Fragment));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                        .WithName(Fragment)
                        .BuildLines();
        }
    }
}