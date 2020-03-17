namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class OptionFilter : IFilter
    {
        public string Option { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}