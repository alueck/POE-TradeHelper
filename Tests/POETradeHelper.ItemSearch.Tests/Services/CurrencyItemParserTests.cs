using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services;

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
            this.itemStringBuilder = new ItemStringBuilder();
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
            string itemString = this.itemStringBuilder
                .WithRarity(rarity)
                .Build();

            bool result = this.currencyItemParser.CanParse(itemString);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}