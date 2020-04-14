using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Linq.Expressions;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class BindableMinMaxFilterViewModel : FilterViewModelBase, IMinMaxFilterViewModel
    {
        public BindableMinMaxFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression)
        {
            BindingExpression = bindingExpression;
        }

        public Expression<Func<SearchQueryRequest, IFilter>> BindingExpression { get; }

        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Current { get; set; }
    }
}