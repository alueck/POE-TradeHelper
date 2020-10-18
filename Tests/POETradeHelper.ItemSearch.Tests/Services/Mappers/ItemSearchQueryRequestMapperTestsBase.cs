using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public abstract class ItemSearchQueryRequestMapperTestsBase<TItemType>
        where TItemType : Item
    {
        public IItemSearchQueryRequestMapper ItemSearchQueryRequestMapper { get; set; }

        public Mock<IOptionsMonitor<ItemSearchOptions>> ItemSearchOptionsMock { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            this.ItemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.ItemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League()
                });
        }

        [Test]
        public void MapShouldMapLeague()
        {
            const string expectedLeague = "TestLeague";
            var item = Items.First(item => item.GetType() == typeof(TItemType));
            this.ItemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League
                    {
                        Id = expectedLeague
                    }
                });

            PathOfExileTradeApi.Models.IQueryRequest queryRequest = this.ItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(queryRequest.League, Is.EqualTo(expectedLeague));
        }

        [Test]
        public void CanMapShouldReturnTrueForMatchingItemType()
        {
            var item = Items.First(item => item.GetType() == typeof(TItemType));

            bool result = this.ItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonMatchingItems))]
        public void CanMapShouldReturnFalseForOtherItems(Item nonEquippableItem)
        {
            bool result = this.ItemSearchQueryRequestMapper.CanMap(nonEquippableItem);

            Assert.IsFalse(result);
        }

        public static IEnumerable<Item> NonMatchingItems => Items.Where(item => item.GetType() != typeof(TItemType));

        public static IEnumerable<Item> Items
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
                yield return new JewelItem(ItemRarity.Magic);
                yield return new EquippableItem(ItemRarity.Magic);
            }
        }

        public static IEnumerable<ItemRarity> NonUniqueItemRarities
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