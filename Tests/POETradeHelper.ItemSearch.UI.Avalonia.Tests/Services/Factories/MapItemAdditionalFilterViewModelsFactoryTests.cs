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
    public class MapItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup() => this.AdditionalFilterViewModelsFactory =
            new MapItemAdditionalFilterViewModelsFactory(Substitute.For<IOptionsMonitor<ItemSearchOptions>>());

        [TestCaseSource(nameof(GetNonMapItems))]
        public void CreateShouldReturnEmptyEnumerableForNonMapItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnQualityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Quality = 10,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.QualityColumn,
                mapItem.Quality,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnItemQuantityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemQuantity = 79,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapItemQuantity,
                mapItem.ItemQuantity,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnItemRarityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemRarity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemRarity = 101,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapItemRarity,
                mapItem.ItemRarity,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnMonsterPackSizeFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapPacksize;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                MonsterPackSize = 35,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapMonsterPacksize,
                mapItem.MonsterPackSize,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnMapTierFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapTier;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Tier = 6,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapTier,
                mapItem.Tier,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnIdentifiedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsIdentified = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnCorruptedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsCorrupted = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.Corrupted,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnBlightedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlighted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsBlighted = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapBlighted,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnBlightRavagedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlightRavaged;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsBlightRavaged = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                mapItem,
                Resources.MapBlightRavaged,
                queryRequestFilter);
        }

        private static IEnumerable<Item> GetNonMapItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new FlaskItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
            yield return new GemItem();
        }
    }
}