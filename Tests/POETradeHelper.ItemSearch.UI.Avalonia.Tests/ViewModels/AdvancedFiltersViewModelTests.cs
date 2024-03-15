using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
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
        private readonly IStatFilterViewModelFactory statFilterViewModelFactoryMock;
        private readonly List<IAdditionalFilterViewModelsFactory> additionalFiltersViewModelFactoryMocks;
        private readonly AdvancedFiltersViewModel advancedFiltersViewModel;

        public AdvancedFiltersViewModelTests()
        {
            this.statFilterViewModelFactoryMock = Substitute.For<IStatFilterViewModelFactory>();
            this.additionalFiltersViewModelFactoryMocks = new List<IAdditionalFilterViewModelsFactory>
            {
                Substitute.For<IAdditionalFilterViewModelsFactory>(),
                Substitute.For<IAdditionalFilterViewModelsFactory>(),
            };
            this.advancedFiltersViewModel = new AdvancedFiltersViewModel(
                this.statFilterViewModelFactoryMock,
                this.additionalFiltersViewModelFactoryMocks);
        }

        public delegate IList<StatFilterViewModel> GetFilterViewModels(AdvancedFiltersViewModel advancedFiltersViewModel);

        [TestCaseSource(nameof(GetDisabledItems))]
        public async Task LoadAsyncShouldSetIsEnabledToFalse(Item item)
        {
            SearchQueryRequest searchQueryRequest = new();

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            this.advancedFiltersViewModel.IsEnabled.Should().BeFalse();
        }

        [TestCaseSource(nameof(GetEnabledItems))]
        public async Task LoadAsyncShouldSetIsEnabledToTrue(Item item)
        {
            SearchQueryRequest searchQueryRequest = new();

            await this.advancedFiltersViewModel.LoadAsync(item, searchQueryRequest, default);

            this.advancedFiltersViewModel.IsEnabled.Should().BeTrue();
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

            foreach (ItemStat itemStat in item.Stats!.AllStats)
            {
                this.statFilterViewModelFactoryMock
                    .Received()
                    .Create(itemStat, searchQueryRequest);
            }
        }

        [TestCaseSource(nameof(GetLoadAsyncShouldAssignFilterViewModelsStatsTestCases))]
        public async Task LoadAsyncShouldSetStatFilterViewModels(StatCategory statCategory, GetFilterViewModels getFilterViewModelsDelegate)
        {
            ItemWithStats item = CreateItemWithStats(statCategory);
            MinMaxStatFilterViewModel expected1 = new() { Id = item.Stats!.AllStats[0].Id };
            MinMaxStatFilterViewModel expected2 = new() { Id = item.Stats.AllStats[1].Id };

            this.statFilterViewModelFactoryMock
                .Create(Arg.Any<ItemStat>(), Arg.Any<SearchQueryRequest>())
                .Returns(expected1, expected2);

            await this.advancedFiltersViewModel.LoadAsync(item, new SearchQueryRequest(), default);

            IList<StatFilterViewModel> filterViewModels = getFilterViewModelsDelegate(this.advancedFiltersViewModel);

            filterViewModels.Should()
                .HaveCount(2)
                .And.Contain(expected1)
                .And.Contain(expected2);
        }

        [Test]
        public async Task LoadAsyncShouldCallCreateOnAdditionalFiltersViewModelFactories()
        {
            EquippableItem item = new(ItemRarity.Magic);
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
            EquippableItem item = new(ItemRarity.Magic);
            BindableMinMaxFilterViewModel expected1 = new(x => x.Query.Filters.MiscFilters.Quality);
            BindableMinMaxFilterViewModel expected2 = new(x => x.Query.Filters.ArmourFilters.Armour);

            this.additionalFiltersViewModelFactoryMocks[0].Create(Arg.Any<Item>(), Arg.Any<SearchQueryRequest>())
                .Returns(new[] { expected1 });
            this.additionalFiltersViewModelFactoryMocks[1].Create(Arg.Any<Item>(), Arg.Any<SearchQueryRequest>())
                .Returns(new[] { expected2 });

            await this.advancedFiltersViewModel.LoadAsync(item, new SearchQueryRequest(), default);

            this.advancedFiltersViewModel.AdditionalFilters.Should()
                .HaveCount(2)
                .And.Contain(expected1)
                .And.Contain(expected2);
        }

        [Test]
        public async Task LoadAsyncShouldClearStatFiltersIfItemIsNotItemWithStats()
        {
            GemItem item = new();
            this.advancedFiltersViewModel.ExplicitItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.ImplicitItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.CraftedItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.EnchantedItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.FracturedItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.MonsterItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.PseudoItemStatFilters.Add(new StatFilterViewModel());
            this.advancedFiltersViewModel.AdditionalFilters.Add(new StatFilterViewModel());

            await this.advancedFiltersViewModel.LoadAsync(item, new SearchQueryRequest(), default);

            this.advancedFiltersViewModel.AllStatFilters.Should().BeEmpty();
        }

        private static ItemWithStats CreateItemWithStats(StatCategory statCategory) =>
            new EquippableItem(ItemRarity.Rare)
            {
                Stats = new ItemStats
                {
                    AllStats =
                    {
                        new ItemStat(statCategory) { Id = $"{statCategory}ItemStatId" },
                        new ItemStat(statCategory) { Id = $"{statCategory}ItemStatId1" },
                    },
                },
            };

        private static IEnumerable GetLoadAsyncShouldAssignFilterViewModelsStatsTestCases()
        {
            yield return new TestCaseData(StatCategory.Enchant, (GetFilterViewModels)(x => x.EnchantedItemStatFilters));
            yield return new TestCaseData(StatCategory.Fractured, (GetFilterViewModels)(x => x.FracturedItemStatFilters));
            yield return new TestCaseData(StatCategory.Implicit, (GetFilterViewModels)(x => x.ImplicitItemStatFilters));
            yield return new TestCaseData(StatCategory.Explicit, (GetFilterViewModels)(x => x.ExplicitItemStatFilters));
            yield return new TestCaseData(StatCategory.Crafted, (GetFilterViewModels)(x => x.CraftedItemStatFilters));
            yield return new TestCaseData(StatCategory.Monster, (GetFilterViewModels)(x => x.MonsterItemStatFilters));
            yield return new TestCaseData(StatCategory.Pseudo, (GetFilterViewModels)(x => x.PseudoItemStatFilters));
        }

        private static IEnumerable<Item> GetDisabledItems()
        {
            yield return new ProphecyItem();
            yield return new DivinationCardItem();
        }

        private static IEnumerable GetEnabledItems()
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