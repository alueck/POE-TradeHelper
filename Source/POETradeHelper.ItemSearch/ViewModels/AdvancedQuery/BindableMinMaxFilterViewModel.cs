using System;
using System.Linq.Expressions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class BindableMinMaxFilterViewModel : BindableFilterViewModel, IMinMaxFilterViewModel
    {
        public BindableMinMaxFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression) : base(bindingExpression)
        {
            this.IsEnabled = false;
        }

        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Current { get; set; }
    }
}