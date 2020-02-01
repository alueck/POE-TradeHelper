using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using System.Text.Json;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiJsonSerializer : IPoeTradeApiJsonSerializer
    {
        private static readonly JsonSerializerOptions camelCaseJsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private static readonly JsonSerializerOptions snakeCaseJsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy() };

        private IJsonSerializerWrapper jsonSerializer;

        public PoeTradeApiJsonSerializer(IJsonSerializerWrapper jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public T Deserialize<T>(string json)
        {
            return this.jsonSerializer.Deserialize<T>(json, camelCaseJsonSerializerOptions);
        }

        public string Serialize(object value)
        {
            return this.jsonSerializer.Serialize(value, snakeCaseJsonSerializerOptions);
        }
    }
}