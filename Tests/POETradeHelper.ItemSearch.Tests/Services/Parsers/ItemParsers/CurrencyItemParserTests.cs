using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    internal class CurrencyItemParserTests : ItemParserTestsBase
    {
        private const string Currency = "Scroll of Wisdom";
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.ItemParser = new CurrencyItemParser();
            this.itemStringBuilder = new ItemStringBuilder().WithRarity(ItemRarity.Currency);
        }

        [TestCase(ItemRarity.Currency, true)]
        [TestCase(ItemRarity.Gem, false)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        [TestCase(ItemRarity.DivinationCard, false)]
        public void CanParseShouldReturnTrueIfRarityIsCurrency(ItemRarity rarity, bool expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(rarity)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnCurrencyItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            Assert.That(item, Is.InstanceOf<CurrencyItem>());
        }

        [Test]
        public void ParseShouldParseCurrencyName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            Assert.That(item.Name, Is.EqualTo(Currency));
        }

        [Test]
        public void ParseShouldParseCurrencyType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            Assert.That(item.Type, Is.EqualTo(Currency));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithName(Currency)
                .BuildLines();
        }
    }
}