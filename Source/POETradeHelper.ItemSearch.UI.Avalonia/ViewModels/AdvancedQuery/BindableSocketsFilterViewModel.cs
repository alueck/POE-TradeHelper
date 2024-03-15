using System;
using System.Linq.Expressions;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class BindableSocketsFilterViewModel : BindableMinMaxFilterViewModel
    {
        public BindableSocketsFilterViewModel(Expression<Func<SearchQueryRequest, MinMaxFilter?>> bindingExpression)
            : base(bindingExpression)
        {
        }

        public int? Red { get; set; }

        public int? Green { get; set; }

        public int? Blue { get; set; }

        public int? White { get; set; }
    }
}