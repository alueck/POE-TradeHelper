using System;
using System.Collections.Generic;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services.Impl;

namespace POETradeHelper.QualityOfLife.Tests.Services.Impl
{
    public class PoeDbUrlProviderTests
    {
        private const string RootUrl = "https://poedb.tw/us/";

        private PoeDbUrlProvider wikiUrlProvider;

        [SetUp]
        public void Setup()
        {
            this.wikiUrlProvider = new PoeDbUrlProvider();
        }

        [Test]
        public void WikiTyperReturnsPoeDb()
        {
            var result = this.wikiUrlProvider.HandledWikiType;

            Assert.That(result, Is.EqualTo(WikiType.PoeDb));
        }

        [TestCaseSource(nameof(NonUniqueItems))]
        public void GetUrlReturnsCorrectUrlForNonUniqueItems(Item item)
        {
            var expected = new Uri(RootUrl + item.Type.Replace(' ', '_').Replace("'", ""));

            var result = this.wikiUrlProvider.GetUrl(item);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<Item> NonUniqueItems
        {
            get
            {
                yield return new EquippableItem(ItemRarity.Normal) { Type = "Soldier's Brigandine" };
                yield return new EquippableItem(ItemRarity.Magic) { Type = "Soldier's Brigandine" };
                yield return new EquippableItem(ItemRarity.Rare) { Type = "Soldier's Brigandine" };
                yield return new CurrencyItem { Type = "Scroll of Wisdom" };
                yield return new GemItem { Type = "Portal" };
                yield return new FragmentItem { Type = "Sacrifice at Dusk" };
                yield return new DivinationCardItem { Type = "The Summoner" };
            }
        }

        [Test]
        public void GetUrlReturnsCorrectUrlForUniqueItem()
        {
            var item = new EquippableItem(ItemRarity.Unique)
            {
                Name = "Thousand Ribbons"
            };
            var expected = new Uri(RootUrl + item.Name.Replace(' ', '_').Replace("'", ""));

            var result = this.wikiUrlProvider.GetUrl(item);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUrlThrowsArgumentNullExceptionIfItemIsNull()
        {
            void Action() => this.wikiUrlProvider.GetUrl(null);

            Assert.Throws<ArgumentNullException>(Action);
        }
    }
}