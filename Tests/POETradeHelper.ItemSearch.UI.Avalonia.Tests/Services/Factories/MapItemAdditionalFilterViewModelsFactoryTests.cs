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
    public class MapItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup() => this.AdditionalFilterViewModelsFactory = new MapItemAdditionalFilterViewModelsFactory();

        [TestCaseSource(nameof(GetNonMapItems))]
        public void CreateShouldReturnEmptyEnumerableForNonMapItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Quality = 10,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.Quality, Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Quality = 10,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 8,
                Max = 15,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                mapItem.Quality,
                Resources.QualityColumn,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnItemQuantityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemQuantity = 79,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                mapItem.ItemQuantity,
                Resources.MapItemQuantity);
        }

        [Test]
        public void CreateShouldReturnItemQuantityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemQuantity = 79,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 60,
                Max = 76,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                mapItem.ItemQuantity,
                Resources.MapItemQuantity,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnItemRarityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemRarity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemRarity = 101,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                mapItem.ItemRarity,
                Resources.MapItemRarity);
        }

        [Test]
        public void CreateShouldReturnItemRarityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemRarity;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                ItemRarity = 101,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 80,
                Max = 95,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                mapItem.ItemRarity,
                Resources.MapItemRarity,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnMonsterPackSizeFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapPacksize;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                MonsterPackSize = 35,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                mapItem,
                mapItem.MonsterPackSize,
                Resources.MapMonsterPacksize);
        }

        [Test]
        public void CreateShouldReturnMonsterPackSizeFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapPacksize;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                MonsterPackSize = 35,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 25,
                Max = 30,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                mapItem.MonsterPackSize,
                Resources.MapMonsterPacksize,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnMapTierFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapTier;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Tier = 6,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.Tier, Resources.MapTier);
        }

        [Test]
        public void CreateShouldReturnMapTierFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapTier;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                Tier = 6,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 4,
                Max = 8,
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                mapItem.Tier,
                Resources.MapTier,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsIdentified = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsIdentified = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnCorruptedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsCorrupted = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.Corrupted);
        }

        [Test]
        public void CreateShouldReturnCorruptedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsCorrupted = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                Resources.Corrupted,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnBlightedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlighted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsBlighted = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.MapBlighted);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnBlightRavagedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlightRavaged;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsBlightRavaged = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.MapBlightRavaged);
        }

        [Test]
        public void CreateShouldReturnBlightedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlighted;
            MapItem mapItem = new(ItemRarity.Rare)
            {
                IsBlighted = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                mapItem,
                Resources.MapBlighted,
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