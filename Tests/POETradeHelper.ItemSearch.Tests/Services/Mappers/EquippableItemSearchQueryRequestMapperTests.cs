using System.Collections;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class EquippableItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<EquippableItem>
    {
        private readonly EquippableItemSearchQueryRequestMapper equippableItemToQueryRequestMapper;

        public EquippableItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.equippableItemToQueryRequestMapper =
                new EquippableItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        public delegate BoolOptionFilter? BoolOptionAccessor(SearchQueryRequest searchQueryRequest);

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Dire Nock";
            EquippableItem item = new(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true,
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            EquippableItem item = new(ItemRarity.Unique)
            {
                Type = "Thicket Bow",
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            EquippableItem item = new(itemRarity)
            {
                Name = "Dire Nock",
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCase(5)]
        [TestCase(6)]
        public void MapToQueryItemShouldMapLinksIfLinkCountIsMinimumFive(int linkCount)
        {
            EquippableItem item = new(ItemRarity.Normal)
            {
                Sockets = GetLinkedItemSockets(linkCount),
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.SocketFilters);
            Assert.NotNull(result.Query.Filters.SocketFilters.Links);

            SocketsFilter? socketsFilter = result.Query.Filters.SocketFilters.Links;
            Assert.That(socketsFilter, Is.Not.Null);
            Assert.That(socketsFilter!.Min, Is.EqualTo(linkCount));
            Assert.That(socketsFilter.Max, Is.EqualTo(linkCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void MapToQueryItemShoulNotMapLinksIfLinkCountIsLowerThanFive(int linkCount)
        {
            EquippableItem item = new(ItemRarity.Normal)
            {
                Sockets = GetLinkedItemSockets(linkCount),
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Filters.SocketFilters.Links);
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            EquippableItem item = new(ItemRarity.Unique);

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity!.Option, Is.EqualTo(ItemRarityFilterOptions.Unique));
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            EquippableItem item = new(itemRarity);

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity!.Option, Is.EqualTo(ItemRarityFilterOptions.NonUnique));
        }

        [TestCaseSource(nameof(GetInfluenceTestCases))]
        public void MapQueryItemShouldMapInfluence(InfluenceType influenceType, BoolOptionAccessor filterOptionAccessor)
        {
            EquippableItem item = new(ItemRarity.Normal)
            {
                Influence = influenceType,
            };

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            BoolOptionFilter? filter = filterOptionAccessor(result);

            Assert.That(filter, Is.Not.Null);
            Assert.That(filter!.Option, Is.True);
        }

        [Test]
        public void MapQueryItemShouldMapItemLevelIfThresholdIsReached()
        {
            const int itemLevel = 86;
            EquippableItem item = new(ItemRarity.Normal)
            {
                ItemLevel = itemLevel,
            };

            this.ItemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    ItemLevelThreshold = itemLevel,
                    League = new League(),
                });

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? itemLevelFilter = result.Query.Filters.MiscFilters.ItemLevel;
            Assert.NotNull(itemLevelFilter);
            Assert.That(itemLevelFilter!.Min, Is.EqualTo(itemLevel));
            Assert.IsNull(itemLevelFilter.Max);
        }

        [Test]
        public void MapQueryItemShouldNotMapItemLevelIfThresholdIsNotReached()
        {
            const int itemLevel = 86;
            EquippableItem item = new(ItemRarity.Normal)
            {
                ItemLevel = itemLevel - 1,
            };

            this.ItemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    ItemLevelThreshold = itemLevel,
                    League = new League(),
                });

            SearchQueryRequest result = this.equippableItemToQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Filters.MiscFilters.ItemLevel);
        }

        private static IEnumerable GetInfluenceTestCases()
        {
            yield return new TestCaseData(InfluenceType.Crusader, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.CrusaderItem));
            yield return new TestCaseData(InfluenceType.Elder, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.ElderItem));
            yield return new TestCaseData(InfluenceType.Hunter, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.HunterItem));
            yield return new TestCaseData(InfluenceType.Redeemer, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.RedeemerItem));
            yield return new TestCaseData(InfluenceType.Shaper, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.ShaperItem));
            yield return new TestCaseData(InfluenceType.Warlord, (BoolOptionAccessor)(result => result.Query.Filters.MiscFilters.WarlordItem));
        }

        private static ItemSockets GetLinkedItemSockets(int linkCount)
        {
            ItemSockets result = new();

            if (linkCount > 0)
            {
                SocketGroup socketGroup = new();

                for (int i = 0; i < linkCount; i++)
                {
                    socketGroup.Sockets.Add(new Socket());
                }

                result.SocketGroups.Add(socketGroup);
            }

            AddSocketsTillMaxSocketCount(linkCount, result);

            return result;
        }

        private static void AddSocketsTillMaxSocketCount(int linkCount, ItemSockets result)
        {
            if (linkCount < 6)
            {
                for (int i = 6 - linkCount; i > 0; i--)
                {
                    result.SocketGroups.Add(new SocketGroup
                    {
                        Sockets =
                        {
                            new Socket(),
                        },
                    });
                }
            }
        }
    }
}