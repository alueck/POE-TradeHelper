using System;
using System.Linq.Expressions;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class BindableFilterViewModel : FilterViewModelBase
    {
        public BindableFilterViewModel(Expression<Func<SearchQueryRequest, IFilter?>> bindingExpression)
        {
            this.BindingExpression = bindingExpression;
        }

        public Expression<Func<SearchQueryRequest, IFilter?>> BindingExpression { get; }
    }
}