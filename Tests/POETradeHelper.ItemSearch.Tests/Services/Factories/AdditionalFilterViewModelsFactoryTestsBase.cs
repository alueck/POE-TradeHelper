using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public abstract class AdditionalFilterViewModelsFactoryTestsBase
    {
        protected IAdditionalFilterViewModelsFactory AdditionalFilterViewModelsFactory { get; set; }

        protected void CreateShouldReturnBindableMinMaxFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression, Item item, int? currentValue, string text)
        {
            // arrange
            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = currentValue,
                Max = currentValue,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = false
            };

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

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
            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = queryRequestFilter.Min,
                Max = queryRequestFilter.Max,
                Current = currentValue.ToString(),
                Text = text,
                IsEnabled = true
            };

            var searchQueryRequest = new SearchQueryRequest();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableMinMaxFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression, Item item, bool isEnabled, string text)
        {
            // arrange
            var expected = new BindableFilterViewModel(expectedBindingExpression)
            {
                Text = text,
                IsEnabled = isEnabled
            };

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableFilterViewModel(x, expected)));
        }

        protected void CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression, Item item, string text, BoolOptionFilter queryRequestFilter)
        {
            // arrange
            var expected = new BindableFilterViewModel(expectedBindingExpression)
            {
                Text = text,
                IsEnabled = queryRequestFilter.Option
            };

            var searchQueryRequest = new SearchQueryRequest();
            SetValueByExpression(expectedBindingExpression, searchQueryRequest, queryRequestFilter);

            // act
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, searchQueryRequest);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchBindableFilterViewModel(x, expected)));
        }

        private static void SetValueByExpression(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression, SearchQueryRequest searchQueryRequest, IFilter value)
        {
            var expressions = bindingExpression.Body.GetExpressionChain().ToList();
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

        private static bool MatchBindableMinMaxFilterViewModel(FilterViewModelBase actual, BindableMinMaxFilterViewModel expected)
        {
            return actual is BindableMinMaxFilterViewModel bindableMinMaxFilterViewModel
                            && bindableMinMaxFilterViewModel.BindingExpression.ToString() == expected.BindingExpression.ToString()
                            && bindableMinMaxFilterViewModel.Min == expected.Min
                            && bindableMinMaxFilterViewModel.Max == expected.Max
                            && bindableMinMaxFilterViewModel.Current == expected.Current
                            && bindableMinMaxFilterViewModel.Text == expected.Text
                            && bindableMinMaxFilterViewModel.IsEnabled == expected.IsEnabled;
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