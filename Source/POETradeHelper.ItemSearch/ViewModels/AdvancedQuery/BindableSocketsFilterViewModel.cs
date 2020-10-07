using System;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class BindableSocketsFilterViewModel : BindableMinMaxFilterViewModel
    {
        public BindableSocketsFilterViewModel(System.Linq.Expressions.Expression<Func<PathOfExileTradeApi.Models.SearchQueryRequest, PathOfExileTradeApi.Models.Filters.IFilter>> bindingExpression)
            : base(bindingExpression)
        {
        }

        public int? Red { get; set; }
        public int? Green { get; set; }
        public int? Blue { get; set; }
        public int? White { get; set; }
    }
}