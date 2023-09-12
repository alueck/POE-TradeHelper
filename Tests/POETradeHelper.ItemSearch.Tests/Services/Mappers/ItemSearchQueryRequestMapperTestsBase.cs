﻿using Microsoft.Extensions.Options;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public abstract class ItemSearchQueryRequestMapperTestsBase<TItemType>
        where TItemType : Item
    {
        protected ItemSearchQueryRequestMapperTestsBase()
        {
            this.ItemSearchOptionsMock = Substitute.For<IOptionsMonitor<ItemSearchOptions>>();
            this.ItemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    League = new League(),
                });
        }

        public IOptionsMonitor<ItemSearchOptions> ItemSearchOptionsMock { get; }

        protected IItemSearchQueryRequestMapper ItemSearchQueryRequestMapper { get; set; } = null!;

        [Test]
        public void MapShouldMapLeague()
        {
            const string expectedLeague = "TestLeague";
            Item item = GetItems().First(item => item.GetType() == typeof(TItemType));
            this.ItemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    League = new League
                    {
                        Id = expectedLeague,
                    },
                });

            SearchQueryRequest queryRequest = this.ItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(queryRequest.League, Is.EqualTo(expectedLeague));
        }

        [Test]
        public void CanMapShouldReturnTrueForMatchingItemType()
        {
            Item item = GetItems().First(item => item.GetType() == typeof(TItemType));

            bool result = this.ItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(GetNonMatchingItems))]
        public void CanMapShouldReturnFalseForOtherItems(Item nonEquippableItem)
        {
            bool result = this.ItemSearchQueryRequestMapper.CanMap(nonEquippableItem);

            Assert.IsFalse(result);
        }

        protected static IEnumerable<Item> GetNonMatchingItems() => GetItems().Where(item => item.GetType() != typeof(TItemType));

        protected static IEnumerable<Item> GetItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new FlaskItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new GemItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
        }

        protected static IEnumerable<ItemRarity> GetNonUniqueItemRarities()
        {
            yield return ItemRarity.Normal;
            yield return ItemRarity.Magic;
            yield return ItemRarity.Rare;
        }
    }
}