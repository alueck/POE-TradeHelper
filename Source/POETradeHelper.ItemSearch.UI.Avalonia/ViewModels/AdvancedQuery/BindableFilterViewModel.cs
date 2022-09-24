using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

using System;
using System.Linq.Expressions;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class BindableFilterViewModel : FilterViewModelBase
    {
        public BindableFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression)
        {
            this.BindingExpression = bindingExpression;
        }

        public Expression<Func<SearchQueryRequest, IFilter>> BindingExpression { get; }
    }
}