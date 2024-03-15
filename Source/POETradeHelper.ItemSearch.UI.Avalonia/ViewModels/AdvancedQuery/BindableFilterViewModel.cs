using System;
using System.Linq.Expressions;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class BindableFilterViewModel<TFilter> : FilterViewModelBase, IBindableFilterViewModel
        where TFilter : IFilter
    {
        private readonly Lazy<Expression<Func<SearchQueryRequest, IFilter?>>> expression;

        public BindableFilterViewModel(Expression<Func<SearchQueryRequest, TFilter?>> bindingExpression)
        {
            this.BindingExpression = bindingExpression;
            this.expression = new Lazy<Expression<Func<SearchQueryRequest, IFilter?>>>(this.GetExpressionWithIFilterType);
        }

        public Expression<Func<SearchQueryRequest, TFilter?>> BindingExpression { get; }

        Expression<Func<SearchQueryRequest, IFilter?>> IBindableFilterViewModel.BindingExpression => this.expression.Value;

        private Expression<Func<SearchQueryRequest, IFilter?>> GetExpressionWithIFilterType()
        {
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(SearchQueryRequest), typeof(IFilter));

            return (Expression<Func<SearchQueryRequest, IFilter?>>)Expression.Lambda(delegateType, this.BindingExpression.Body, this.BindingExpression.Parameters);
        }
    }
}