namespace POETradeHelper.ItemSearch.ViewModels
{
    public class StatFilterViewModel : FilterViewModelBase
    {
        public StatFilterViewModel()
        {
            this.IsEnabled = false;
        }

        public string Id { get; set; }
    }
}