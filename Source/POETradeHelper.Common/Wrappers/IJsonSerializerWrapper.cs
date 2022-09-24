using System.Text.Json;

namespace POETradeHelper.Common.Wrappers
{
    public interface IJsonSerializerWrapper
    {
        T? Deserialize<T>(string json, JsonSerializerOptions? options = null);

        string Serialize(object value, JsonSerializerOptions? options = null);
    }
}