using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        [SetUp]
        public void Setup() => this.AdditionalFilterViewModelsFactory =
            new GemItemAdditionalFilterViewModelsFactory(Substitute.For<IOptionsMonitor<ItemSearchOptions>>());

        [TestCaseSource(nameof(GetNonGemItems))]
        public void CreateShouldReturnEmptyEnumerableForNonGemItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnQualityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression =
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
        public void CreateShouldReturnGemLevelFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression =
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
        public void CreateShouldReturnExperiencePercentFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression =
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