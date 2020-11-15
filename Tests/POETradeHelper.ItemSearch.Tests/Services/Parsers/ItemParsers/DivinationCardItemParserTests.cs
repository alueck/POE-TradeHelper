using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class DivinationCardItemParserTests : ItemParserTestsBase
    {
        private const string DivinationCard = "The Fox";
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.ItemParser = new DivinationCardItemParser();
            this.itemStringBuilder = new ItemStringBuilder().WithRarity(ItemRarity.DivinationCard);
        }

        [TestCase(ItemRarity.DivinationCard, true)]
        [TestCase(ItemRarity.Gem, false)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        [TestCase(ItemRarity.Currency, false)]
        public void CanParseShouldReturnTrueIfRarityIsGem(ItemRarity rarity, bool expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(rarity)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            DivinationCardItem result = this.ItemParser.Parse(itemStringLines) as DivinationCardItem;

            Assert.That(result.Name, Is.EqualTo(DivinationCard));
        }

        [Test]
        public void ParseShouldParseType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            DivinationCardItem result = this.ItemParser.Parse(itemStringLines) as DivinationCardItem;

            Assert.That(result.Type, Is.EqualTo(DivinationCard));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                        .WithName(DivinationCard)
                        .BuildLines();
        }
    }
}