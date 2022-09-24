namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class OptionFilter : IFilter
    {
        public string Option { get; set; } = string.Empty;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}