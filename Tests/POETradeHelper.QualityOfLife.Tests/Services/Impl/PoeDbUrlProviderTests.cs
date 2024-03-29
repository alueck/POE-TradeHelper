﻿using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services.Impl;

namespace POETradeHelper.QualityOfLife.Tests.Services.Impl
{
    public class PoeDbUrlProviderTests
    {
        private const string RootUrl = "https://poedb.tw/us/";

        private readonly PoeDbUrlProvider wikiUrlProvider;

        public PoeDbUrlProviderTests()
        {
            this.wikiUrlProvider = new PoeDbUrlProvider();
        }

        [Test]
        public void WikiTyperReturnsPoeDb()
        {
            WikiType result = this.wikiUrlProvider.HandledWikiType;

            result.Should().Be(WikiType.PoeDb);
        }

        [TestCaseSource(nameof(GetNonUniqueItems))]
        public void GetUrlReturnsCorrectUrlForNonUniqueItems(Item item)
        {
            Uri expected = new(RootUrl + item.Type.Replace(' ', '_').Replace("'", string.Empty));

            Uri result = this.wikiUrlProvider.GetUrl(item);

            result.Should().Be(expected);
        }

        [Test]
        public void GetUrlReturnsCorrectUrlForUniqueItem()
        {
            EquippableItem item = new(ItemRarity.Unique)
            {
                Name = "Thousand Ribbons",
            };
            Uri expected = new(RootUrl + item.Name.Replace(' ', '_').Replace("'", string.Empty));

            Uri result = this.wikiUrlProvider.GetUrl(item);

            result.Should().Be(expected);
        }

        private static IEnumerable<Item> GetNonUniqueItems()
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
}