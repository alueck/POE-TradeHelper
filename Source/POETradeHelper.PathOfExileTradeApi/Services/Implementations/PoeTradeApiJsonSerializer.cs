using System.Text.Json;
using System.Text.Json.Serialization;

using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiJsonSerializer : IPoeTradeApiJsonSerializer
    {
        private static readonly JsonSerializerOptions CamelCaseJsonSerializerOptions =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private static readonly JsonSerializerOptions SnakeCaseJsonSerializerOptions = new()
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            Converters = { new JsonStringEnumConverter(new JsonSnakeCaseNamingPolicy()) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private readonly IJsonSerializerWrapper jsonSerializer;

        public PoeTradeApiJsonSerializer(IJsonSerializerWrapper jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public T? Deserialize<T>(string json) => this.jsonSerializer.Deserialize<T>(json, CamelCaseJsonSerializerOptions);

        public string Serialize(object value) => this.jsonSerializer.Serialize(value, SnakeCaseJsonSerializerOptions);
    }
}