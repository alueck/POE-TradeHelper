using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace POETradeHelper.Common.Wrappers.Implementations
{
    [ExcludeFromCodeCoverage]
    public class JsonSerializerWrapper : IJsonSerializerWrapper
    {
        public T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        public string Serialize(object value, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(value, options);
        }
    }
}