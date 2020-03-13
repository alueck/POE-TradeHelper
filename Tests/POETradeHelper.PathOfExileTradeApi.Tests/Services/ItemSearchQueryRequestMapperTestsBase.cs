using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public abstract class ItemSearchQueryRequestMapperTestsBase<TItemType>
        where TItemType : Item
    {
        public IItemSearchQueryRequestMapper ItemSearchQueryRequestMapper { get; set; }

        [Test]
        public void CanMapShouldReturnTrueForEquippableItem()
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