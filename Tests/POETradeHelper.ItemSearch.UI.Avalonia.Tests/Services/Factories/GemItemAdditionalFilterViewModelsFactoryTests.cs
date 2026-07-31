using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using AwesomeAssertions;

using Microsoft.Extensions.Options;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class GemItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        public GemItemAdditionalFilterViewModelsFactoryTests()
        {
            this.AdditionalFilterViewModelsFactory = new GemItemAdditionalFilterViewModelsFactory(Substitute.For<IOptionsMonitor<ItemSearchOptions>>());
        }

        [TestCaseSource(nameof(GetNonGemItems))]
        public void Create_ShouldReturnEmptyEnumerableForNonGemItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            result.Should().BeEmpty();
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void Create_ShouldReturnQualityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Quality;
            GemItem gemItem = new()
            {
                Quality = 11,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.QualityColumn,
                gemItem.Quality,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void Create_ShouldReturnGemLevelFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.GemLevel;
            GemItem gemItem = new()
            {
                Level = 10,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.GemLevelColumn,
                gemItem.Level,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void Create_ShouldReturnExperiencePercentFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.GemLevelProgress;
            GemItem gemItem = new()
            {
                ExperiencePercent = 27,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.GemExperiencePercentColumn,
                gemItem.ExperiencePercent,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Create_ShouldReturnCorruptedFilterViewModel_IfNonVaalGem(bool corrupted)
        {
            // arrange
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            GemItem gemItem = new()
            {
                IsCorrupted = corrupted,
            };

            BoolOptionFilter queryRequestFilter = new() { Option = corrupted };

            // act & assert
            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.Corrupted,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Create_ShouldReturnImbuedFilterViewModel_IfNonVaalGem(bool imbued)
        {
            // arrange
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.GemImbued;
            GemItem gemItem = new()
            {
                IsImbued = imbued,
            };

            BoolOptionFilter queryRequestFilter = new() { Option = imbued };

            // act & assert
            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.Imbued,
                queryRequestFilter);
        }

        [Test]
        public void Create_ShouldNotReturnCorruptedFilterViewModel_IfVaalGem()
        {
            // arrange
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            GemItem gemItem = new()
            {
                IsCorrupted = true,
                IsVaalVersion = true,
            };

            BoolOptionFilter queryRequestFilter = new() { Option = true };
            SearchQueryRequest searchQueryRequest = new();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(gemItem, searchQueryRequest);

            // assert
            result.Should().NotContain(x => x.Text == Resources.Corrupted);
        }

        [Test]
        public void Create_ShouldNotReturnImbuedFilterViewModel_IfVaalGem()
        {
            // arrange
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            GemItem gemItem = new()
            {
                IsVaalVersion = true,
                IsImbued = false,
            };

            BoolOptionFilter queryRequestFilter = new() { Option = false };
            SearchQueryRequest searchQueryRequest = new();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(gemItem, searchQueryRequest);

            // assert
            result.Should().NotContain(x => x.Text == Resources.Imbued);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Create_ShouldReturnTransfiguredFilterViewModel_IfNonVaalGem(bool transfigured)
        {
            // arrange
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.GemTransfigured;
            GemItem gemItem = new()
            {
                IsTransfigured = transfigured,
            };

            BoolOptionFilter queryRequestFilter = new() { Option = transfigured };

            // act & assert
            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                gemItem,
                Resources.Transfigured,
                queryRequestFilter);
        }

        private static IEnumerable<Item> GetNonGemItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new FlaskItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
        }
    }
}