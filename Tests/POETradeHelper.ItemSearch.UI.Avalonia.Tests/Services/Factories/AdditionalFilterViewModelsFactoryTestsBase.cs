using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using FluentAssertions;
using FluentAssertions.Equivalency;

using Microsoft.Extensions.Options;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public abstract class AdditionalFilterViewModelsFactoryTestsBase
    {
        protected IAdditionalFilterViewModelsFactory AdditionalFilterViewModelsFactory { get; set; }

        protected ItemSearchOptions ItemSearchOptions { get; } = new();

        protected IOptionsMonitor<ItemSearchOptions> ItemSearchOptionsMonitorMock { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.ItemSearchOptionsMonitorMock = Substitute.For<IOptionsMonitor<ItemSearchOptions>>();
            this.ItemSearchOptionsMonitorMock.CurrentValue.Returns(this.ItemSearchOptions);
        }

        protected void CreateShouldReturnBindableMinMaxFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression,
            Item item,
            string text,
            int currentValue,
            MinMaxFilter queryRequestFilter,
            bool offsetCurrentValue = false)
        {
            // arrange
            BindableMinMaxFilterViewModel expected = new(expectedBindingExpression)
            {
                Min = queryRequestFilter?.Min ??
                      Math.Floor(currentValue * (offsetCurrentValue ? 1 + this.ItemSearchOptions.AdvancedQueryOptions.MinValuePercentageOffset : 1)),
                Max = queryRequestFilter?.Max ??
                      Math.Ceiling(currentValue * (offsetCurrentValue ? 1 + this.ItemSearchOptions.AdvancedQueryOptions.MaxValuePercentageOffset : 1)),
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = queryRequestFilter != null,
            };

            SearchQueryRequest searchQueryRequest = new();
            if (queryRequestFilter != null)
            {
                SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);
            }

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            result.Should().ContainEquivalentOf(
                expected,
                config => config
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.ThrownExceptions));
        }

        protected void CreateShouldReturnBindableMinMaxFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression,
            Item item,
            string text,
            decimal currentValue,
            MinMaxFilter queryRequestFilter)
        {
            // arrange
            BindableMinMaxFilterViewModel expected = new(expectedBindingExpression)
            {
                Min = queryRequestFilter?.Min ??
                      Math.Round(currentValue * (1 + this.ItemSearchOptions.AdvancedQueryOptions.MinValuePercentageOffset), 2),
                Max = queryRequestFilter?.Max ??
                      Math.Round(currentValue * (1 + this.ItemSearchOptions.AdvancedQueryOptions.MaxValuePercentageOffset), 2),
                Current = currentValue.ToString("N2"),
                Text = text,
                IsEnabled = queryRequestFilter != null,
            };

            SearchQueryRequest searchQueryRequest = new();
            if (queryRequestFilter != null)
            {
                SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);
            }

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            result.Should().ContainEquivalentOf(
                expected,
                config => config
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.ThrownExceptions));
        }

        protected void CreateShouldReturnBindableSocketsFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter>> expectedBindingExpression,
            Item item,
            string text,
            int? currentValue,
            SocketsFilter queryRequestFilter)
        {
            // arrange
            BindableSocketsFilterViewModel expected = new(expectedBindingExpression)
            {
                Min = queryRequestFilter?.Min ?? currentValue,
                Max = queryRequestFilter?.Max ?? currentValue,
                Red = queryRequestFilter?.Red,
                Green = queryRequestFilter?.Green,
                Blue = queryRequestFilter?.Blue,
                White = queryRequestFilter?.White,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = queryRequestFilter != null,
            };

            SearchQueryRequest searchQueryRequest = new SearchQueryRequest();
            if (queryRequestFilter != null)
            {
                SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);
            }

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            result.Should().ContainEquivalentOf(
                expected,
                config => config
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.ThrownExceptions));
        }

        protected void CreateShouldReturnBindableBoolOptionFilterViewModel(
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression,
            Item item,
            string text,
            BoolOptionFilter queryRequestFilter)
        {
            // arrange
            BindableFilterViewModel<BoolOptionFilter> expected = new(expectedBindingExpression)
            {
                Text = text,
                IsEnabled = queryRequestFilter?.Option,
            };

            SearchQueryRequest searchQueryRequest = new SearchQueryRequest();
            if (queryRequestFilter != null)
            {
                SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);
            }

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            result.Should().ContainEquivalentOf(
                expected,
                config => config
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.Changed)
                    .Excluding(x => x.ThrownExceptions));
        }

        protected static IEnumerable<BoolOptionFilter> GetBoolOptionFilterTestCases()
        {
            yield return new BoolOptionFilter { Option = false };
            yield return new BoolOptionFilter { Option = true };
        }

        protected static IEnumerable<MinMaxFilter> GetMinMaxFilterTestCases()
        {
            yield return null;
            yield return new MinMaxFilter { Min = 45, Max = 97 };
        }

        private static void SetValueByExpression<TFilter>(
            Expression<Func<SearchQueryRequest, TFilter>> bindingExpression,
            SearchQueryRequest searchQueryRequest,
            TFilter value)
            where TFilter : IFilter
        {
            List<Expression> expressions = bindingExpression.Body.GetExpressionChain().ToList();
            object parent = searchQueryRequest;

            foreach (MemberExpression expression in expressions)
            {
                if (expression == expressions.Last())
                {
                    ((PropertyInfo)expression.Member).SetValue(parent, value);
                    break;
                }

                parent = ((PropertyInfo)expression.Member).GetValue(parent);
            }
        }
    }
}