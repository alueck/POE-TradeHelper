﻿using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApiTests.Services
{
    public class EquippableItemSearchQueryRequestMapperTests
    {
        private Mock<IOptions<ItemSearchOptions>> itemSearchOptionsMock;
        private EquippableItemSearchQueryRequestMapper equippableItemToQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.itemSearchOptionsMock = new Mock<IOptions<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.Value)
                .Returns(new ItemSearchOptions());

            this.equippableItemToQueryRequestMapper = new EquippableItemSearchQueryRequestMapper(this.itemSearchOptionsMock.Object);
        }

        [Test]
        public void CanMapShouldReturnTrueForEquippableItem()
        {
            var item = new EquippableItem(ItemRarity.Normal);

            bool result = this.equippableItemToQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonEquippableItems))]
        public void CanMapShouldReturnFalseForOtherItems(Item nonEquippableItem)
        {
            bool result = this.equippableItemToQueryRequestMapper.CanMap(nonEquippableItem);

            Assert.IsFalse(result);
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Thicket Bow";
            var item = new EquippableItem(ItemRarity.Normal)
            {
                Type = expectedType
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Dire Nock";
            var item = new EquippableItem(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            var item = new EquippableItem(ItemRarity.Unique)
            {
                Type = "Thicket Bow"
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new EquippableItem(itemRarity)
            {
                Name = "Dire Nock"
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCase(5)]
        [TestCase(6)]
        public void MapToQueryItemShouldMapLinksIfLinkCountIsMinimumFive(int linkCount)
        {
            var item = new EquippableItem(ItemRarity.Normal)
            {
                Sockets = GetLinkedItemSockets(linkCount)
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.SocketFilters);
            Assert.NotNull(result.Query.Filters.SocketFilters.Links);

            SocketsFilter socketsFilter = result.Query.Filters.SocketFilters.Links;
            Assert.That(socketsFilter.Min, Is.EqualTo(linkCount));
            Assert.That(socketsFilter.Max, Is.EqualTo(linkCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void MapToQueryItemShoulNotdMapLinksIfLinkCountIsLowerThanFive(int linkCount)
        {
            var item = new EquippableItem(ItemRarity.Normal)
            {
                Sockets = GetLinkedItemSockets(linkCount)
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Filters.SocketFilters.Links);
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            var item = new EquippableItem(ItemRarity.Unique);

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.Unique));
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new EquippableItem(itemRarity);

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.NonUnique));
        }

        [TestCaseSource(nameof(InfluenceTestCases))]
        public void MapQueryItemShouldMapInfluence(InfluenceType influenceType, BoolOptionAccessor filterOptionAccessor)
        {
            var item = new EquippableItem(ItemRarity.Normal)
            {
                Influence = influenceType
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Filters.MiscFilters.Filters.Count, Is.EqualTo(1));
            Assert.IsTrue(filterOptionAccessor(result).Option);
        }

        [Test]
        public void MapQueryItemShouldMapItemLevelIfThresholdIsReached()
        {
            const int itemLevel = 86;
            var item = new EquippableItem(ItemRarity.Normal)
            {
                ItemLevel = itemLevel
            };

            this.itemSearchOptionsMock.Setup(x => x.Value)
                .Returns(new ItemSearchOptions
                {
                    ItemLevelThreshold = itemLevel
                });

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter itemLevelFilter = result.Query.Filters.MiscFilters.ItemLevel;
            Assert.NotNull(itemLevelFilter);
            Assert.That(itemLevelFilter.Min, Is.EqualTo(itemLevel));
            Assert.IsNull(itemLevelFilter.Max);
        }

        [Test]
        public void MapQueryItemShouldNotMapItemLevelIfThresholdIsNotReached()
        {
            const int itemLevel = 86;
            var item = new EquippableItem(ItemRarity.Normal)
            {
                ItemLevel = itemLevel - 1
            };

            this.itemSearchOptionsMock.Setup(x => x.Value)
                .Returns(new ItemSearchOptions
                {
                    ItemLevelThreshold = itemLevel
                });

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Filters.MiscFilters.ItemLevel);
        }

        public delegate BoolOptionFilter BoolOptionAccessor(SearchQueryRequest searchQueryRequest);

        private static IEnumerable InfluenceTestCases
        {
            get
            {
                yield return new TestCaseData(InfluenceType.Crusader, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.CrusaderItem));
                yield return new TestCaseData(InfluenceType.Elder, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.ElderItem));
                yield return new TestCaseData(InfluenceType.Hunter, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.HunterItem));
                yield return new TestCaseData(InfluenceType.Redeemer, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.RedeemerItem));
                yield return new TestCaseData(InfluenceType.Shaper, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.ShaperItem));
                yield return new TestCaseData(InfluenceType.Warlord, (BoolOptionAccessor)((SearchQueryRequest result) => result.Query.Filters.MiscFilters.WarlordItem));
            }
        }

        private static ItemSockets GetLinkedItemSockets(int linkCount)
        {
            var result = new ItemSockets();

            if (linkCount > 0)
            {
                var socketGroup = new SocketGroup();

                for (int i = 0; i < linkCount; i++)
                {
                    socketGroup.Sockets.Add(new Socket());
                }

                result.SocketGroups.Add(socketGroup);
            }

            return result;
        }

        private static IEnumerable<Item> NonEquippableItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
            }
        }

        private static IEnumerable<ItemRarity> NonUniqueItemRarities
        {
            get
            {
                yield return ItemRarity.Normal;
                yield return ItemRarity.Magic;
                yield return ItemRarity.Rare;
            }
        }
    }
}