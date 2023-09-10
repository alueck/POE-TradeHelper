using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.ViewModels
{
    public class AdvancedFiltersViewModelTests
    {
        private IStatFilterViewModelFactory statFilterViewModelFactoryMock;
        private List<IAdditionalFilterViewModelsFactory> additionalFiltersViewModelFactoryMocks;
        private AdvancedFiltersViewModel advancedFiltersViewModel;

        [SetUp]
        public void Setup()
        {
            this.statFilterViewModelFactoryMock = Substitute.For<IStatFilterViewModelFactory>();
            this.additionalFiltersViewModelFactoryMocks = new List<IAdditionalFilterViewModelsFactory> {
                Substitute.For<IAdditionalFilterViewModelsFactory>(),
                Substitute.For<IAdditionalFilterViewModelsFactory>()
            };
            this.advancedFiltersViewModel = new AdvancedFiltersViewModel(this.statFilterViewModelFactoryMock, this.additionalFiltersViewModelFactoryMocks);
        }

        [TestCaseSource(nameof(DisabledItems))]
        public async Task LoadAsyncShouldSetIsEnabledToFalse(Item item)
        {
            var searchQueryRequest = new SearchQueryRequest();

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            Assert.IsFalse(this.advancedFiltersViewModel.IsEnabled);
        }

        [TestCaseSource(nameof(EnabledItems))]
        public async Task LoadAsyncShouldSetIsEnabledToTrue(Item item)
        {
            var searchQueryRequest = new SearchQueryRequest();

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            Assert.IsTrue(this.advancedFiltersViewModel.IsEnabled);
        }

        [TestCase(StatCategory.Enchant)]
        [TestCase(StatCategory.Fractured)]
        [TestCase(StatCategory.Implicit)]
        [TestCase(StatCategory.Explicit)]
        [TestCase(StatCategory.Crafted)]
        [TestCase(StatCategory.Monster)]
        [TestCase(StatCategory.Pseudo)]
        public async Task LoadAsyncShouldCallCreateOnStatFilterViewModelFactoryForItemStats(StatCategory statCategory)
        {
            ItemWithStats item = CreateItemWithStats(statCategory);
            SearchQueryRequest searchQueryRequest = new() { League = "Heist" };

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            foreach (var itemStat in item.Stats.AllStats)
            {
                this.statFilterViewModelFactoryMock
                .Received()
                .Create(itemStat, searchQueryRequest);
            }
        }

        [TestCaseSource(nameof(LoadAsyncShouldAssignFilterViewModelsStatsTestCases))]
        public async Task LoadAsyncShouldSetStatFilterViewModels(StatCategory statCategory, GetFilterViewModels getFilterViewModelsDelegate)
        {
            ItemWithStats item = CreateItemWithStats(statCategory);
            var expected1 = new MinMaxStatFilterViewModel { Id = item.Stats.AllStats[0].Id };
            var expected2 = new MinMaxStatFilterViewModel { Id = item.Stats.AllStats[1].Id };

            this.statFilterViewModelFactoryMock
                .Create(Arg.Any<ItemStat>(), Arg.Any<SearchQueryRequest>())
                .Returns(expected1, expected2);

            await this.advancedFiltersViewModel.LoadAsync(item, new SearchQueryRequest(), default);

            var filterViewModels = getFilterViewModelsDelegate(this.advancedFiltersViewModel);

            Assert.That(filterViewModels, Has.Count.EqualTo(2));

            Assert.That(filterViewModels[0], Is.EqualTo(expected1));
            Assert.That(filterViewModels[1], Is.EqualTo(expected2));
        }

        [Test]
        public async Task LoadAsyncShouldCallCreateOnAdditionalFiltersViewModelFactories()
        {
            var item = new EquippableItem(ItemRarity.Magic);
            SearchQueryRequest searchQueryRequest = new() { League = "Heist" };

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            this.additionalFiltersViewModelFactoryMocks[0]
                .Received()
                .Create(item, searchQueryRequest);
            this.additionalFiltersViewModelFactoryMocks[1]
                .Received()
                .Create(item, searchQueryRequest);
        }

        [Test]
        public async Task LoadAsyncShouldAddReturnValuesOfAdditionalFiltersViewModelFactoriesToAdditionalFilters()
        {
            var item = new EquippableItem(ItemRarity.Magic);
            var expected1 = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.Quality);
            var expected2 = new BindableMinMaxFilterViewModel(x => x.Query.Filters.ArmourFilters.Armour);

            this.additionalFiltersViewModelFactoryMocks[0].Create(Arg.Any<Item>(), Arg.Any<SearchQueryRequest>())
                .Returns(new[] { expected1 });
            this.additionalFiltersViewModelFactoryMocks[1].Create(Arg.Any<Item>(), Arg.Any<SearchQueryRequest>())
                .Returns(new[] { expected2 });

            await this.advancedFiltersViewModel.LoadAsync(item, new SearchQueryRequest(), default);

            Assert.That(this.advancedFiltersViewModel.AdditionalFilters, Has.Count.EqualTo(2));
            Assert.That(this.advancedFiltersViewModel.AdditionalFilters, Contains.Item(expected1));
            Assert.That(this.advancedFiltersViewModel.AdditionalFilters, Contains.Item(expected2));
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

        public delegate IList<StatFilterViewModel> GetFilterViewModels(AdvancedFiltersViewModel advancedFiltersViewModel);

        private static IEnumerable LoadAsyncShouldAssignFilterViewModelsStatsTestCases
        {
            get
            {
                yield return new TestCaseData(StatCategory.Enchant, (GetFilterViewModels)(x => x.EnchantedItemStatFilters));
                yield return new TestCaseData(StatCategory.Fractured, (GetFilterViewModels)(x => x.FracturedItemStatFilters));
                yield return new TestCaseData(StatCategory.Implicit, (GetFilterViewModels)(x => x.ImplicitItemStatFilters));
                yield return new TestCaseData(StatCategory.Explicit, (GetFilterViewModels)(x => x.ExplicitItemStatFilters));
                yield return new TestCaseData(StatCategory.Crafted, (GetFilterViewModels)(x => x.CraftedItemStatFilters));
                yield return new TestCaseData(StatCategory.Monster, (GetFilterViewModels)(x => x.MonsterItemStatFilters));
                yield return new TestCaseData(StatCategory.Pseudo, (GetFilterViewModels)(x => x.PseudoItemStatFilters));
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
