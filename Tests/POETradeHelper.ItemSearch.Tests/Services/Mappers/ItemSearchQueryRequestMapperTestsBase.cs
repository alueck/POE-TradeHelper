using AwesomeAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

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

            SearchQueryRequest result = this.ItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.League.Should().BeEquivalentTo(expectedLeague);
        }

        [Test]
        public void CanMapShouldReturnTrueForMatchingItemType()
        {
            Item item = GetItems().First(item => item.GetType() == typeof(TItemType));

            bool result = this.ItemSearchQueryRequestMapper.CanMap(item);

            result.Should().BeTrue();
        }

        [TestCaseSource(nameof(GetNonMatchingItems))]
        public void CanMapShouldReturnFalseForOtherItems(Item nonEquippableItem)
        {
            bool result = this.ItemSearchQueryRequestMapper.CanMap(nonEquippableItem);

            result.Should().BeFalse();
        }

        [Test]
        public void MapShouldMapHighTierStats()
        {
            if (!this.MapsHighTierItemStats())
            {
                Assert.Ignore("Item has no Tier 1 stats.");
            }

            ItemWithStats item = (ItemWithStats)GetItems().First(item => item.GetType() == typeof(TItemType));
            MinMaxValueItemStat minMaxValueItemStat = new(StatCategory.Explicit) { Id = "MinMaxValueStat", Tier = 1, MinValue = 2, MaxValue = 5 };
            SingleValueItemStat singleValueItemStat = new(StatCategory.Explicit) { Id = "SingleValueStat", Tier = 1, Value = 4 };
            SingleValueItemStat highTierCrucibleStat = new(StatCategory.Crucible) { Id ="CrucibleStat", Tier = 5, Value = 3 };

            item.Stats = new ItemStats
            {
                AllStats =
                {
                    minMaxValueItemStat,
                    singleValueItemStat,
                    highTierCrucibleStat,
                    new MinMaxValueItemStat(StatCategory.Explicit) { Id = "Tier2MinMaxValueStat", Tier = 2 },
                    new SingleValueItemStat(StatCategory.Explicit) { Id = "Tier2SingleValueStat", Tier = 2 },
                    new SingleValueItemStat(StatCategory.Crucible) { Id = "Tier1CrucibleStat", Tier = 1 },
                },
            };

            SearchQueryRequest result = this.ItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Stats.Should().ContainSingle();
            result.Query.Stats[0].Filters.Should().SatisfyRespectively(
                x => x.Should().BeEquivalentTo(new StatFilter
                {
                    Id = minMaxValueItemStat.Id,
                    Value = new MinMaxFilter { Min = minMaxValueItemStat.MinValue, Max = minMaxValueItemStat.MaxValue },
                }),
                x => x.Should().BeEquivalentTo(new StatFilter
                {
                    Id = singleValueItemStat.Id,
                    Value = new MinMaxFilter { Min = singleValueItemStat.Value },
                }),
                x => x.Should().BeEquivalentTo(new StatFilter
                {
                    Id = highTierCrucibleStat.Id,
                    Value = new MinMaxFilter{ Min = highTierCrucibleStat.Value },
                }));
        }

        protected virtual bool MapsHighTierItemStats() => typeof(TItemType).IsAssignableTo(typeof(ItemWithStats)) && !typeof(TItemType).IsAssignableTo(typeof(GemItem));

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