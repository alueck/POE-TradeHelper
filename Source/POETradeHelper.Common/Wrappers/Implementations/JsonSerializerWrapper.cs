using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace POETradeHelper.Common.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class JsonSerializerWrapper : IJsonSerializerWrapper
    {
        public T Deserialize<T>(string json, JsonSerializerOptions options = null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
        }

        public string Serialize(object value, JsonSerializerOptions options = null)
        {
            return System.Text.Json.JsonSerializer.Serialize(value, options);
        }
    }
}