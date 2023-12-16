using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NUnit.Framework;

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

        protected void CreateShouldReturnBindableMinMaxFilterViewModel(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            int? currentValue,
            string text)
        {
            // arrange
            BindableMinMaxFilterViewModel expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = currentValue,
                Max = currentValue,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = false,
            };

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableMinMaxFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            int? currentValue,
            string text,
            MinMaxFilter queryRequestFilter)
        {
            // arrange
            BindableMinMaxFilterViewModel expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = queryRequestFilter.Min,
                Max = queryRequestFilter.Max,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = true,
            };

            SearchQueryRequest searchQueryRequest = new SearchQueryRequest();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableMinMaxFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableSocketsFilterViewModel(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            int? currentValue,
            string text)
        {
            // arrange
            BindableSocketsFilterViewModel expected = new BindableSocketsFilterViewModel(expectedBindingExpression)
            {
                Min = currentValue,
                Max = currentValue,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = false,
            };

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableSocketsFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableSocketsFilterViewModelWithValuesFromQueryRequest(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            int? currentValue,
            string text,
            SocketsFilter queryRequestFilter)
        {
            // arrange
            BindableSocketsFilterViewModel expected = new BindableSocketsFilterViewModel(expectedBindingExpression)
            {
                Min = queryRequestFilter.Min,
                Max = queryRequestFilter.Max,
                Red = queryRequestFilter.Red,
                Green = queryRequestFilter.Green,
                Blue = queryRequestFilter.Blue,
                White = queryRequestFilter.White,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = true,
            };

            SearchQueryRequest searchQueryRequest = new SearchQueryRequest();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableSocketsFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableFilterViewModel(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            bool? isEnabled,
            string text)
        {
            // arrange
            BindableFilterViewModel expected = new BindableFilterViewModel(expectedBindingExpression)
            {
                Text = text,
                IsEnabled = isEnabled,
            };

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            Item item,
            string text,
            BoolOptionFilter queryRequestFilter)
        {
            // arrange
            BindableFilterViewModel expected = new BindableFilterViewModel(expectedBindingExpression)
            {
                Text = text,
                IsEnabled = queryRequestFilter.Option,
            };

            SearchQueryRequest searchQueryRequest = new SearchQueryRequest();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            // assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableFilterViewModel(x, expected)));
        }

        private static void SetValueByExpression(
            Expression<Func<SearchQueryRequest, IFilter>> bindingExpression,
            SearchQueryRequest searchQueryRequest,
            IFilter value)
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

        private static bool MatchBindableMinMaxFilterViewModel(
            FilterViewModelBase actual,
            BindableMinMaxFilterViewModel expected)
        {
            return actual is BindableMinMaxFilterViewModel bindableMinMaxFilterViewModel
                   && bindableMinMaxFilterViewModel.BindingExpression.ToString() == expected.BindingExpression.ToString()
                   && bindableMinMaxFilterViewModel.Min == expected.Min
                   && bindableMinMaxFilterViewModel.Max == expected.Max
                   && bindableMinMaxFilterViewModel.Current == expected.Current
                   && bindableMinMaxFilterViewModel.Text == expected.Text
                   && bindableMinMaxFilterViewModel.IsEnabled == expected.IsEnabled;
        }

        private static bool MatchBindableSocketsFilterViewModel(
            FilterViewModelBase actual,
            BindableSocketsFilterViewModel expected)
        {
            return MatchBindableMinMaxFilterViewModel(actual, expected)
                   && actual is BindableSocketsFilterViewModel bindableSocketsFilterViewModel
                   && bindableSocketsFilterViewModel.Red == expected.Red
                   && bindableSocketsFilterViewModel.Green == expected.Green
                   && bindableSocketsFilterViewModel.Blue == expected.Blue
                   && bindableSocketsFilterViewModel.White == expected.White;
        }

        private static bool MatchBindableFilterViewModel(FilterViewModelBase actual, BindableFilterViewModel expected)
        {
            return actual is BindableFilterViewModel bindableFilterViewModel
                   && bindableFilterViewModel.BindingExpression.ToString() == expected.BindingExpression.ToString()
                   && bindableFilterViewModel.Text == expected.Text
                   && bindableFilterViewModel.IsEnabled == expected.IsEnabled;
        }
    }
}