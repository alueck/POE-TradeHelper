namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IPoeTradeApiJsonSerializer
    {
        T Deserialize<T>(string json);

        string Serialize(object value);
    }
}