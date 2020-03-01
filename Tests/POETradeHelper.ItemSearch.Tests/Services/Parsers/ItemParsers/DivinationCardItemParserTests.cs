using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class DivinationCardItemParserTests
    {
        private DivinationCardItemParser divinationCardItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.divinationCardItemParser = new DivinationCardItemParser();
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

            bool result = this.divinationCardItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseName()
        {
            const string expected = "The Fox";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            DivinationCardItem result = this.divinationCardItemParser.Parse(itemStringLines) as DivinationCardItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseType()
        {
            const string expected = "The Fox";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            DivinationCardItem result = this.divinationCardItemParser.Parse(itemStringLines) as DivinationCardItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }
    }
}