using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

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
        public void Setup() => this.AdditionalFilterViewModelsFactory = new FlaskItemAdditionalFilterViewModelsFactory();

        [TestCaseSource(nameof(GetNonFlaskItems))]
        public void CreateShouldReturnEmptyEnumerableForNonFlaskItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            FlaskItem flaskItem = new(ItemRarity.Unique)
            {
                Quality = 20,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                flaskItem,
                flaskItem.Quality,
                Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            FlaskItem flaskItem = new(ItemRarity.Unique)
            {
                Quality = 19,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 15,
                Max = 20,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                flaskItem,
                flaskItem.Quality,
                Resources.QualityColumn,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            FlaskItem flaskItem = new(ItemRarity.Rare)
            {
                IsIdentified = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, flaskItem, null, Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            FlaskItem flaskItem = new(ItemRarity.Rare)
            {
                IsIdentified = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
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