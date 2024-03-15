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
    public class FlaskItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup() => this.AdditionalFilterViewModelsFactory = new FlaskItemAdditionalFilterViewModelsFactory(Substitute.For<IOptionsMonitor<ItemSearchOptions>>());

        [TestCaseSource(nameof(GetNonFlaskItems))]
        public void CreateShouldReturnEmptyEnumerableForNonFlaskItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnQualityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            FlaskItem flaskItem = new(ItemRarity.Unique)
            {
                Quality = 20,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                flaskItem,
                Resources.QualityColumn,
                flaskItem.Quality,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnIdentifiedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            FlaskItem flaskItem = new(ItemRarity.Rare)
            {
                IsIdentified = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                flaskItem,
                Resources.Identified,
                queryRequestFilter);
        }

        private static IEnumerable<Item> GetNonFlaskItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
            yield return new GemItem();
        }
    }
}