using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services
{
    internal class CurrencyItemParserTests
    {
        private CurrencyItemParser currencyItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.currencyItemParser = new CurrencyItemParser();
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

            bool result = this.currencyItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnCurrencyItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.currencyItemParser.Parse(itemStringLines);

            Assert.That(item, Is.InstanceOf<CurrencyItem>());
        }

        [Test]
        public void ParseShouldParseCurrencyName()
        {
            string expected = "Scroll of Wisdom";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            Item item = this.currencyItemParser.Parse(itemStringLines);

            Assert.That(item.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCurrencyType()
        {
            string expected = "Scroll of Wisdom";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            Item item = this.currencyItemParser.Parse(itemStringLines);

            Assert.That(item.Type, Is.EqualTo(expected));
        }
    }
}