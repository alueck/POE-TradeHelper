using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class AdvancedQueryViewModelFactoryTests
    {
        private Mock<IStatFilterViewModelFactory> statFilterViewModelFactoryMock;
        private AdvancedQueryViewModelFactory advancedQueryViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.statFilterViewModelFactoryMock = new Mock<IStatFilterViewModelFactory>();
            this.advancedQueryViewModelFactory = new AdvancedQueryViewModelFactory(this.statFilterViewModelFactoryMock.Object);
        }

        [Test]
        public void CreateShouldReturnDisabledAdvancedQueryViewModelForExchangeQueryRequest()
        {
            var item = new EquippableItem(ItemRarity.Normal);
            var searchQueryRequest = new ExchangeQueryRequest();

            AdvancedQueryViewModel result = this.advancedQueryViewModelFactory.Create(item, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsEnabled);
        }

        [TestCaseSource(nameof(DisabledItems))]
        public void CreateShouldReturnDisabledAdvancedQueryViewModel(Item item)
        {
            var searchQueryRequest = new SearchQueryRequest();

            AdvancedQueryViewModel result = this.advancedQueryViewModelFactory.Create(item, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsEnabled);
        }

        [TestCaseSource(nameof(EnabledItems))]
        public void CreateShouldReturnEnabledAdvancedQueryViewModel(Item item)
        {
            var searchQueryRequest = new SearchQueryRequest();

            AdvancedQueryViewModel result = this.advancedQueryViewModelFactory.Create(item, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsEnabled);
        }

        [TestCase(StatCategory.Enchant)]
        [TestCase(StatCategory.Implicit)]
        [TestCase(StatCategory.Explicit)]
        [TestCase(StatCategory.Crafted)]
        [TestCase(StatCategory.Monster)]
        public void CreateShouldCallCreateOnStatFilterViewModelFactoryForItemStats(StatCategory statCategory)
        {
            ItemWithStats item = CreateItemWithStats(statCategory);

            SearchQueryRequest queryRequest = new SearchQueryRequest();
            this.advancedQueryViewModelFactory.Create(item, queryRequest);

            foreach (var itemStat in item.Stats.AllStats)
            {
                this.statFilterViewModelFactoryMock.Verify(x => x.Create(itemStat, queryRequest, It.IsAny<StatFilterViewModelFactoryConfiguration>()));
            }
        }

        [TestCaseSource(nameof(CreateShouldAssignFilterViewModelsStatsTestCases))]
        public void CreateShouldAssignStatFilterViewModels(StatCategory statCategory, GetFilterViewModels getFilterViewModelsDelegate)
        {
            ItemWithStats item = CreateItemWithStats(statCategory);
            var expected1 = new MinMaxStatFilterViewModel { Id = item.Stats.AllStats[0].Id };
            var expected2 = new MinMaxStatFilterViewModel { Id = item.Stats.AllStats[1].Id };

            this.statFilterViewModelFactoryMock.SetupSequence(x => x.Create(It.IsAny<ItemStat>(), It.IsAny<SearchQueryRequest>(), It.IsAny<StatFilterViewModelFactoryConfiguration>()))
                .Returns(expected1)
                .Returns(expected2);

            AdvancedQueryViewModel result = this.advancedQueryViewModelFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);

            var filterViewModels = getFilterViewModelsDelegate(result);

            Assert.That(filterViewModels, Has.Count.EqualTo(2));

            Assert.That(filterViewModels[0], Is.EqualTo(expected1));
            Assert.That(filterViewModels[1], Is.EqualTo(expected2));
        }

        [Test]
        public void CreateShouldSetQueryRequest()
        {
            var expected = new SearchQueryRequest();
            var item = new EquippableItem(ItemRarity.Magic);

            AdvancedQueryViewModel result = this.advancedQueryViewModelFactory.Create(item, expected);

            Assert.That(result.QueryRequest, Is.EqualTo(expected));
        }

        private static ItemWithStats CreateItemWithStats(StatCategory statCategory)
        {
            return new EquippableItem(ItemRarity.Rare)
            {
                Stats = new ItemStats
                {
                    AllStats =
                    {
                        new ItemStat(statCategory){ Id = $"{statCategory}ItemStatId" },
                        new ItemStat(statCategory){ Id = $"{statCategory}ItemStatId1" },
                    }
                }
            };
        }

        public delegate IList<StatFilterViewModel> GetFilterViewModels(AdvancedQueryViewModel advancedQueryViewModel);

        private static IEnumerable CreateShouldAssignFilterViewModelsStatsTestCases
        {
            get
            {
                yield return new TestCaseData(StatCategory.Enchant, (GetFilterViewModels)(x => x.EnchantedItemStatFilters));
                yield return new TestCaseData(StatCategory.Implicit, (GetFilterViewModels)(x => x.ImplicitItemStatFilters));
                yield return new TestCaseData(StatCategory.Explicit, (GetFilterViewModels)(x => x.ExplicitItemStatFilters));
                yield return new TestCaseData(StatCategory.Crafted, (GetFilterViewModels)(x => x.CraftedItemStatFilters));
                yield return new TestCaseData(StatCategory.Monster, (GetFilterViewModels)(x => x.MonsterItemStatFilters));
            }
        }

        private static IEnumerable<Item> DisabledItems
        {
            get
            {
                yield return new ProphecyItem();
                yield return new DivinationCardItem();
            }
        }

        private static IEnumerable EnabledItems
        {
            get
            {
                yield return new EquippableItem(ItemRarity.Normal);
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new GemItem();
                yield return new JewelItem(ItemRarity.Normal);
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
            }
        }
    }
}