using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiJsonSerializer : IPoeTradeApiJsonSerializer
    {
        private static readonly JsonSerializerOptions CamelCaseJsonSerializerOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { IgnoreExtensionData },
                },
            };

        private static readonly JsonSerializerOptions SnakeCaseJsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };

        public PoeTradeApiJsonSerializer(IJsonSerializerWrapper jsonSerializer)
        {
            this.JsonSerializer = jsonSerializer;
        }

        protected IJsonSerializerWrapper JsonSerializer { get; }

        public virtual T? Deserialize<T>(string json) => this.JsonSerializer.Deserialize<T>(json, CamelCaseJsonSerializerOptions);

        public string Serialize(object value) => this.JsonSerializer.Serialize(value, SnakeCaseJsonSerializerOptions);

        private static void IgnoreExtensionData(JsonTypeInfo typeInfo)
        {
            var extensionDataProperty = typeInfo.Properties.FirstOrDefault(p => p.IsExtensionData);
            extensionDataProperty?.Set = null;
        }
    }
}